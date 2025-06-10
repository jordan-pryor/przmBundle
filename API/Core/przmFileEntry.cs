using System;

// This little data goblin keeps track of each file in the bundle — where it lives, how big it is, and if it’s behaving.
namespace przmBundleSystem.API.Core
{
    public class przmFileEntry
    {
        public required string FilePath { get; set; } // Virtual path inside the bundle (e.g., Scripts/init.lua)
        public long Offset { get; set; }              // Byte offset inside the PRZM file
        public int CompressedSize { get; set; }       // Size after compression + encryption
        public int OriginalSize { get; set; }         // Original file size
        public string Hash { get; set; } = string.Empty; // SHA-256 of original file for integrity

        // Human-friendly override so you can easily debug the manifest
        public override string ToString() =>
            $"{FilePath} (Original: {OriginalSize}B, Compressed: {CompressedSize}B, Offset: {Offset})";
    }
}