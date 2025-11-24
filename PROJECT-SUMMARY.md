# Tailbreeze - Project Summary

## Overview

Tailbreeze is a modern .NET library for integrating Tailwind CSS into ASP.NET applications with zero Node.js dependencies. It provides seamless integration for Razor Pages, MVC, Blazor Server, and Blazor WebAssembly projects.

**Inspired by:** [tailwind-dotnet](https://github.com/kallebysantos/tailwind-dotnet) by Kalleby Santos

## Key Features

1. **Zero Dependencies**: No Node.js or npm required - uses standalone Tailwind CLI
2. **Multi-Version Support**: Full support for Tailwind CSS v3.x and v4.x
3. **Multi-Framework**: Works with .NET 8.0, 9.0, and 10.0
4. **Hot Reload**: Automatic CSS regeneration during development
5. **MSBuild Integration**: Compile CSS during build and publish
6. **Production Ready**: Automatic minification and optimization
7. **Easy Setup**: Two NuGet packages, minimal configuration
8. **Modern Standards**: Built with modern .NET coding practices

## Project Structure

```
Tailbreeze/
├── src/
│   ├── Tailbreeze/                          # Core library
│   │   ├── TailbreezeOptions.cs            # Configuration options
│   │   ├── TailwindVersion.cs              # Version management
│   │   ├── Services/
│   │   │   ├── ITailwindCliService.cs      # CLI service interface
│   │   │   ├── TailwindCliService.cs       # CLI implementation
│   │   │   └── TailwindHostedService.cs    # Hosted service for watch mode
│   │   ├── Middleware/
│   │   │   └── TailwindMiddleware.cs       # CSS serving middleware
│   │   ├── TagHelpers/
│   │   │   └── TailwindLinkTagHelper.cs    # Razor tag helper
│   │   ├── Components/
│   │   │   └── TailwindLink.razor          # Blazor component
│   │   └── Extensions/
│   │       ├── ServiceCollectionExtensions.cs
│   │       └── ApplicationBuilderExtensions.cs
│   │
│   └── Tailbreeze.Build/                    # MSBuild integration
│       ├── Tasks/
│       │   └── CompileTailwindTask.cs      # MSBuild compilation task
│       ├── build/
│       │   ├── Tailbreeze.Build.props      # MSBuild properties
│       │   └── Tailbreeze.Build.targets    # MSBuild targets
│       └── buildMultiTargeting/            # Multi-targeting support
│
├── samples/
│   ├── Tailbreeze.Samples.RazorPages/      # Razor Pages sample
│   ├── Tailbreeze.Samples.BlazorServer/    # Blazor Server sample
│   └── Tailbreeze.Samples.BlazorWasm/      # Blazor WebAssembly sample
│
├── docs/
│   ├── GETTING-STARTED.md                  # Quick start guide
│   ├── API-REFERENCE.md                    # Complete API docs
│   └── CONFIGURATION.md                    # Configuration guide
│
├── README.md                               # Main documentation
├── LICENSE                                 # MIT License with attribution
├── CHANGELOG.md                            # Version history
├── CONTRIBUTING.md                         # Contribution guidelines
├── Directory.Build.props                   # Shared MSBuild properties
├── Tailbreeze.sln                         # Solution file
├── build.ps1                              # PowerShell build script
└── build.sh                               # Bash build script
```

## Components

### Core Library (Tailbreeze)

**Services:**
- `ITailwindCliService` / `TailwindCliService`: Manages Tailwind CLI installation and execution
- `TailwindHostedService`: Runs Tailwind CLI in watch mode during development

**Middleware:**
- `TailwindMiddleware`: Serves generated CSS files with proper caching headers

**Configuration:**
- `TailbreezeOptions`: Runtime configuration options
- `TailwindVersion`: Version parsing and management

**Tag Helpers & Components:**
- `TailwindLinkTagHelper`: Razor tag helper for `<tailwind-link>`
- `TailwindLink.razor`: Blazor component for CSS inclusion

**Extensions:**
- `AddTailbreeze()`: Service registration
- `UseTailbreeze()`: Middleware registration

### Build Library (Tailbreeze.Build)

**MSBuild Tasks:**
- `CompileTailwindTask`: Compiles CSS during build/publish

**MSBuild Integration:**
- Automatic compilation on build
- Automatic minification on publish
- Configurable via `.csproj` properties

## How It Works

### Development Mode

1. **Startup**: `TailwindHostedService` starts when the application runs
2. **CLI Installation**: Downloads Tailwind CLI if not present (cached locally)
3. **Config Generation**: Creates default `tailwind.config.js` and `app.css` if missing
4. **Watch Mode**: Starts CLI in watch mode to monitor file changes
5. **Middleware**: Serves generated CSS at configured route
6. **Hot Reload**: CSS regenerates automatically when classes change

### Production Mode

1. **Build Time**: MSBuild task compiles CSS during `dotnet build`
2. **Publish Time**: CSS is compiled and minified during `dotnet publish`
3. **Optimization**: Unused classes are purged for minimal file size
4. **Static Files**: Optimized CSS is included in published output

## Technology Stack

- **Language**: C# 12 with modern features
- **Framework**: .NET 8.0, 9.0, 10.0
- **Build System**: MSBuild
- **Tailwind CSS**: v3.x and v4.x support
- **Architecture**:
  - Hosted Services for background tasks
  - Middleware for request processing
  - MSBuild tasks for build-time compilation

## Key Design Decisions

### 1. Standalone CLI Approach
Uses the official Tailwind CLI standalone binary instead of requiring Node.js, making deployment and development simpler.

### 2. Multi-Targeting
Supports .NET 8.0, 9.0, and 10.0 to provide broad compatibility while using modern .NET features.

### 3. Automatic Setup
Automatically creates configuration files and downloads CLI to minimize manual setup steps.

### 4. Dual Mode Operation
Separate behavior for development (watch mode with hot reload) and production (build-time compilation with minification).

### 5. Platform Independence
Works on Windows, macOS, and Linux with automatic platform detection for CLI downloads.

## Improvements Over tailwind-dotnet

1. **Modern Architecture**: Uses IHostedService, modern async patterns, and nullable reference types
2. **Enhanced Configuration**: More flexible options with sensible defaults
3. **Better Error Handling**: Graceful fallbacks and detailed logging
4. **v4 Support**: Full support for Tailwind CSS v4.x with automatic config generation
5. **Blazor Components**: Dedicated Blazor components for better DX
6. **Comprehensive Docs**: Extensive documentation with API reference
7. **Build Scripts**: Included build scripts for easy packaging
8. **Sample Projects**: Multiple complete sample projects

## Attribution

This project is inspired by [tailwind-dotnet](https://github.com/kallebysantos/tailwind-dotnet) by Kalleby Santos. We've built upon those excellent foundations with:
- Modern .NET standards and patterns
- Enhanced feature set
- Full Tailwind CSS v4.x support
- Improved developer experience
- Comprehensive documentation

Attribution is maintained in:
- LICENSE file (copyright notice)
- README.md (acknowledgments section)
- NuGet package metadata (description and copyright)
- CHANGELOG.md (inspired by section)

## Building the Project

### Using PowerShell (Windows)
```powershell
.\build.ps1 -Pack
```

### Using Bash (macOS/Linux)
```bash
./build.sh -p
```

### Manual Build
```bash
dotnet restore
dotnet build -c Release
dotnet pack -c Release -o ./artifacts/packages
```

## Publishing to NuGet

1. Build packages: `.\build.ps1 -Pack`
2. Test packages locally in sample projects
3. Publish to NuGet:
   ```bash
   dotnet nuget push ./artifacts/packages/Tailbreeze.1.0.0.nupkg --api-key YOUR_API_KEY --source https://api.nuget.org/v3/index.json
   dotnet nuget push ./artifacts/packages/Tailbreeze.Build.1.0.0.nupkg --api-key YOUR_API_KEY --source https://api.nuget.org/v3/index.json
   ```

## Testing the Samples

### Razor Pages Sample
```bash
cd samples/Tailbreeze.Samples.RazorPages
dotnet watch run
```

### Blazor Server Sample
```bash
cd samples/Tailbreeze.Samples.BlazorServer
dotnet watch run
```

### Blazor WebAssembly Sample
```bash
cd samples/Tailbreeze.Samples.BlazorWasm
dotnet watch run
```

## Next Steps

### For Users
1. Install packages from NuGet
2. Follow the [Getting Started Guide](docs/GETTING-STARTED.md)
3. Check out [sample projects](samples/)
4. Read the [API Reference](docs/API-REFERENCE.md)

### For Contributors
1. Read [CONTRIBUTING.md](CONTRIBUTING.md)
2. Set up development environment
3. Make your changes
4. Submit a pull request

## Support

- **GitHub Issues**: For bugs and feature requests
- **GitHub Discussions**: For questions and community support
- **Documentation**: Comprehensive docs in `docs/` folder

## License

MIT License - See [LICENSE](LICENSE) file for details

---

**Made with ❤️ for the .NET community**

**Special thanks to Kalleby Santos for the original tailwind-dotnet project!**
