using System.IO;

namespace H.Necessaire
{
    public static class FileSystemExtensions
    {
        static readonly char[] invalidFileNameChars = new char[] { '"', '<', '>', '|', '\0', '\u0001', '\u0002', '\u0003', '\u0004', '\u0005', '\u0006', '\a', '\b', '\t', '\n', '\v', '\f', '\r', '\u000e', '\u000f', '\u0010', '\u0011', '\u0012', '\u0013', '\u0014', '\u0015', '\u0016', '\u0017', '\u0018', '\u0019', '\u001a', '\u001b', '\u001c', '\u001d', '\u001e', '\u001f', ':', '*', '?', '\\', '/' };
        static readonly string invalidFileNameCharsRegEx = string.Format(@"([{0}]*\.+$)|([{0}]+)", System.Text.RegularExpressions.Regex.Escape(new string(invalidFileNameChars)));

        public static string ToSafeFileName(this string value, int? maxLength = 130, string replacementValue = null, bool removeSpaces = true)
        {
            if (string.IsNullOrWhiteSpace(value))
                return replacementValue ?? string.Empty;

            string escapedValue = System.Text.RegularExpressions.Regex.Replace(value, invalidFileNameCharsRegEx, replacementValue ?? string.Empty);
            if (removeSpaces)
                escapedValue = escapedValue.Replace(" ", replacementValue ?? string.Empty);

            if (maxLength == null || escapedValue.Length <= maxLength.Value)
                return escapedValue;

            return escapedValue.Substring(0, maxLength.Value);
        }

        /// <summary>
        /// Copies an entire folder and its contents, deep/recursively; overwrites any existing folders/files
        /// Destination folder is created if it doesn't exist
        /// </summary>
        /// <param name="source">Folder to copy</param>
        /// <param name="target">Destination folder to copy to</param>
        public static void CopyTo(this DirectoryInfo source, DirectoryInfo target)
        {
            if (source?.Exists != true)
            {
                return;
            }

            if (target is null)
            {
                return;
            }

            if (source.FullName.Is(target.FullName))
            {
                return;
            }

            // Check if the target directory exists, if not, create it.
            target.Create();

            // Copy each file into it's new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                fi.CopyTo(Path.Combine(target.ToString(), fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                diSourceSubDir.CopyTo(nextTargetSubDir);
            }
        }
    }
}
