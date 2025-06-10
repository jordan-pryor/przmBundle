// This enum is the all-you-can-eat buffet of compression options.
// Use it to tell the PRZM system how aggressively to squish your files.
namespace przmBundleSystem.API.Compression
{
	public enum compressionType
	{
		// This is the "I'm in a hurry and have disk space to spare" option.
		None = 0,

		// LZ4 is like the sports car of compression: fast, light, and gets the job done.
		LZ4 = 1,

		// ZSTD is the chonky, thorough uncle of compression—bigger, slower, but squeeeeezes hard.
		ZSTD = 2,

		// Future-proofing — you never know when you’ll need that sweet Brotli action.
		// Brotli = 3, // Uncomment if added

		// When someone invents quantum compression, we'll add it here.
	}
}
