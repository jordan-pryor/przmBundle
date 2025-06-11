// CryptoUtils.cs
// Handy cryptographic helper methods — because even ninjas need tools for their secret codes.

using System;
using System.Security.Cryptography;
using System.Text;

namespace przmBundleSystem.API.Encryption
{
    public static class CryptoUtils
    {
        /// <summary>
        /// Generates a random cryptographically secure byte array of specified length.
        /// </summary>
        public static byte[] GenerateRandomBytes(int length)
        {
            var bytes = new byte[length];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }
            return bytes;
        }

        /// <summary>
        /// Derives a 256-bit AES key from a passphrase using PBKDF2 (Rfc2898).
        /// </summary>
        public static byte[] DeriveKey(string passphrase, byte[] salt, int iterations = 100_000)
        {
            using var kdf = new Rfc2898DeriveBytes(passphrase, salt, iterations, HashAlgorithmName.SHA256);
            return kdf.GetBytes(32); // 256-bit key
        }

        /// <summary>
        /// Computes SHA256 hash of the input data.
        /// </summary>
        public static byte[] ComputeSha256(byte[] data)
        {
            using var sha256 = SHA256.Create();
            return sha256.ComputeHash(data);
        }

        /// <summary>
        /// Converts byte array to a hex string (lowercase).
        /// </summary>
        public static string ToHexString(byte[] bytes)
        {
            var sb = new StringBuilder(bytes.Length * 2);
            foreach (var b in bytes)
                sb.AppendFormat("{0:x2}", b);
            return sb.ToString();
        }

        /// <summary>
        /// Converts a hex string to a byte array.
        /// </summary>
        public static byte[] FromHexString(string hex)
        {
            if (hex.Length % 2 != 0)
                throw new ArgumentException("Invalid hex string length.");

            var bytes = new byte[hex.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }
            return bytes;
        }
    }
}