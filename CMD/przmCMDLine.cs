using System;
using System.IO;
using przmBundleSystem.API.Encryption; // DevKey manager
using przmBundleSystem;                // PRZM Bundle API

namespace przmBundleSystem.CMD
{
    internal class przmCMDLine
    {
        private static bool _exitRequested = false;

        // ===========================
        // Entry Point
        // ===========================
        static void Main(string[] args)
        {
            Console.Title = "przm-CMDLine-Tool";

            while (!_exitRequested)
            {
                // === Main Menu ===
                Console.Clear();
                Console.WriteLine("=== PRZM CMD-Line Tool ===");
                Console.WriteLine("1. Pack New .przm");
                Console.WriteLine("2. Unpack .przm");
                Console.WriteLine("3. Select or Generate DevKey");
                Console.WriteLine("4. Exit");
                Console.WriteLine("----------------------------");
                Console.WriteLine("Type 'b' to go back anytime, or 'x' to exit.");
                Console.Write("\nSelect an option (1–4): ");

                string choice = Console.ReadLine()?.Trim().ToLower();
                HandleGlobalCommand(choice);
                if (_exitRequested) break;

                switch (choice)
                {
                    case "1":
                        PackBundle();
                        break;
                    case "2":
                        UnpackBundle();
                        break;
                    case "3":
                        SelectOrGenerateDevKey();
                        break;
                    case "4":
                        _exitRequested = true;
                        break;
                    default:
                        Console.WriteLine("Invalid option. Press any key to try again...");
                        Console.ReadKey();
                        break;
                }
            }

            Console.WriteLine("Exiting PRZM Tool. Goodbye!");
        }
        // ===========================
        // 📦 .przm Dominant File Type
        // ===========================
        static string GetDominantFileType(string[] files)
        {
            var typeMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { ".png", "Textures" },
                { ".jpg", "Textures" },
                { ".jpeg", "Textures" },
                { ".bmp", "Textures" },
                { ".ogg", "Audio" },
                { ".mp3", "Audio" },
                { ".wav", "Audio" },
                { ".fbx", "Models" },
                { ".obj", "Models" },
                { ".glb", "Models" },
                { ".gltf", "Models" },
                { ".lua", "Scripts" },
                { ".txt", "Data" },
                { ".json", "Data" },
                { ".shader", "Shaders" }
            };

            var counts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            foreach (var file in files)
            {
                string ext = Path.GetExtension(file);
                if (typeMap.TryGetValue(ext, out string type))
                {
                    if (!counts.ContainsKey(type))
                        counts[type] = 0;

                    counts[type]++;
                }
            }

            if (counts.Count == 0)
                return "Mixed";

            return counts.OrderByDescending(kvp => kvp.Value).First().Key;
        }

        // ===========================
        // 📦 PACK .przm
        // ===========================
        static void PackBundle()
        {
            Console.Clear();
            Console.WriteLine("=== Pack New .przm ===");

            string sourceFolder = PromptUntilValid("Enter folder path to pack: ", Directory.Exists);
            if (_exitRequested || string.IsNullOrWhiteSpace(sourceFolder)) return;

            // Determine dominant file type
            string[] allFiles = Directory.GetFiles(sourceFolder, "*", SearchOption.AllDirectories);
            string dominantType = GetDominantFileType(allFiles);

            // Auto-generate output path
            string parentDir = Path.GetDirectoryName(sourceFolder.TrimEnd(Path.DirectorySeparatorChar)) ?? sourceFolder;
            string outputPath = Path.Combine(parentDir, $"{dominantType}_Bundle.przm");

            Console.WriteLine($"Auto-selected output path: {outputPath}");

            // Determine DevKey path
            string devKeyPath = Path.ChangeExtension(outputPath, ".przmk");

            if (!File.Exists(devKeyPath))
            {
                string devName = ReadInput("DevKey not found. Enter Developer Name to generate: ");
                if (_exitRequested) return;

                var key = devKeyManager.Generate(devName);
                devKeyManager.SaveKey(key, devKeyPath);
                Console.WriteLine($"✔ New DevKey generated: {devKeyPath}");
            }

            try
            {
                przmBundle.PackFromFolder(sourceFolder, outputPath, devKeyPath);
                Console.WriteLine($"✅ Packing Complete: {outputPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}");
            }

            Console.WriteLine("Press any key to return to main menu...");
            Console.ReadKey();
        }


        // ===========================
        // 📂 UNPACK .przm
        // ===========================
        static void UnpackBundle()
        {
            Console.Clear();
            Console.WriteLine("=== Unpack .przm ===");

            string bundlePath = PromptUntilValid("Enter path to .przm bundle: ",
                path => File.Exists(path) && Path.GetExtension(path) == ".przm");
            if (_exitRequested) return;

            string targetFolder = PromptUntilValid("Enter target folder to extract to: ",
                folder => !string.IsNullOrWhiteSpace(folder));
            if (_exitRequested) return;

            Directory.CreateDirectory(targetFolder);

            string devKeyPath = Path.ChangeExtension(bundlePath, ".przmk");
            if (!File.Exists(devKeyPath))
            {
                Console.WriteLine($"❌ DevKey missing: {devKeyPath}");
                Console.WriteLine("Press any key to return...");
                Console.ReadKey();
                return;
            }

            try
            {
                przmBundle.UnpackToFolder(bundlePath, targetFolder, devKeyPath);
                Console.WriteLine($"✅ Unpacked to: {targetFolder}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}");
            }

            Console.WriteLine("Press any key to return to main menu...");
            Console.ReadKey();
        }

        // ===========================
        // 🔐 DEVKEY SELECTION/GEN
        // ===========================
        static void SelectOrGenerateDevKey()
        {
            Console.Clear();
            Console.WriteLine("=== DevKey Manager ===");

            string inputPath = ReadInput("Enter path to existing DevKey (.przmk) or leave blank to generate new: ");
            if (_exitRequested) return;

            if (string.IsNullOrWhiteSpace(inputPath))
            {
                string devName = ReadInput("Enter Developer Name for new DevKey: ");
                if (_exitRequested) return;

                string fileName = $"devkey_{devName}_{Guid.NewGuid().ToString()[..8]}.przmk";
                string savePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);

                var newKey = devKeyManager.Generate(devName);
                devKeyManager.SaveKey(newKey, savePath);
                Console.WriteLine($"✅ DevKey saved at: {savePath}");
            }
            else if (File.Exists(inputPath))
            {
                try
                {
                    var key = devKeyManager.LoadKey(inputPath);
                    Console.WriteLine($"🔐 Loaded DevKey for: {key?.DevName} (ID: {key?.DevId})");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Failed to load DevKey: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("❌ File not found.");
            }

            Console.WriteLine("Press any key to return to main menu...");
            Console.ReadKey();
        }

        // ===========================
        // ✨ Utility: Input Reader
        // ===========================
        static string ReadInput(string prompt)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine()?.Trim();

            HandleGlobalCommand(input);
            return _exitRequested ? string.Empty : input ?? string.Empty;
        }

        static string PromptUntilValid(string prompt, Func<string, bool> isValid)
        {
            string input;
            do
            {
                input = ReadInput(prompt);
                if (_exitRequested || input == "b") return string.Empty;

            } while (!isValid(input));

            return input;
        }

        static void HandleGlobalCommand(string? input)
        {
            if (string.IsNullOrWhiteSpace(input)) return;

            switch (input.Trim().ToLower())
            {
                case "b":
                    // Back to main menu — just exit current screen
                    break;
                case "x":
                    _exitRequested = true;
                    break;
            }
        }
    }
}
