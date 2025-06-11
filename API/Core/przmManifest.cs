using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using przmBundleSystem.API.Compression;
using przmBundleSystem.API.Utils;

// This is the big list of “what’s inside” the bundle. Like an ingredients label, but for files and nerds.
namespace przmBundleSystem.API.Core
{
	public class przmManifest
	{
		public compressionType Compression { get; set; }
		public List<przmFileEntry> Files { get; set; } = new();

		private const int ManifestMagic = 0x50525A4D; // 'PRZM' in hex
		private const int ManifestVersion = 1;

		// Called after all data is written; appends manifest JSON and metadata footer
		public static void WriteManifest(BinaryWriter writer, przmManifest manifest)
		{
			long manifestStart = writer.BaseStream.Position;

			var json = JsonSerializer.Serialize(manifest, new JsonSerializerOptions { WriteIndented = false });
			byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(json);

			writer.Write(jsonBytes);

			// Write footer with manifest length, version, magic
			writer.Write(jsonBytes.Length);       // 4 bytes
			writer.Write(ManifestVersion);        // 4 bytes
			writer.Write(ManifestMagic);          // 4 bytes

			Logger.Log($"[MANIFEST] Written manifest ({manifest.Files.Count} files)", Logger.LogLevel.Debug);
		}

		// Called by the reader to load the manifest from the end of the file
		public static przmManifest ReadManifest(BinaryReader reader)
		{
			reader.BaseStream.Seek(-12, SeekOrigin.End); // Footer = 12 bytes
			int manifestLength = reader.ReadInt32();
			int version = reader.ReadInt32();
			int magic = reader.ReadInt32();

			if (magic != ManifestMagic)
				throw new InvalidDataException("Invalid PRZM manifest footer");

			reader.BaseStream.Seek(-12 - manifestLength, SeekOrigin.End);
			byte[] manifestBytes = reader.ReadBytes(manifestLength);
			var manifest = JsonSerializer.Deserialize<przmManifest>(manifestBytes);

			Logger.Log($"[MANIFEST] Loaded manifest (version {version})", Logger.LogLevel.Debug);
			return manifest ?? throw new Exception("Failed to parse manifest");
		}
	}
}
