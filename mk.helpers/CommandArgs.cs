using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mk.helpers
{
    /// <summary>
    /// Splits and joins command-line arguments using Windows quoting rules.
    /// </summary>
    public static class CommandArgs
    {
        private static readonly char[] _needsQuotes = { ' ', '\t', '"', '&', '|', '<', '>', '^', '(', ')' };

        /// <summary>
        /// Splits a command line into arguments, respecting quotes and backslash-escaped quotes.
        /// </summary>
        public static List<string> Split(string line)
        {
            var args = new List<string>();
            if (string.IsNullOrWhiteSpace(line))
                return args;

            var current = new StringBuilder();
            var inQuotes = false;
            var hasArg = false;
            for (var i = 0; i < line.Length; i++)
            {
                var c = line[i];
                if (c == '\\')
                {
                    var end = i;
                    while (end < line.Length && line[end] == '\\')
                        end++;

                    var count = end - i;
                    hasArg = true;
                    if (end < line.Length && line[end] == '"')
                    {
                        // backslashes before a quote are escapes: 2n give n, 2n+1 give n and a literal quote
                        current.Append('\\', count / 2);
                        if (count % 2 == 1)
                        {
                            current.Append('"');
                            i = end;
                        }
                        else
                        {
                            i = end - 1;
                        }
                    }
                    else
                    {
                        current.Append('\\', count);
                        i = end - 1;
                    }

                    continue;
                }

                if (c == '"')
                {
                    if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                    {
                        current.Append('"');
                        i++;
                    }
                    else
                    {
                        inQuotes = !inQuotes;
                    }

                    hasArg = true;
                    continue;
                }

                if ((c == ' ' || c == '\t') && !inQuotes)
                {
                    if (hasArg)
                    {
                        args.Add(current.ToString());
                        current.Clear();
                        hasArg = false;
                    }

                    continue;
                }

                current.Append(c);
                hasArg = true;
            }

            if (hasArg)
                args.Add(current.ToString());

            return args;
        }

        /// <summary>
        /// Joins arguments into one command line, quoting each one that needs it.
        /// </summary>
        public static string Join(IEnumerable<string> args)
        {
            return string.Join(" ", args.Select(Quote));
        }

        /// <summary>
        /// Quotes an argument when it needs it.
        /// </summary>
        public static string Quote(string arg)
        {
            if (string.IsNullOrEmpty(arg))
                return "\"\"";

            if (arg.IndexOfAny(_needsQuotes) < 0)
                return arg;

            var result = new StringBuilder("\"");
            var backslashes = 0;
            foreach (var c in arg)
            {
                if (c == '\\')
                {
                    backslashes++;
                    continue;
                }

                if (c == '"')
                {
                    result.Append('\\', backslashes * 2 + 1);
                    result.Append('"');
                }
                else
                {
                    result.Append('\\', backslashes);
                    result.Append(c);
                }

                backslashes = 0;
            }

            result.Append('\\', backslashes * 2);
            result.Append('"');
            return result.ToString();
        }
    }
}
