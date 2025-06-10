using System;

// This is the mouthpiece of the PRZM system — it yells important things at you in technicolor.
namespace przmBundleSystem.API.Utils
{
	public static class Logger
	{
		public enum LogLevel
		{
			Info,
			Warning,
			Error,
			Success,
			Debug
		}

		// This is the main brain — it handles everything from helpful hints to catastrophic screaming.
		public static void Log(string message, LogLevel level = LogLevel.Info)
		{
			Console.ForegroundColor = GetColor(level);
			Console.Write($"[{DateTime.Now:HH:mm:ss}] ");

			Console.Write(level switch
			{
				LogLevel.Info => "[INFO] ",
				LogLevel.Warning => "[WARN] ",
				LogLevel.Error => "[ERROR]",
				LogLevel.Success => "[ OK ] ",
				LogLevel.Debug => "[DEBUG]",
				_ => "[?????]"
			});

			Console.ResetColor();
			Console.Write(" ");

			Console.ForegroundColor = GetColor(level);
			Console.WriteLine(message);
			Console.ResetColor();
		}

		// This is where we choose the paint for the scream. Or the whisper.
		private static ConsoleColor GetColor(LogLevel level)
		{
			return level switch
			{
				LogLevel.Info => ConsoleColor.Gray,
				LogLevel.Warning => ConsoleColor.Yellow,
				LogLevel.Error => ConsoleColor.Red,
				LogLevel.Success => ConsoleColor.Green,
				LogLevel.Debug => ConsoleColor.Cyan,
				_ => ConsoleColor.White
			};
		}
	}
}