using System;
using System.Collections.Generic;
using System.IO;
using przmBundleSystem.API.Compression;
using przmBundleSystem.API.Encryption;
using przmBundleSystem.API.Core;
using przmBundleSystem.API.Utils;

// This reads the fat encrypted meatball and serves you back clean, juicy, decompressed assets.
namespace przmBundleSystem.API.IO
{
    public class przmBundleReader
    {
        private readonly Dictionary<string, przmFileEntry> _fileIndex;
        private readonly BinaryReader _reader;
        private readonly byte[] _aesKey;
        private readonly compressionType _compression;

        public przmBundleReader(string filePath, devKeyManager.DevKey devKey)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Bundle not found: {filePath}");

            _reader = new BinaryReader(File.OpenRead(filePath));
            _aesKey = devKeyManager.GetRawKey(devKey);

            // Load manifest (from end)
            przmManifest manifest = przmManifest.ReadManifest(_reader);
            _compression = manifest.Compression;

            _fileIndex = new Dictionary<string, przmFileEntry>();
            foreach (var entry in manifest.Files)
                _fileIndex[entry.FilePath] = entry;

            Logger.Log($"[READ] Loaded manifest with {_fileIndex.Count} entries from '{filePath}'", Logger.LogLevel.Info);
        }

        // Extracts and returns a file as a byte array
        public byte[] Extract(string virtualPath)
        {
            if (!_fileIndex.ContainsKey(virtualPath))
                throw new Exception($"File '{virtualPath}' not found in bundle");

            var entry = _fileIndex[virtualPath];
            _reader.BaseStream.Seek(entry.Offset, SeekOrigin.Begin);
            byte[] encrypted = _reader.ReadBytes(entry.CompressedSize);

            // Decrypt
            byte[] compressed = przmEncryptor.Decrypt(encrypted, _aesKey);

            // Decompress
            byte[] original = PrzmCompressor.Decompress(compressed, _compression);

            if (original.Length != entry.OriginalSize)
                throw new Exception($"Size mismatch for '{virtualPath}'");

            return original;
        }

        public IEnumerable<string> ListFiles() => _fileIndex.Keys;
    }
}