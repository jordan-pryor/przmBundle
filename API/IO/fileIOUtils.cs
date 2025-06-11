// FileIOUtils.cs
// Utility class for file and directory wrangling — kind of like a digital janitor with a flashlight.

using System;
using System.Collections.Generic;
using System.IO;

namespace przmBundleSystem.API.IO
{
    public static class fileIOUtils
    {
        // Recursively gets all files in a directory with relative paths
        public static IEnumerable<string> GetAllFiles(string baseDirectory)
        {
            if (!Directory.Exists(baseDirectory))
                throw new DirectoryNotFoundException($"Directory '{baseDirectory}' not found.");

            var allFiles = Directory.GetFiles(baseDirectory, "*", SearchOption.AllDirectories);
            foreach (var file in allFiles)
            {
                yield return Path.GetRelativePath(baseDirectory, file).Replace("\\", "/");
            }
        }

        // Ensures the directory exists before writing a file
        public static void EnsureDirectoryForFile(string filePath)
        {
            string? dir = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);
        }

        // Writes a byte array to a file, ensuring directory exists
        public static void WriteFileSafe(string outputPath, byte[] data)
        {
            EnsureDirectoryForFile(outputPath);
            File.WriteAllBytes(outputPath, data);
        }
    }
}