# Prism(PRZM) Bundle System

[![License](https://img.shields.io/badge/license-PolyForm%20Shield%201.0.0-blue.svg)](https://polyformproject.org/licenses/shield/1.0.0/)

---

## Overview

**przmBundleSystem** is a lightweight, easy-to-integrate C# library designed to create, manage, and use **.przm** asset bundles. These bundles are optimized container files that can hold game content such as textures, models, audio, and other resources in a single compressed archive.

It was originally developed to support the **VIEN** game engine project but is designed with modularity and adaptability in mind, allowing developers to incorporate it seamlessly into existing projects or new software.

The library is designed and developed using **.NET 9.0** for modern performance and compatibility.

---

## Features

- **Create** `.przm` bundles from folders containing game assets.
- **Extract** `.przm` bundles to retrieve individual files.
- **Encrypt** and **decrypt** bundles using DevKeys to secure content.
- Supports multiple compression algorithms for optimized size and speed.
- Command-line utility for packing/unpacking and managing DevKeys.
- Designed for straightforward integration with minimal dependencies.

---

## Included Tool: `przmCMDLineTool`

The project includes a fully functional command-line interface (`przmCMDLineTool`) that allows users to:

1. Pack a folder of assets into a `.przm` bundle.
2. Unpack a `.przm` bundle into a folder.
3. Select or generate developer keys (`DevKeys`) used for encryption.
4. Navigate easily with global commands (`b` for back, `x` for exit) from any prompt.

This tool demonstrates the full capabilities of the library and serves as a practical utility for developers managing `.przm` bundles.

---

## Why Use przmBundleSystem?

- **Custom Bundles:** Unlike general-purpose archive formats, `.przm` bundles are tailored for game development needs.
- **Encryption & Security:** Protect your game assets with DevKey-based encryption.
- **Compression:** Use advanced compression algorithms to reduce bundle sizes without sacrificing speed.
- **Modular & Lightweight:** Minimal dependencies and easy to add to existing C# projects.
- **Open to Extension:** Easily extend compression, encryption, or asset management features.

---

## Getting Started

### Installation

Add the `przmBundleSystem` library to your C# project by including the source or referencing the compiled DLL.

### Dependencies

Make sure to include the following NuGet packages in your project for full functionality:

```xml
<PackageReference Include="K4os.Compression.LZ4" Version="1.3.8" />
<PackageReference Include="Rijndael256" Version="3.2.0" />
<PackageReference Include="System.Text.Json" Version="9.0.5" />
<PackageReference Include="ZstdSharp.Port" Version="0.8.5" />
````

These packages provide compression, encryption, and JSON serialization support.

---

## API Reference

Below are the main classes and methods you will use when working with `przmBundleSystem` programmatically.

### `przmBundle`

This static class provides core functionality to pack and unpack bundles.

| Method                                                                             | Description                                                                                          | Parameters                                                                                                                     | Returns |
| ---------------------------------------------------------------------------------- | ---------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------ | ------- |
| `PackFromFolder(string folderPath, string outputPath, string devKeyPath = null)`   | Packs all files from the specified folder into a `.przm` bundle. Optionally encrypts using a DevKey. | `folderPath` - Folder to pack.<br>`outputPath` - Output bundle file path.<br>`devKeyPath` - Path to DevKey file (optional).    | `void`  |
| `UnpackToFolder(string bundlePath, string outputFolder, string devKeyPath = null)` | Extracts the contents of a `.przm` bundle to a folder. Decrypts if encrypted.                        | `bundlePath` - Path to `.przm` file.<br>`outputFolder` - Destination folder.<br>`devKeyPath` - Path to DevKey file (optional). | `void`  |

---

### `devKeyManager`

Handles developer key creation, saving, loading, and validation.

| Method                             | Description                                              | Parameters                                         | Returns                      |
| ---------------------------------- | -------------------------------------------------------- | -------------------------------------------------- | ---------------------------- |
| `Generate(string developerName)`   | Generates a new DevKey associated with a developer name. | `developerName` - The name for the key owner.      | `DevKey` (custom key object) |
| `SaveKey(DevKey key, string path)` | Saves a DevKey to a file on disk.                        | `key` - The DevKey to save.<br>`path` - File path. | `void`                       |
| `LoadKey(string path)`             | Loads a DevKey from a file.                              | `path` - Path to DevKey file.                      | `DevKey`                     |
| `ValidateKey(DevKey key)`          | Validates a DevKey for use in encryption/decryption.     | `key` - The DevKey to validate.                    | `bool`                       |

---

### Compression Options

By default, LZ4 compression is used, but the system supports others like Zstd via the `ZstdSharp.Port` package. You can customize compression by modifying the compression handler class in the source.

---

## Usage Example

#### Packing a Folder

```csharp
using przmBundleSystem;
using przmBundleSystem.API.Encryption;

// Define source folder and output bundle path
string sourceFolder = @"C:\MyGameAssets";
string outputBundle = @"C:\Bundles\Textures.przm";
string devKeyPath = @"C:\Bundles\Textures.przmk";

// Generate a DevKey if needed
if (!File.Exists(devKeyPath))
{
    var key = devKeyManager.Generate("MyDeveloperName");
    devKeyManager.SaveKey(key, devKeyPath);
}

// Pack the folder into a bundle
przmBundle.PackFromFolder(sourceFolder, outputBundle, devKeyPath);
```

#### Unpacking a Bundle

```csharp
string bundlePath = @"C:\Bundles\Textures.przm";
string extractFolder = @"C:\ExtractedAssets";
string devKeyPath = @"C:\Bundles\Textures.przmk";

przmBundle.UnpackToFolder(bundlePath, extractFolder, devKeyPath);
```

---

## Using the Command-Line Tool

Build and run the `przmCMDLineTool` executable. The CLI provides the following menu:

```
=== PRZM CMD-Line Tool ===
1. Pack New .przm
2. Unpack .przm
3. Select or Generate DevKey
4. Exit
----------------------------
Type 'b' to go back anytime, or 'x' to exit.
```

* Enter the option number to perform the action.
* You can type `b` at any input prompt to return to the main menu.
* Type `x` to exit the tool at any time.

---

## Integration Tips

* The library is framework-agnostic and works with **.NET 9.0** or any compatible .NET Standard projects.
* Bundle files are cross-platform compatible.
* You can extend the compression algorithms by modifying the compression layer.
* DevKeys ensure your content is safe from unauthorized extraction.
* Suitable for game engines, asset pipelines, modding tools, or any scenario requiring compressed, encrypted asset bundles.

---

## License

This project is licensed under the **PolyForm Shield License 1.0.0**.
See the full license here: [https://polyformproject.org/licenses/shield/1.0.0/](https://polyformproject.org/licenses/shield/1.0.0/)

The license is designed to protect the software from commercial exploitation without permission, while allowing free use for non-commercial or internal development purposes.

---

## Contributing

Contributions are welcome! Please fork the repo and submit pull requests. For major changes, open an issue first to discuss your ideas.

---

## Contact

Developed by **Jordan Pryor** for **Studio Prism** as part of the VIEN game engine ecosystem/pipeline.

For questions or support, please open an issue or contact via your preferred method.

---

Thank you for using **przmBundleSystem** — streamlined asset bundling for modern game development!


