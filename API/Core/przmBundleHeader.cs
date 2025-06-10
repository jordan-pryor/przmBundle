// przmBundleHeader.cs
// This is the majestic front gate to every .przm file — it tells us if this bundle is legit or just a suspicious pretender.

using System;
using System.IO;
using System.Text;

namespace przmBundleSystem.API.Core
{
    public static class przmBundleHeader
    {
        public const string Magic = "PRZM"; // Because PRZM is way cooler than ZIP
        public const int Version = 1;
        public const int HeaderSize = 16; // 4 bytes magic + 4 bytes version + 8 bytes reserved (future-proof)

        // Writes the header to the bundle stream
        public static void WriteHeader(BinaryWriter writer)
        {
            writer.Write(Encoding.ASCII.GetBytes(Magic));
            writer.Write(Version);
            writer.Write(new byte[8]); // Reserved space, for future shenanigans
        }

        // Validates and reads the header from the bundle stream
        public static void ReadHeader(BinaryReader reader)
        {
            byte[] magicBytes = reader.ReadBytes(4);
            string magic = Encoding.ASCII.GetString(magicBytes);

            if (magic != Magic)
                throw new InvalidDataException($"Invalid PRZM magic header. Expected '{Magic}', got '{magic}'");

            int version = reader.ReadInt32();
            if (version != Version)
                throw new InvalidDataException($"Unsupported PRZM version: {version}");

            reader.ReadBytes(8); // Skip reserved
        }
    }
}