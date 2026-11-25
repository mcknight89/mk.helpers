using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace mk.helpers
{
    /// <summary>
    /// Path checks and lookups.
    /// </summary>
    public static class PathHelper
    {
        private static readonly StringComparison _comparison = Path.DirectorySeparatorChar == '\\' ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

        /// <summary>
        /// True when the path is the folder or anything under it.
        /// </summary>
        public static bool IsInside(string path, string folder)
        {
            var fullPath = Path.GetFullPath(path).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            var fullFolder = Path.GetFullPath(folder).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            if (string.Equals(fullPath, fullFolder, _comparison))
                return true;

            return fullPath.StartsWith(fullFolder + Path.DirectorySeparatorChar, _comparison);
        }

        /// <summary>
        /// Combines a folder and a relative path, or returns null when the result would leave the folder.
        /// </summary>
        public static string SafeCombine(string folder, string relative)
        {
            var combined = Path.GetFullPath(Path.Combine(folder, relative));
            return IsInside(combined, folder) ? combined : null;
        }

        /// <summary>
        /// Walks up from the start folder and returns the first folder that matches, or null when none do.
        /// </summary>
        public static string FindUp(string start, Func<DirectoryInfo, bool> match)
        {
            var folder = new DirectoryInfo(Path.GetFullPath(start));
            while (folder != null)
            {
                if (match(folder))
                    return folder.FullName;

                folder = folder.Parent;
            }

            return null;
        }

        /// <summary>
        /// Turns backslashes into forward slashes.
        /// </summary>
        public static string ToForwardSlashes(string path)
        {
            return path?.Replace('\\', '/');
        }

        /// <summary>
        /// An 8-character hash of the full path, ignoring case and a trailing slash.
        /// </summary>
        public static string ShortHash(string path)
        {
            var fullPath = Path.GetFullPath(path).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).ToLowerInvariant();
            using (var sha = SHA1.Create())
            {
                var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(fullPath));
                return string.Concat(hash.Take(4).Select(x => x.ToString("x2")));
            }
        }
    }
}
