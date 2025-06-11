using System;
using System.IO;
using System.Security.Cryptography;
using przmBundleSystem.API.Utils;

// This bad boy wraps your data in layers of encryption like a cyber-onion. AES-256, CBC, IV-in-header.
namespace przmBundleSystem.API.Encryption
{
    public static class przmEncryptor
    {
        // Encrypts and returns: [16-byte IV][ciphertext]
        public static byte[] Encrypt(byte[] data, byte[] aesKey)
        {
            using Aes aes = Aes.Create();
            aes.KeySize = 256;
            aes.BlockSize = 128;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            aes.Key = aesKey;
            aes.GenerateIV(); // New IV per file

            using MemoryStream ms = new();
            ms.Write(aes.IV, 0, aes.IV.Length); // Prepend IV

            using CryptoStream cs = new(ms, aes.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(data, 0, data.Length);
            cs.FlushFinalBlock();

            Logger.Log("[ENCRYPT] Encrypted data with AES-256-CBC", Logger.LogLevel.Debug);
            return ms.ToArray();
        }

        // Expects: [16-byte IV][ciphertext]
        public static byte[] Decrypt(byte[] encryptedData, byte[] aesKey)
        {
            if (encryptedData.Length < 16)
                throw new Exception("Encrypted data too short — missing IV?");

            byte[] iv = new byte[16];
            Array.Copy(encryptedData, 0, iv, 0, 16);

            using Aes aes = Aes.Create();
            aes.KeySize = 256;
            aes.BlockSize = 128;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = aesKey;
            aes.IV = iv;

            using MemoryStream ms = new();
            using CryptoStream cs = new(ms, aes.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(encryptedData, 16, encryptedData.Length - 16);
            cs.FlushFinalBlock();

            Logger.Log("[DECRYPT] Decrypted file block", Logger.LogLevel.Debug);
            return ms.ToArray();
        }
    }
}