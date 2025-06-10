// Crc32.cs
// Computes CRC32 checksums — like a paranoid librarian checking every page for tampering.

using System;

namespace przmBundleSystem.API.Utils
{
    public static class crc32
    {
        private static readonly uint[] Table;

        static crc32()
        {
            const uint polynomial = 0xEDB88320;
            Table = new uint[256];
            for (uint i = 0; i < 256; i++)
            {
                uint crc = i;
                for (int j = 0; j < 8; j++)
                {
                    if ((crc & 1) == 1)
                        crc = (crc >> 1) ^ polynomial;
                    else
                        crc >>= 1;
                }
                Table[i] = crc;
            }
        }

        /// <summary>
        /// Computes the CRC32 of a byte array.
        /// </summary>
        public static uint Compute(byte[] data)
        {
            uint crc = 0xFFFFFFFF;
            for (int i = 0; i < data.Length; i++)
            {
                byte index = (byte)((crc ^ data[i]) & 0xFF);
                crc = (crc >> 1) ^ Table[index];
            }
            return ~crc;
        }

        /// <summary>
        /// Computes the CRC32 of a portion of a byte array.
        /// </summary>
        public static uint Compute(byte[] data, int offset, int length)
        {
            uint crc = 0xFFFFFFFF;
            for (int i = offset; i < offset + length; i++)
            {
                byte index = (byte)((crc ^ data[i]) & 0xFF);
                crc = (crc >> 1) ^ Table[index];
            }
            return ~crc;
        }
    }
}