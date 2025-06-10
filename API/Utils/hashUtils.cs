using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace przmBundleSystem.API.Utils
{
    // This one’s your forensic tech. Runs around sniffing every byte and goes “Yup, that’s changed.”
    public static class hashUtils
    {
        /// <summary>
        /// Gets the SHA256 hash of a byte array as a hex string.
        /// </summary>
        public static string ComputeSha256(byte[] data)
        {
            using SHA256 sha256 = SHA256.Create();
            byte[] hashBytes = sha256.ComputeHash(data);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
        }

        /// <summary>
        /// Gets the SHA256 hash of a file at a given path.
        /// </summary>
        public static string ComputeSha256FromFile(string filePath)
        {
            using FileStream stream = File.OpenRead(filePath);
            using SHA256 sha256 = SHA256.Create();
            byte[] hashBytes = sha256.ComputeHash(stream);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
        }

        /// <summary>
        /// (Optional) Gets the MD5 hash of a byte array. Only for debugging / ID purposes.
        /// </summary>
        public static string ComputeMd5(byte[] data)
        {
            using MD5 md5 = MD5.Create();
            byte[] hashBytes = md5.ComputeHash(data);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
        }

        /// <summary>
        /// Compares two SHA256 strings in constant time to prevent timing attacks.
        /// </summary>
        public static bool CompareHashes(string hashA, string hashB)
        {
            if (hashA.Length != hashB.Length) return false;

            bool match = true;
            for (int i = 0; i < hashA.Length; i++)
                match &= hashA[i] == hashB[i];

            return match;
        }
    }
}