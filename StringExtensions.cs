using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace mk.helpers
{
    public static class StringExtensions
    {

        public static string ToKMB(this string data)
        {
            decimal value= decimal.Zero;
            return decimal.TryParse(data,out value) ?  ToKMB(value) : null;
        }
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

        // http://stackoverflow.com/questions/2134161/format-number-like-stack-overflow-rounded-to-thousands-with-k-suffix
        public static string KiloFormat(this int num)
        {
            if (num >= 100000000)
                return (num / 1000000).ToString("#,0M");

            if (num >= 10000000)
                return (num / 1000000).ToString("0.#") + "M";

            if (num >= 100000)
                return (num / 1000).ToString("#,0K");

            if (num >= 10000)
                return (num / 1000).ToString("0.#") + "K";

            return num.ToString("#,0");
        }

        public static string KiloFormat(this decimal num)
        {
            if (num >= 100000000)
                return (num / 1000000).ToString("#,0M");

            if (num >= 10000000)
                return (num / 1000000).ToString("0.#") + "M";

            if (num >= 100000)
                return (num / 1000).ToString("#,0K");

            if (num >= 10000)
                return (num / 1000).ToString("0.#") + "K";

            return num.ToString("#,0");
        }

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

        public static string ToHtml(this string text)
        {
            text = HttpUtility.HtmlEncode(text);
            text = text.Replace("\r\n", "\r");
            text = text.Replace("\n", "\r");
            text = text.Replace("\r", "<br>\r\n");
            text = text.Replace("  ", " &nbsp;");
            return text;
        }

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

        public static bool IsNullOrEmpty(this string str) => string.IsNullOrEmpty(str);
        public static bool IsNullOrWhiteSpace(this string str) => string.IsNullOrWhiteSpace(str);

        public static string ToSlug(this string phrase)
        {
            var str = phrase.RemoveAccent().ToLower();   
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
            str = Regex.Replace(str, @"\s+", " ").Trim();
            str = Regex.Replace(str, @"\s", "-");
            return str;
        }

        public static string RemoveAccent(this string txt)
        {
            byte[] bytes = System.Text.Encoding.GetEncoding("Cyrillic").GetBytes(txt);
            return System.Text.Encoding.ASCII.GetString(bytes);
        }

        public static string TrimStart(this string target, string trimString)
        {
            var result = target;
            while (result.StartsWith(trimString))
                result = result.Substring(trimString.Length);
            return result;
        }

        public static string TrimEnd(this string target, string trimString)
        {
            var result = target;
            while (result.EndsWith(trimString))
                result = result.Substring(0, result.Length - trimString.Length);
            return result;
        }

        public static string ToTitleCase(this string str)
        {
            if (string.IsNullOrEmpty(str)) return str;
            var cultureInfo = System.Threading.Thread.CurrentThread.CurrentCulture;
            return cultureInfo.TextInfo.ToTitleCase(str.ToLower());
        }
        public static string ToTitleCase(this string str, string cultureInfoName)
        {
            if (string.IsNullOrEmpty(str)) return str;
            var cultureInfo = new CultureInfo(cultureInfoName);
            return cultureInfo.TextInfo.ToTitleCase(str.ToLower());
        }

        public static string ToTitleCase(this string str, CultureInfo cultureInfo)
        {
            if (string.IsNullOrEmpty(str)) return str;
            return cultureInfo.TextInfo.ToTitleCase(str.ToLower());
        }

        public static string ToUpperCase(this string str)
        {
            if (string.IsNullOrEmpty(str)) return str;
            return str.ToUpper();
        }

        public static string ConvertCapsToWords(this string str)
        {
            return Regex.Replace(str, "(\\B[A-Z])", " $1");
        }

        public static string NormalizePostcode(this string postcode)
        {
            if (string.IsNullOrEmpty(postcode)) return null;
            postcode = postcode.ToUpper();
            if (postcode.Length >= 6)
            {
                postcode = postcode.Trim().Replace(" ", "");
                postcode= postcode.Insert(postcode.Length-3, " ");
                if (postcode.Length > 8)
                    postcode = postcode.Replace(" ", "");// Undo the above. This is not a postcode!
                return postcode;
            }
            postcode = postcode.Trim();
            return postcode;
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

