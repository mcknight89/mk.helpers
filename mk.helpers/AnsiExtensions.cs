using System.Text.RegularExpressions;

namespace mk.helpers
{
    /// <summary>
    /// Cleans console output for showing or saving as plain text.
    /// </summary>
    public static class AnsiExtensions
    {
        // colour and cursor codes, window titles, two-character escapes
        private static readonly Regex _escapes = new Regex(@"\x1b(\[[0-?]*[ -/]*[@-~]|\][^\x07\x1b]*(\x07|\x1b\\)?|[@-Z\\-_])", RegexOptions.Compiled);

        /// <summary>
        /// Removes ANSI escape codes, such as colours, cursor moves and window titles.
        /// </summary>
        public static string StripAnsi(this string text)
        {
            if (string.IsNullOrEmpty(text) || text.IndexOf('\x1b') < 0)
                return text;

            return _escapes.Replace(text, "");
        }
    }
}
