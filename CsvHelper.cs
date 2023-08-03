﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace mk.helpers
{
    public static class CsvHelper
    {
        public static string ToCsv<T>(this IEnumerable<T> items) where T : class
            => ToCsv<T>(items, false, ',');
        public static string ToCsv<T>(this IEnumerable<T> items, char delimiter = ',') where T : class
            => ToCsv<T>(items, false, delimiter);
        public static string ToCsv<T>(this List<T> items) where T : class
      => ToCsv<T>(items, false, ',');
        public static string ToCsv<T>(this List<T> items, char delimiter = ',') where T : class
            => ToCsv<T>(items, false, delimiter);



        public static string ToCsv<T>(this IEnumerable<T> items, bool addHeader = false, char delimiter = ',') where T : class
        {
            using (var sw = new StringWriter())
            {
                foreach (var item in items)
                {
                    sw.Write(ToCsv(item, addHeader, delimiter));
                    addHeader = false;
                }
                return sw.ToString();
            }
        }

        public static string ToCsv<T>(this T[] items, bool addHeader = false, char delimiter = ',') where T : class
        {
            using (var sw = new StringWriter())
            {
                foreach (var item in items)
                {
                    sw.Write(ToCsv(item, addHeader, delimiter));
                    addHeader = false;
                }
                return sw.ToString();
            }
        }

        public static string ToCsv<T>(T item, bool addHeader = false, char delimiter = ',') where T : class
        {
            string _EscapeString(string value)
            {
                var mustQuote = value.Any(x => x == ',' || x == '\"' || x == '\r' || x == '\n');
                value = value.Replace("\"", "\"\"");
                return string.Format("\"{0}\"", value);
            }

            var properties = typeof(T).GetProperties()
                .Where(n =>
                    n.PropertyType == typeof(string) ||
                    n.PropertyType == typeof(bool) ||
                    n.PropertyType == typeof(char) ||
                    n.PropertyType == typeof(byte) ||
                    n.PropertyType == typeof(decimal) ||
                    n.PropertyType == typeof(int) ||
                    n.PropertyType == typeof(DateTime) ||
                    n.PropertyType == typeof(DateTime?) ||
                    n.PropertyType == typeof(double) || // Additional primitive types
                    n.PropertyType == typeof(float) ||
                    n.PropertyType == typeof(short) ||
                    n.PropertyType == typeof(ushort) ||
                    n.PropertyType == typeof(long) ||
                    n.PropertyType == typeof(ulong) ||
                    n.PropertyType == typeof(sbyte)
                );

            using (var sw = new StringWriter())
            {
                if (addHeader)
                {
                    var header = properties
                        .Select(n => n.Name)
                        .Aggregate((a, b) => a + delimiter + b);
                    sw.WriteLine(header);
                }

                var fields = properties
                    .Select(n => n.GetValue(item, null))
                    .Select(n =>
                    {
                        if (n is bool boolValue)
                        {
                            return boolValue ? "true" : "false";
                        }
                        return n == null ? "null" : n.ToString();
                    })
                    .Select(n => n.GetType() == typeof(string) ? _EscapeString(n.ToString()) : n.ToString())
                    .ToList();

                var agg = string.Join(delimiter.ToString(), fields);
                sw.WriteLine(agg);

                return sw.ToString();
            }

        }
    }
}
