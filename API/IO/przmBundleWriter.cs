using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using przmBundleSystem.API.Compression;
using przmBundleSystem.API.Encryption;
using przmBundleSystem.API.Utils;
using przmBundleSystem.API.Core;

// This takes your lovingly crafted mod folder and turns it into a fat encrypted meatball of game content.
namespace przmBundleSystem.API.IO
{
	public static class przmBundleWriter
	{
		public static void WriteBundle(
			string inputDirectory,
			string outputFilePath,
			devKeyManager.DevKey devKey,
			compressionType compressionType = compressionType.LZ4)
		{
			if (!Directory.Exists(inputDirectory))
			{
				Logger.Log($"[WRITE] Input directory not found: {inputDirectory}", Logger.LogLevel.Error);
				return;
			}

			var fileEntries = new List<przmFileEntry>();
			using var outStream = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write);
			using var writer = new BinaryWriter(outStream);

			Logger.Log($"[WRITE] Packing from: {inputDirectory}", Logger.LogLevel.Info);

			string[] files = Directory.GetFiles(inputDirectory, "*", SearchOption.AllDirectories);
			long currentOffset = 0;

			foreach (var filePath in files)
			{
				string relativePath = Path.GetRelativePath(inputDirectory, filePath).Replace('\\', '/');
				byte[] fileData = File.ReadAllBytes(filePath);

				// Compress
				byte[] compressed = PrzmCompressor.Compress(fileData, compressionType);

				// Encrypt
				byte[] encrypted = przmEncryptor.Encrypt(compressed, devKeyManager.GetRawKey(devKey));

				// Save file entry
				var entry = new przmFileEntry
				{
					FilePath = relativePath,
					Offset = currentOffset,
					CompressedSize = encrypted.Length,
					OriginalSize = fileData.Length,
					Hash = hashUtils.ComputeSha256(fileData)
				};

				fileEntries.Add(entry);

				// Write encrypted bytes
				writer.Write(encrypted);
				currentOffset += encrypted.Length;

				Logger.Log($"[WRITE] + {relativePath} ({fileData.Length} bytes)", Logger.LogLevel.Success);
			}

			// Save manifest at end of file
			var manifest = new przmManifest
			{
				Compression = compressionType,
				Files = fileEntries
			};

			przmManifest.WriteManifest(writer, manifest);
			Logger.Log($"[WRITE] Wrote {fileEntries.Count} files into {outputFilePath}", Logger.LogLevel.Info);
		}
	}
}
