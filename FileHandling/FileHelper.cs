using System;
using System.IO;

namespace FileHandling
{
    public static class FileHelper
    {
        public static string GetFileNameWithoutExtension(string filePath)
        {
            return Path.GetFileNameWithoutExtension(filePath);
        }

        public static string GetFileExtension(string filePath)
        {
            return Path.GetExtension(filePath);
        }

        public static string GetFileName(string filePath)
        {
            return Path.GetFileName(filePath);
        }

        public static string CombinePaths(params string[] paths)
        {
            return Path.Combine(paths);
        }

        public static bool HasExtension(string filePath, string extension)
        {
            return Path.GetExtension(filePath).Equals(extension, StringComparison.OrdinalIgnoreCase);
        }
    }
}
