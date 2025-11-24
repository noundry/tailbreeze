# Tailbreeze - Quick Start Guide

[![NuGet](https://img.shields.io/nuget/v/Tailbreeze.svg)](https://www.nuget.org/packages/Tailbreeze/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

**A breeze to integrate Tailwind CSS into ASP.NET applications** - No Node.js required!

## Features

- ✅ Zero Node.js dependency
- ✅ Tailwind CSS v3.x and v4.x support
- ✅ .NET 8.0, 9.0, and 10.0 support
- ✅ Hot reload during development
- ✅ Automatic minification for production
- ✅ Works with Razor Pages, MVC, Blazor Server, and Blazor WebAssembly
- ✅ MSBuild integration for build-time compilation

## Installation

```bash
dotnet add package Tailbreeze
dotnet add package Tailbreeze.Build
```

## Quick Setup

### 1. Add Services (Program.cs)

```csharp
using Tailbreeze.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add Tailbreeze
builder.Services.AddTailbreeze();

var app = builder.Build();

// Use Tailbreeze middleware
app.UseTailbreeze();

app.Run();
```

### 2. Add CSS Link

**For Razor Pages/MVC** (`_ViewImports.cshtml`):
```cshtml
@addTagHelper *, Tailbreeze
```

Then in `_Layout.cshtml`:
```html
<head>
    <tailwind-link />
</head>
```

**For Blazor** (`_Imports.razor`):
```razor
@using Tailbreeze.Components
```

Then in `MainLayout.razor`:
```razor
<HeadContent>
    <TailwindLink />
</HeadContent>
```

### 3. Start Using Tailwind!

```html
<div class="container mx-auto px-4">
    <h1 class="text-4xl font-bold text-blue-600">
        Hello Tailwind!
    </h1>
    <button class="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded">
        Click Me
    </button>
</div>
```

## What Happens Automatically

- Creates `Styles/app.css` with Tailwind directives
- Creates `tailwind.config.js` with your project structure
- Downloads Tailwind CLI (first time only, cached locally)
- Starts watch mode in development for hot reload
- Compiles and minifies CSS for production builds

## Configuration (Optional)

Customize in `Program.cs`:

```csharp
builder.Services.AddTailbreeze(options =>
{
    options.TailwindVersion = "latest"; // or "3", "4", "3.4.1"
    options.InputCssPath = "Styles/app.css";
    options.OutputCssPath = "css/app.css";
    options.EnableHotReload = true;
});
```

Or in your `.csproj`:

```xml
<PropertyGroup>
  <TailbreezeTailwindVersion>latest</TailbreezeTailwindVersion>
  <TailbreezeMinifyOnPublish>true</TailbreezeMinifyOnPublish>
</PropertyGroup>
```

## Documentation

- 📖 [Full Documentation](https://github.com/yourusername/tailbreeze)
- 🚀 [Getting Started Guide](https://github.com/yourusername/tailbreeze/blob/main/docs/GETTING-STARTED.md)
- 📚 [API Reference](https://github.com/yourusername/tailbreeze/blob/main/docs/API-REFERENCE.md)
- 💡 [Sample Projects](https://github.com/yourusername/tailbreeze/tree/main/samples)

## Support

- **Issues**: [GitHub Issues](https://github.com/yourusername/tailbreeze/issues)
- **Discussions**: [GitHub Discussions](https://github.com/yourusername/tailbreeze/discussions)

## Attribution

Inspired by [tailwind-dotnet](https://github.com/kallebysantos/tailwind-dotnet) by Kalleby Santos.

## License

MIT License - See [LICENSE](https://github.com/yourusername/tailbreeze/blob/main/LICENSE)

---

Made with ❤️ for the .NET community
