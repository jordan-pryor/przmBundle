using System;
using System.Collections.Generic;
using System.IO;
using przmBundleSystem.API.Compression;
using przmBundleSystem.API.Core;
using przmBundleSystem.API.Utils;
using przmBundleSystem.API.Encryption;
using przmBundleSystem.API.IO;


// This is the brains of the whole PRZM operation — think of it as the Command Center for mod squishery and bundle tomfoolery.
namespace przmBundleSystem
{
	public static class PrzmBundle
	{
		// This is where the magic starts — Pack your folder into a shiny `.przm` like a magician stuffing rabbits into a hat.
		public static void PackFromFolder(string sourceFolder, string outputPath, string devKeyPath, compressionType compression = compressionType.LZ4, bool encrypt = true)
		{
			if (!Directory.Exists(sourceFolder))
				throw new DirectoryNotFoundException($"The source folder '{sourceFolder}' does not exist.");

			Logger.Log($"[PRZM] Packing bundle from '{sourceFolder}'...");

			var devKey = devKeyManager.LoadKey(devKeyPath);

			przmBundleWriter writer = new przmBundleWriter();
			writer.Pack(sourceFolder, outputPath, devKey, compression, encrypt);

			Logger.Log($"[PRZM] Bundle packed to '{outputPath}' successfully.");
		}

		// This is the great unzipper — it'll take that `.przm` and fling the contents into a folder like a confetti cannon.
		public static void UnpackToFolder(string przmPath, string outputFolder, string devKeyPath)
		{
			if (!File.Exists(przmPath))
				throw new FileNotFoundException($"Bundle file '{przmPath}' not found.");

			Logger.Log($"[PRZM] Unpacking bundle '{przmPath}' to '{outputFolder}'...");

			var devKey = devKeyManager.LoadKey(devKeyPath);
			przmBundleReader reader = new przmBundleReader();
			reader.Unpack(przmPath, outputFolder, devKey);

			Logger.Log($"[PRZM] Unpacked to '{outputFolder}' successfully.");
		}

		// This one’s for the game engine — load it up, don’t unpack it, just slurp it into memory like warm RAM noodles.
		public static przmBundleReader LoadFromFile(string przmPath, string devKeyPath)
		{
			if (!File.Exists(przmPath))
				throw new FileNotFoundException($"Bundle file '{przmPath}' not found.");

			Logger.Log($"[PRZM] Loading bundle into memory from '{przmPath}'...");

			var devKey = devKeyManager.LoadKey(devKeyPath);
			przmBundleReader reader = new przmBundleReader();
			reader.Load(przmPath, devKey);

			Logger.Log($"[PRZM] Bundle loaded successfully.");
			return reader;
		}

		// This is the snack-sized method — grab one file from the bundle like a single Pringle™ from the tube.
		public static byte[] ReadFile(string przmPath, string fileName, string devKeyPath)
		{
			var reader = LoadFromFile(przmPath, devKeyPath);

			if (!reader.FileExists(fileName))
			{
				Logger.Log($"[PRZM] File '{fileName}' not found in bundle.");
				return null;
			}

			return reader.ReadFile(fileName);
		}
	}
}
