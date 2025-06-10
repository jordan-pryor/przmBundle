using System;
using K4os.Compression.LZ4;
using przmBundleSystem.API.Compression;

// This is the data squasher. It exists solely to crush your megabytes into kilobytes.
// Don’t worry, it also knows how to uncrush them later.
namespace przmBundleSystem.API.Compression
{
	public static class PrzmCompressor
	{
		// This method takes your fat, juicy bytes and slims them down like a gym membership ad.
		public static byte[] Compress(byte[] data, CompressionType type)
		{
			switch (type)
			{
				case CompressionType.None:
					return data; // raw and uncompressed — the protein shake of bundles.

				case CompressionType.LZ4:
					return LZ4Pickler.Pickle(data); // ultra-fast, mildly effective.

				case CompressionType.ZSTD:
					throw new NotImplementedException("ZSTD compression is not implemented yet. Install ZstdNet and update this method.");

				default:
					throw new ArgumentOutOfRangeException(nameof(type), $"Unsupported compression type: {type}");
			}
		}

		// This method takes compressed data and reinflates it like a sad beach ball on a summer day.
		public static byte[] Decompress(byte[] compressedData, CompressionType type)
		{
			switch (type)
			{
				case CompressionType.None:
					return compressedData;

				case CompressionType.LZ4:
					return LZ4Pickler.Unpickle(compressedData); // quick decompression for snappy game loading.

				case CompressionType.ZSTD:
					throw new NotImplementedException("ZSTD decompression is not implemented yet. Install ZstdNet and update this method.");

				default:
					throw new ArgumentOutOfRangeException(nameof(type), $"Unsupported compression type: {type}");
			}
		}
	}
}
