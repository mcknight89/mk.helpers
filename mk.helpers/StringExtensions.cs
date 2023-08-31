using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace mk.helpers
{

    /// <summary>
    /// Provides extension methods for string manipulation and formatting.
    /// </summary>
    public static class StringExtensions
    {

        /// <summary>
        /// Converts a string representation of a number to a formatted string with K, M, or B suffix.
        /// </summary>
        /// <param name="data">The input string containing the number.</param>
        /// <returns>A formatted string with K, M, or B suffix.</returns>
        public static string ToKMB(this string data)
        {
            decimal value= decimal.Zero;
            return decimal.TryParse(data,out value) ?  ToKMB(value) : null;
        }

        /// <summary>
        /// Converts a decimal number to a formatted string with K, M, or B suffix.
        /// </summary>
        /// <param name="num">The input decimal number.</param>
        /// <returns>A formatted string with K, M, or B suffix.</returns>
        public static string ToKMB(this decimal num)
        {
            if (num > 999999999 || num < -999999999)
            {
                return num.ToString("0,,,.###B", CultureInfo.InvariantCulture);
            }
            else
            if (num > 999999 || num < -999999)
            {
                return num.ToString("0,,.##M", CultureInfo.InvariantCulture);
            }
            else
            if (num > 999 || num < -999)
            {
                return num.ToString("0,.#K", CultureInfo.InvariantCulture);
            }
            else
            {
                return num.ToString(CultureInfo.InvariantCulture);
            }
        }


        /// <summary>
        /// Formats an integer as a string with Kilo notation (e.g., 1.5K).
        /// </summary>
        /// <param name="num">The input integer.</param>
        /// <returns>A string with Kilo notation.</returns>
        public static string KiloFormat(this int num)
        {
            if (num >= 1000000) // Check for millions
                return (num / 1000000.0).ToString("#,0.##M");

            if (num >= 1000) // Check for thousands
                return (num / 1000.0).ToString("0.#") + "K";

            return num.ToString("#,0");
        }


        /// <summary>
        /// Formats an integer as a string with Kilo notation (e.g., 1.5K).
        /// </summary>
        /// <param name="num">The input integer.</param>
        /// <returns>A string with Kilo notation.</returns>
        public static string KiloFormat(this decimal num)
        {
            if (num >= 1000000) // Check for millions
                return (num / 1000000.0m).ToString("#,0.##M");

            if (num >= 1000) // Check for thousands
                return (num / 1000.0m).ToString("0.#") + "K";

            return num.ToString("#,0");
        }


        /// <summary>
        /// Gets the appropriate suffix for a day based on its numeric value.
        /// </summary>
        /// <param name="day">The day value.</param>
        /// <returns>The day suffix (st, nd, rd, or th).</returns>

        public static string GetDaySuffix(this int day)
        {
            switch (day)
            {
                case 1:
                case 21:
                case 31:
                    return "st";
                case 2:
                case 22:
                    return "nd";
                case 3:
                case 23:
                    return "rd";
                default:
                    return "th";
            }
        }
        /// <summary>
        /// Converts a string to its HTML representation with line breaks and non-breaking spaces.
        /// </summary>
        /// <param name="text">The input string.</param>
        /// <returns>The HTML-encoded string with line breaks and non-breaking spaces.</returns>
        public static string ToHtml(this string text)
        {
            text = HttpUtility.HtmlEncode(text);
            text = text.Replace("\r\n", "\r");
            text = text.Replace("\n", "\r");
            text = text.Replace("\r", "<br>\r\n");
            text = text.Replace("  ", " &nbsp;");
            return text;
        }

        /// <summary>
        /// Converts a byte count to a human-readable string representation with appropriate unit suffix.
        /// </summary>
        /// <param name="byteCount">The byte count.</param>
        /// <returns>The human-readable string representation of the byte count.</returns>
        public static string BytesToString(this long byteCount)
        {
            string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB
            if (byteCount == 0)
                return "0" + suf[0];
            var bytes = Math.Abs(byteCount);
            var place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            var num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return $"{(Math.Sign(byteCount) * num)} {suf[place]}";
        }
        /// <summary>
        /// Determines whether the specified string is null or empty.
        /// </summary>
        /// <param name="str">The input string.</param>
        /// <returns>true if the string is null or empty; otherwise, false.</returns>
        public static bool IsNullOrEmpty(this string str) => string.IsNullOrEmpty(str);

        /// <summary>
        /// Determines whether the specified string is null, empty, or consists only of white-space characters.
        /// </summary>
        /// <param name="str">The input string.</param>
        /// <returns>true if the string is null, empty, or consists only of white-space characters; otherwise, false.</returns>
        public static bool IsNullOrWhiteSpace(this string str) => string.IsNullOrWhiteSpace(str);

        /// <summary>
        /// Converts a string to a URL-friendly slug.
        /// </summary>
        /// <param name="phrase">The input string.</param>
        /// <returns>The URL-friendly slug version of the input string.</returns>
        public static string ToSlug(this string phrase)
        {
            var str = phrase.RemoveAccent().ToLower();   
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
            str = Regex.Replace(str, @"\s+", " ").Trim();
            str = Regex.Replace(str, @"\s", "-");
            return str;
        }


        /// <summary>
        /// Converts a string to a byte array using UTF-8 encoding.
        /// </summary>
        /// <param name="str">The input string.</param>
        /// <returns>The byte array representation of the input string using UTF-8 encoding.</returns>
        public static byte[] ToBytesUtf8(this string str)
        {
            return Encoding.UTF8.GetBytes(str);
        }

        /// <summary>
        /// Converts a string to a byte array using UTF-32 encoding.
        /// </summary>
        /// <param name="str">The input string.</param>
        /// <returns>The byte array representation of the input string using UTF-32 encoding.</returns>
        public static byte[] ToBytesUtf32(this string str)
        {
            return Encoding.UTF32.GetBytes(str);
        }

        /// <summary>
        /// Converts a string to a byte array using UTF-7 encoding.
        /// </summary>
        /// <param name="str">The input string.</param>
        /// <returns>The byte array representation of the input string using UTF-7 encoding.</returns>
        public static byte[] ToBytesUtf7(this string str)
        {
            return Encoding.UTF7.GetBytes(str);
        }

        /// <summary>
        /// Converts a string to a byte array using ASCII encoding.
        /// </summary>
        /// <param name="str">The input string.</param>
        /// <returns>The byte array representation of the input string using ASCII encoding.</returns>
        public static byte[] ToBytesAscii(this string str)
        {
            return Encoding.ASCII.GetBytes(str);
        }

        /// <summary>
        /// Converts a string to a byte array using Unicode encoding (UTF-16).
        /// </summary>
        /// <param name="str">The input string.</param>
        /// <returns>The byte array representation of the input string using Unicode encoding (UTF-16).</returns>
        public static byte[] ToBytesUnicode(this string str)
        {
            return Encoding.Unicode.GetBytes(str);
        }

        private static bool _codePagesSetup = false;
        private static void SetupCodePages()
        {
            try
            {
                if (_codePagesSetup)
                    return;

                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

               
            }
            catch
            {

            }
            _codePagesSetup = true;
        }


        /// <summary>
        /// Removes accents from a string using Cyrillic encoding and converts to ASCII encoding.
        /// </summary>
        /// <param name="txt">The input string.</param>
        /// <returns>The string with accents removed and converted to ASCII encoding.</returns>
        public static string RemoveAccent(this string txt)
        {
            SetupCodePages();

            byte[] bytes = Encoding.GetEncoding("Cyrillic").GetBytes(txt);
            return Encoding.ASCII.GetString(bytes);
        }

        /// <summary>
        /// Removes a specified substring from the start of the target string.
        /// </summary>
        /// <param name="target">The target string.</param>
        /// <param name="trimString">The substring to be removed from the start.</param>
        /// <returns>The modified string after trimming the specified substring from the start.</returns>
        public static string TrimStart(this string target, string trimString)
        {
            var result = target;
            while (result.StartsWith(trimString))
                result = result.Substring(trimString.Length);
            return result;
        }

        /// <summary>
        /// Removes a specified substring from the end of the target string.
        /// </summary>
        /// <param name="target">The target string.</param>
        /// <param name="trimString">The substring to be removed from the end.</param>
        /// <returns>The modified string after trimming the specified substring from the end.</returns>
        public static string TrimEnd(this string target, string trimString)
        {
            var result = target;
            while (result.EndsWith(trimString))
                result = result.Substring(0, result.Length - trimString.Length);
            return result;
        }

        /// <summary>
        /// Converts a string to title case using the current thread's culture.
        /// </summary>
        /// <param name="str">The input string.</param>
        /// <returns>The input string converted to title case.</returns>
        public static string ToTitleCase(this string str)
        {
            if (string.IsNullOrEmpty(str)) return str;
            var cultureInfo = System.Threading.Thread.CurrentThread.CurrentCulture;
            return cultureInfo.TextInfo.ToTitleCase(str.ToLower());
        }

        /// <summary>
        /// Converts a string to title case using the specified culture.
        /// </summary>
        /// <param name="str">The input string.</param>
        /// <param name="cultureInfoName">The name of the culture to use.</param>
        /// <returns>The input string converted to title case using the specified culture.</returns>
        public static string ToTitleCase(this string str, string cultureInfoName)
        {
            if (string.IsNullOrEmpty(str)) return str;
            var cultureInfo = new CultureInfo(cultureInfoName);
            return cultureInfo.TextInfo.ToTitleCase(str.ToLower());
        }

        /// <summary>
        /// Converts a string to title case using the specified culture.
        /// </summary>
        /// <param name="str">The input string.</param>
        /// <param name="cultureInfo">The culture to use for title case conversion.</param>
        /// <returns>The input string converted to title case using the specified culture.</returns>
        public static string ToTitleCase(this string str, CultureInfo cultureInfo)
        {
            if (string.IsNullOrEmpty(str)) return str;
            return cultureInfo.TextInfo.ToTitleCase(str.ToLower());
        }

        /// <summary>
        /// Converts a string to uppercase.
        /// </summary>
        /// <param name="str">The input string.</param>
        /// <returns>The input string converted to uppercase.</returns>
        public static string ToUpperCase(this string str)
        {
            if (string.IsNullOrEmpty(str)) return str;
            return str.ToUpper();
        }


        /// <summary>
        /// Short hand method to convert a string to base64 encoded string
        /// </summary>
        /// <param name="str">Input string</param>
        /// <param name="encoding">The type of text encoding to use. Defaults to Encoding.UTF8</param>
        public static string ToBase64(this string str, Encoding encoding=null)
        {
            var encode = encoding ?? Encoding.UTF8;
            var plainTextBytes = encode.GetBytes(str);
            return System.Convert.ToBase64String(plainTextBytes);
        }


        /// <summary>
        /// Short hand method to convert a string to base64 encoded bytes
        /// </summary>
        /// <param name="str">Input string</param>
        /// <param name="encoding">The type of text encoding to use. Defaults to Encoding.UTF8</param>
        public static byte[] ToBase64Bytes(this string str, Encoding encoding = null)
        {
            var encode = encoding ?? Encoding.UTF8;
            var plainTextBytes = encode.GetBytes(str);
            return plainTextBytes;
        }

        /// <summary>
        /// Converts camel case or Pascal case string to words with spaces between uppercase letters.
        /// </summary>
        /// <param name="str">The input string in camel case or Pascal case.</param>
        /// <returns>The input string with spaces added between uppercase letters.</returns>
        public static string ConvertCapsToWords(this string str)
        {
            return Regex.Replace(str, "(\\B[A-Z])", " $1");
        }

        /// <summary>
        /// Normalizes a postcode by converting it to uppercase and adding spaces where appropriate.
        /// </summary>
        /// <param name="postcode">The input postcode string.</param>
        /// <returns>The normalized version of the input postcode.</returns>
        public static string NormalizePostcode(this string postcode)
        {
            if (string.IsNullOrEmpty(postcode)) return null;
            postcode = postcode.ToUpper();
            if (postcode.Length >= 6)
            {
                postcode = postcode.Trim().Replace(" ", "");
                postcode = postcode.Insert(postcode.Length - 3, " ");
                if (postcode.Length > 8)
                    postcode = postcode.Replace(" ", ""); // Undo the above. This is not a postcode!
                return postcode;
            }
            postcode = postcode.Trim();
            return postcode;
        }

        /// <summary>
        /// Replaces a specified substring at the start of the target string with a replacement string.
        /// </summary>
        /// <param name="str">The target string.</param>
        /// <param name="start">The substring to be replaced at the start of the target.</param>
        /// <param name="replacement">The replacement string.</param>
        /// <returns>The modified string after replacing the specified substring at the start.</returns>
        public static string ReplaceStart(this string str, string start, string replacement)
        {
            if (str != null && replacement != null && str.Length >= start.Length && str.Length > 0 && start.Length > 0
                && str.Substring(0, start.Length) == start
                )
            {
                str = string.Concat(replacement, str.Substring(start.Length, str.Length - start.Length));
            }
            return str;
        }



        private static int Compute(string source, string target)
        {
            if ((source == null) || (target == null)) return 0;
            if ((source.Length == 0) || (target.Length == 0)) return 0;
            if (source == target) return source.Length;

            var sourceWordCount = source.Length;
            var targetWordCount = target.Length;

            if (sourceWordCount == 0)
                return targetWordCount;

            if (targetWordCount == 0)
                return sourceWordCount;

            var distance = new int[sourceWordCount + 1, targetWordCount + 1];

            for (var i = 0; i <= sourceWordCount; distance[i, 0] = i++) ;
            for (var j = 0; j <= targetWordCount; distance[0, j] = j++) ;

            for (var i = 1; i <= sourceWordCount; i++)
            {
                for (var j = 1; j <= targetWordCount; j++)
                {
                    var cost = (target[j - 1] == source[i - 1]) ? 0 : 1;
                    distance[i, j] = Math.Min(Math.Min(distance[i - 1, j] + 1, distance[i, j - 1] + 1), distance[i - 1, j - 1] + cost);
                }
            }

            return distance[sourceWordCount, targetWordCount];
        }


        /// <summary>
        /// Computes the Levenshtein similarity between two strings.
        /// </summary>
        /// <param name="source">The source string.</param>
        /// <param name="target">The target string.</param>
        /// <returns>The Levenshtein similarity between the two strings.</returns>

        public static double ComputeSimilarityLevenshtein(this string source, string target)
        {
            if ((source == null) || (target == null)) return 0.0;
            if ((source.Length == 0) || (target.Length == 0)) return 0.0;
            if (source == target) return 1.0;

            var stepsToSame = Compute(source, target);
            return (1.0 - (stepsToSame / (double)Math.Max(source.Length, target.Length)));
        }
    }
}

