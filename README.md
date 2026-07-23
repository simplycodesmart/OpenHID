# OpenHID — HID Engineering Toolkit

<div align="center">

![OpenHID](https://img.shields.io/badge/OpenHID-v1.0.0-00E5FF?style=for-the-badge)
![Build](https://img.shields.io/badge/build-passing-6BFF8F?style=for-the-badge)
![Tests](https://img.shields.io/badge/tests-12/12-6BFF8F?style=for-the-badge)
![License](https://img.shields.io/badge/license-MIT-DDB7FF?style=for-the-badge)
![Platform](https://img.shields.io/badge/platform-Windows%20|%20Linux%20|%20macOS-6BFF8F?style=for-the-badge)

</div>

---

## The Problem

If you've ever worked with HID (Human Interface Devices) — keyboards, mice, game controllers, barcode scanners, or any custom USB HID hardware — you know the pain:

| Task | Tool Needed |
|------|------------|
| List connected devices | USB Device Tree Viewer or USBView |
| Inspect VID/PID/serial | Multiple property inspectors |
| Read/send HID reports | Custom HIDAPI code or vendor utilities |
| Decode report descriptors | Specialized descriptor parsers |
| Monitor live traffic | Wireshark + USBPcap (complex setup) |
| Reverse-engineer unknown devices | Manual byte-by-byte analysis |
| Log/replay packets | Custom scripts |
| Test firmware changes | Manual testing with multiple tools |
| Generate documentation | Copy-paste everything into docs |

**OpenHID solves all of this in a single application.**

---

## What OpenHID Does

OpenHID is a **complete HID engineering workstation** that replaces 10+ separate tools with one unified desktop application. It lets you:

1. **Discover** every HID device connected to your computer
2. **Inspect** full device properties (VID, PID, manufacturer, serial, paths)
3. **Connect** safely and manage multiple open sessions
4. **Explore** HID report descriptors — see every collection, usage, button, and value
5. **Communicate** — send output reports, read input reports, read/write feature reports
6. **Monitor** live traffic in real-time with filters
7. **Decode** raw hex bytes into meaningful field-by-field data
8. **Detect** vendor-specific protocols (Logitech HID++, more coming)
9. **Log & export** all communication to JSON, CSV, Markdown, or PCAP
10. **Automate** testing with scripted command sequences
11. **Reverse-engineer** unknown devices with pattern detection and bit analysis
12. **Generate** professional device documentation in MD, HTML, or JSON
13. **Extend** via plugins
14. **Integrate** via the SDK

---

## Who Is This For?

| Role | Benefit |
|------|---------|
| **Firmware Engineers** | Test HID firmware without writing custom host code. Monitor reports in real-time. Decode descriptors instantly. |
| **Hardware Engineers** | Verify hardware behavior. Validate report descriptors against specifications. |
| **QA Engineers** | Automate regression testing with scripts. Log and replay traffic. Document device behavior. |
| **Reverse Engineers** | Understand unknown HID devices. Detect patterns in report data. Guess protocols automatically. |
| **Game Developers** | Debug controller inputs. Decode gamepad reports. Test custom HID peripherals. |
| **IoT Developers** | Interface with custom HID sensors and actuators. Prototype quickly. |
| **Students & Educators** | Learn HID protocol internals with a visual interface. Experiment safely. |

---

## Architecture

```
OpenHID.slnx
├── src/
│   ├── OpenHardwarePlatform.Abstractions/     — Interfaces (IDeviceManager, IDeviceTransport, etc.)
│   ├── OpenHardwarePlatform.Core/             — Core engine (DeviceManager, hot-plug, plugin loader)
│   ├── OpenHardwarePlatform.Transports.Hid/   — HID transport via HidSharp
│   ├── OpenHardwarePlatform.Protocols/        — Protocol decoders (Standard HID, Logitech HID++)
│   ├── OpenHardwarePlatform.Desktop/          — Avalonia UI desktop application
│   ├── OpenHardwarePlatform.Cli/              — Command-line scanner
│   └── OpenHID.SDK/                           — High-level SDK for external consumers
└── tests/
    ├── OpenHardwarePlatform.Core.Tests/       — 10 unit tests
    ├── OpenHardwarePlatform.Desktop.Tests/    — 1 unit test
    └── OpenHardwarePlatform.Transports.Hid.Tests/ — 1 unit test
```

---

## UI Preview

The application features a **modern dark-themed interface** with:

- **11 navigation modules** with icons and descriptions
- **Device cards** with VID/PID badges, hover glow effects
- **Glassmorphism cards** with subtle borders
- **Monospace font** (JetBrains Mono) for hex/technical data
- **Cyan/cyan accent color scheme** (accessibility-friendly)
- **Real-time status bar** with device count and connection state
- **Responsive layout** — resizable window down to 1100x720

---

## How to Use

### Quick Start

```bash
# Clone the repository
git clone https://github.com/yourusername/OpenHID.git
cd OpenHID

# Build and run the desktop app
dotnet run --project src/OpenHardwarePlatform.Desktop

# Or run the CLI scanner
dotnet run --project src/OpenHardwarePlatform.Cli
```

### Navigation Guide

| Icon | Module | What You Can Do |
|------|--------|-----------------|
| 🔍 | Device Explorer | Browse all connected HID devices. Search by name, VID, PID. Sort by any field. |
| 📋 | Device Inspector | View every property of the selected device. |
| 🔌 | Connection Manager | Open/close communication sessions. Monitor active connections. |
| 📄 | Descriptor Explorer | Parse HID report descriptors. See raw hex and parsed items. |
| ⌨️ | HID Console | Send output reports. Read input reports. Write/read feature reports. |
| 📡 | Live Monitor | Watch incoming reports in real-time. Filter by direction or data. |
| 🔢 | Packet Decoder | Paste hex bytes. Get byte-by-byte analysis with hex, binary, decimal, and interpretation. |
| 📝 | Packet Logger | Record all traffic. Export to JSON, CSV, or Markdown. |
| ⚡ | Automation | Write scripts (OUT, WAIT, FEATURE commands). Set repeat count and delay. |
| 🔬 | Reverse Engineering | Analyze unknown report data. Detect patterns, checksums, and guess protocols. |
| 📚 | Documentation | Generate device documentation in MD, HTML, or JSON format. |

---

## Building and Publishing

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) or later
- A C# IDE (Visual Studio 2022+, Rider, or VS Code)
- Windows, Linux, or macOS

### Development Build

```bash
# Full solution build
dotnet build

# Run all tests
dotnet test

# Run specific test project
dotnet test tests/OpenHardwarePlatform.Core.Tests
```

### Publish as Standalone EXE

```bash
# Windows 64-bit (single file, trimmed)
dotnet publish src/OpenHardwarePlatform.Desktop \
  -c Release \
  -r win-x64 \
  --self-contained true \
  -p:PublishSingleFile=true \
  -p:IncludeNativeLibrariesForSelfExtract=true \
  -o ./publish/win-x64

# Linux 64-bit
dotnet publish src/OpenHardwarePlatform.Desktop \
  -c Release \
  -r linux-x64 \
  --self-contained true \
  -p:PublishSingleFile=true \
  -o ./publish/linux-x64

# macOS (Apple Silicon)
dotnet publish src/OpenHardwarePlatform.Desktop \
  -c Release \
  -r osx-arm64 \
  --self-contained true \
  -p:PublishSingleFile=true \
  -o ./publish/osx-arm64
```

The published EXE will be in `./publish/` and is immediately runnable on the target platform. No additional runtime installation required.

### Cross-Platform Notes

- **Windows**: Full support via HidSharp (WinUSB/HID API)
- **Linux**: Requires `libudev` (install via `sudo apt install libudev-dev`)
- **macOS**: Uses IOKit through HidSharp; no additional setup needed

---

## CLI Usage

```bash
dotnet run --project src/OpenHardwarePlatform.Cli
```

Output example:
```
OpenHID CLI - Device Scanner
============================

Scanning for HID devices...

Device #1
  Transport : HID
  VID       : 0x046D
  PID       : 0xC52B
  Product   : USB Receiver
  Manufacturer : Logitech
  Serial    : (none)
  Path      : \\?\hid#vid_046d&pid_c52b#7&...

Total: 3 device(s) found.
```

---

## SDK Usage (for Developers)

The `OpenHID.SDK` package lets you integrate HID functionality into your own applications:

```csharp
using OpenHID.SDK;

// Create the client
await using var client = new OpenHidClient();

// List all devices
var devices = await client.ListDevicesAsync();

// Connect to a device
var connection = await client.ConnectAsync(devices[0]);

// Decode a report
var decoded = client.DecodeReport(0x046D, 0xC52B, 
    new byte[] { 0x01, 0x00, 0x08, 0xFF });

Console.WriteLine(decoded.Summary); // "Logitech HID++ report"
```

---

## Extending with Plugins

OpenHID supports a plugin system. Create a class library that implements `IPlugin`:

```csharp
using OpenHardwarePlatform.Abstractions;

public class MyAnalyzerPlugin : IPlugin
{
    public string Id => "com.example.myanalyzer";
    public string Name => "My Analyzer";
    public Version Version => new(1, 0, 0);
    public string Description => "Custom device analyzer";

    public Task InitializeAsync(IPluginContext context, CancellationToken ct)
    {
        context.Log.Info("Plugin initialized!");
        // Access all devices via context.DeviceManager
        return Task.CompletedTask;
    }

    public Task ShutdownAsync(CancellationToken ct) => Task.CompletedTask;
}
```

Build the DLL and place it in the `plugins/` directory next to the executable.

---

## Contribution Guidelines

We welcome contributions! Here's how to get started:

### Getting Started

1. **Fork** the repository
2. **Clone** your fork: `git clone https://github.com/yourusername/OpenHID.git`
3. **Create a branch**: `git checkout -b feature/your-feature-name`
4. **Build**: `dotnet build`
5. **Make your changes**
6. **Run tests**: `dotnet test` (all tests must pass)
7. **Commit**: `git commit -m "Description of your changes"`
8. **Push**: `git push origin feature/your-feature-name`
9. **Open a Pull Request**

### Code Style

- Follow existing patterns (CommunityToolkit.Mvvm for ViewModels)
- Use file-scoped namespaces
- Keep `OpenHardwarePlatform.Core` independent of UI
- Platform-specific code goes in `OpenHardwarePlatform.Transports.*`
- XML documentation comments required on public APIs
- Unit tests required for all new features

### Development Workflow

1. Build **one module at a time** following the dependency graph
2. Never generate code for a module until all its dependencies are complete
3. Every module must compile independently before starting the next
4. Every feature requires: **unit tests + documentation + working UI**
5. Favor reusable services over direct UI logic
6. Keep platform-specific code isolated

### Module Dependency Graph

```
Foundation → Device Explorer → Device Inspector → Connection Manager
→ Descriptor Explorer → HID Console → Live Monitor → Packet Decoder
→ Protocol Library → Packet Logger & Automation → Reverse Engineering
→ Documentation Generator → Plugin System → SDK
```

### Reporting Issues

- Bug reports: Include OS, .NET version, device VID/PID, and steps to reproduce
- Feature requests: Describe the use case and expected behavior
- Security issues: Email the maintainers directly

---

## Roadmap

| Feature | Status |
|---------|--------|
| Device Explorer (search, filter, sort, hot-plug) | ✅ Complete |
| Device Inspector (all properties) | ✅ Complete |
| Connection Manager (multiple sessions) | ✅ Complete |
| Descriptor Explorer (item parsing) | ✅ Complete |
| HID Console (output/input/feature reports) | ✅ Complete |
| Live Monitor (real-time, filters, auto-scroll) | ✅ Complete |
| Packet Decoder (byte-by-byte analysis) | ✅ Complete |
| Protocol Library (extensible decoders) | ✅ Complete |
| Packet Logger (JSON/CSV/MD export) | ✅ Complete |
| Automation (scripting, replay, assertions) | ✅ Complete |
| Reverse Engineering (pattern detection, bit analysis) | ✅ Complete |
| Documentation Generator (MD/HTML/JSON) | ✅ Complete |
| Plugin System | ✅ Implemented |
| SDK | ✅ Implemented |
| PCAP export | 🔜 Planned |
| Wireshark integration | 🔜 Planned |
| Xbox/DualSense protocol decoders | 🔜 Planned |
| USB transport support | 🔜 Planned |
| Bluetooth transport support | 🔜 Planned |

---

## Comparison to Existing Tools

| Feature | OpenHID | USBView | USB Tree Viewer | HIDAPI | Wireshark |
|---------|---------|---------|----------------|--------|-----------|
| Device enumeration | ✅ | ✅ | ✅ | ✅ | ❌ |
| Property inspection | ✅ | ✅ | ✅ | ❌ | ❌ |
| HID descriptor parse | ✅ | ❌ | ❌ | ❌ | ❌ |
| Send output reports | ✅ | ❌ | ❌ | ✅ | ❌ |
| Read input reports | ✅ | ❌ | ❌ | ✅ | ✅ (capture) |
| Feature reports | ✅ | ❌ | ❌ | ✅ | ❌ |
| Live monitoring | ✅ | ❌ | ❌ | ❌ | ✅ |
| Protocol decoding | ✅ | ❌ | ❌ | ❌ | Partial |
| Packet logging | ✅ | ❌ | ❌ | ❌ | ✅ |
| Export (JSON/CSV/MD) | ✅ | ❌ | ❌ | ❌ | ✅ (PCAP) |
| Automation scripts | ✅ | ❌ | ❌ | ❌ | ❌ |
| Reverse engineering | ✅ | ❌ | ❌ | ❌ | Partial |
| Documentation gen | ✅ | ❌ | ❌ | ❌ | ❌ |
| Plugin system | ✅ | ❌ | ❌ | ❌ | ❌ |
| SDK | ✅ | ❌ | ❌ | ❌ | ❌ |
| Hot-plug detection | ✅ | ❌ | ❌ | ❌ | ❌ |
| Cross-platform | ✅ | ❌ | ❌ | ✅ | ✅ |

---

## License

MIT License — see [LICENSE](LICENSE) for details.

---

## Acknowledgments

- [HidSharp](https://github.com/jdrews/HidSharp) — Cross-platform HID library
- [Avalonia UI](https://avaloniaui.net/) — Cross-platform UI framework
- [CommunityToolkit.Mvvm](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/) — MVVM infrastructure
- .NET Team — For the amazing .NET platform

---

<div align="center">
  
**OpenHID — One tool to discover, inspect, communicate, decode, automate, and document every HID device.**

⭐ Star us on GitHub! 🐛 Report issues 💡 Suggest features 🤝 Contribute code

</div>