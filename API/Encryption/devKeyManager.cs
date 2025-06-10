using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using przmBundleSystem.API.Utils;

// This handles your secret sauce — AES keys that tell the PRZM system who’s boss.
// No key? No access. Just how we like it.
namespace przmBundleSystem.API.Encryption
{
	public static class devKeyManager
	{
		private const int KeySizeBytes = 32; // 256-bit AES

		public class DevKey
		{
			public required string DevName { get; set; }
			public required string DevId { get; set; }
			public required string AesKey { get; set; } // Base64-encoded
		}

		// Generates a new shiny AES key and wraps it with a Dev ID — feels official, doesn't it?
		public static DevKey Generate(string devName)
		{
			using var aes = Aes.Create();
			aes.KeySize = 256;
			aes.GenerateKey();

			string devId = Guid.NewGuid().ToString();
			string keyBase64 = Convert.ToBase64String(aes.Key);

			var devKey = new DevKey
			{
				DevName = devName,
				DevId = devId,
				AesKey = keyBase64
			};

			Logger.Log($"[KEYGEN] Generated DevKey for '{devName}' (ID: {devId})", Logger.LogLevel.Success);
			return devKey;
		}

		// Saves your classified encryption key to a JSON file (don’t post this on GitHub, please).
		public static void SaveKey(DevKey devKey, string path)
		{
			var json = JsonSerializer.Serialize(devKey, new JsonSerializerOptions { WriteIndented = true });
			File.WriteAllText(path, json);
			Logger.Log($"[KEYSAVE] DevKey saved to '{path}'", Logger.LogLevel.Info);
		}

		// Loads a DevKey from a JSON file — no DevKey, no mod juju.
		public static DevKey? LoadKey(string path)
		{
			if (!File.Exists(path))
				throw new FileNotFoundException($"DevKey file not found: {path}");

			var json = File.ReadAllText(path);
			var devKey = JsonSerializer.Deserialize<DevKey>(json);

			Logger.Log($"[KEYLOAD] Loaded DevKey for '{devKey?.DevName}'", Logger.LogLevel.Info);
			return devKey;
		}

		// Converts the DevKey to a raw byte[] for AES use
		public static byte[] GetRawKey(DevKey devKey)
		{
			return Convert.FromBase64String(devKey.AesKey);
		}
	}
}
