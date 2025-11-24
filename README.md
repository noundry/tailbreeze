# Tailbreeze

**A breeze to integrate Tailwind CSS into ASP.NET applications.**

Tailbreeze makes it incredibly easy to use Tailwind CSS in your ASP.NET Razor Pages, MVC, and Blazor applications. No Node.js required, no complex setup - just add two NuGet packages and you're ready to go!

## Inspiration & Attribution

Tailbreeze is inspired by the excellent [tailwind-dotnet](https://github.com/kallebysantos/tailwind-dotnet) project by [Kalleby Santos](https://github.com/kallebysantos). We've built upon those foundations with modern .NET standards, enhanced features, and full support for both Tailwind CSS v3.x and v4.x.

## Features

- **Zero Node.js Dependency**: Automatically downloads and manages the Tailwind CLI
- **Hot Reload**: Automatic CSS regeneration during development with `dotnet watch`
- **Multi-Version Support**: Full support for Tailwind CSS v3.x and v4.x
- **Production Ready**: Automatic minification and optimization for production builds
- **Modern .NET**: Built for .NET 8.0, 9.0, and 10.0 with modern coding standards
- **Blazor & Razor**: Seamless integration with Razor Pages, MVC, Blazor Server, and Blazor WebAssembly
- **MSBuild Integration**: Automatic compilation during build and publish
- **Easy Configuration**: Simple, intuitive API with sensible defaults
- **CDN Fallback**: Optional CDN fallback for development environments

## Quick Start

### Installation

Install both NuGet packages:

```bash
dotnet add package Tailbreeze
dotnet add package Tailbreeze.Build
```

### Configuration

#### For Razor Pages or MVC:

**Program.cs:**
```csharp
using Tailbreeze.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add Tailbreeze
builder.Services.AddTailbreeze(options =>
{
    options.TailwindVersion = "latest"; // or "3", "4", "3.4.1", etc.
});

var app = builder.Build();

// Use Tailbreeze middleware
app.UseTailbreeze();

// ... other middleware

app.Run();
```

**_Layout.cshtml or _ViewImports.cshtml:**
```cshtml
@addTagHelper *, Tailbreeze
```

**In your layout file:**
```html
<head>
    <tailwind-link cdn-fallback="true" />
</head>
```

#### For Blazor:

**Program.cs:**
```csharp
using Tailbreeze.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add Tailbreeze
builder.Services.AddTailbreeze(options =>
{
    options.TailwindVersion = "latest";
});

var app = builder.Build();

// Use Tailbreeze middleware
app.UseTailbreeze();

// ... other middleware

app.Run();
```

**MainLayout.razor or App.razor:**
```razor
@using Tailbreeze.Components

<TailwindLink CdnFallback="true" />
```

### That's It!

Tailbreeze will automatically:
1. Create a default `Styles/app.css` file with Tailwind directives
2. Create a default `tailwind.config.js` with your project structure
3. Download the Tailwind CLI for your platform
4. Start watching for changes in development
5. Compile and minify CSS for production

## Configuration Options

Customize Tailbreeze behavior in your `Program.cs`:

```csharp
builder.Services.AddTailbreeze(options =>
{
    // Tailwind version: "3", "4", "latest", or specific like "3.4.1"
    options.TailwindVersion = "latest";

    // Input CSS file path (relative to project root)
    options.InputCssPath = "Styles/app.css";

    // Output CSS file path (relative to wwwroot)
    options.OutputCssPath = "css/app.css";

    // Tailwind config file path
    options.ConfigPath = "tailwind.config.js";

    // Enable hot reload in development (default: true)
    options.EnableHotReload = true;

    // Minify CSS (default: false in dev, true in production)
    options.MinifyCss = false;

    // Auto-install Tailwind CLI if not found (default: true)
    options.AutoInstallCli = true;

    // CDN fallback if CLI fails (default: true in dev, false in prod)
    options.UseCdnFallback = true;

    // Custom content paths to scan
    options.ContentPaths = new List<string>
    {
        "./Pages/**/*.cshtml",
        "./Views/**/*.cshtml",
        "./Components/**/*.razor"
    };

    // Additional Tailwind CLI arguments
    options.AdditionalArguments = "--verbose";
});
```

## MSBuild Configuration

Customize build behavior in your `.csproj` file:

```xml
<PropertyGroup>
  <!-- Tailwind version -->
  <TailbreezeTailwindVersion>latest</TailbreezeTailwindVersion>

  <!-- Input CSS file -->
  <TailbreezeInputCss>Styles/app.css</TailbreezeInputCss>

  <!-- Output CSS file -->
  <TailbreezeOutputCss>css/app.css</TailbreezeOutputCss>

  <!-- Config file -->
  <TailbreezeConfigPath>tailwind.config.js</TailbreezeConfigPath>

  <!-- Minify during build -->
  <TailbreezeMinify>false</TailbreezeMinify>

  <!-- Minify during publish -->
  <TailbreezeMinifyOnPublish>true</TailbreezeMinifyOnPublish>

  <!-- Auto-install CLI -->
  <TailbreezeAutoInstall>true</TailbreezeAutoInstall>

  <!-- Compile on build -->
  <TailbreezeCompileOnBuild>true</TailbreezeCompileOnBuild>
</PropertyGroup>
```

## Tailwind CSS v3 vs v4

Tailbreeze automatically handles the differences between Tailwind v3 and v4:

### Tailwind CSS v3.x

When using v3, Tailbreeze creates an input CSS file with:
```css
@tailwind base;
@tailwind components;
@tailwind utilities;
```

And a `tailwind.config.js`:
```javascript
/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./Pages/**/*.{cshtml,razor}",
    "./Views/**/*.cshtml",
    "./Components/**/*.{cshtml,razor}"
  ],
  theme: {
    extend: {},
  },
  plugins: [],
}
```

### Tailwind CSS v4.x

When using v4 (or "latest"), Tailbreeze creates:
```css
@import "tailwindcss";
```

And a `tailwind.config.js`:
```javascript
export default {
  content: [
    "./Pages/**/*.{cshtml,razor}",
    "./Views/**/*.cshtml",
    "./Components/**/*.{cshtml,razor}"
  ],
}
```

## How It Works

### Development Mode

1. **Hosted Service**: Tailbreeze runs as an `IHostedService` that starts the Tailwind CLI in watch mode
2. **File Watching**: Monitors your Razor/Blazor files for changes
3. **Auto Regeneration**: Automatically regenerates CSS when classes change
4. **Middleware**: Serves the generated CSS through middleware with no-cache headers

### Production Mode

1. **MSBuild Task**: Compiles Tailwind CSS during `dotnet publish`
2. **Minification**: Automatically minifies output CSS
3. **Optimization**: Purges unused classes for minimal file size
4. **Static Files**: Outputs to wwwroot for static file serving

## Tag Helpers & Components

### Razor Pages / MVC

Use the `<tailwind-link>` tag helper:

```html
<!-- Basic usage -->
<tailwind-link />

<!-- With CDN fallback -->
<tailwind-link cdn-fallback="true" />
```

### Blazor

Use the `<TailwindLink>` component:

```razor
@using Tailbreeze.Components

<!-- Basic usage -->
<TailwindLink />

<!-- With CDN fallback -->
<TailwindLink CdnFallback="true" />
```

## Sample Projects

Check out the `samples` folder for complete examples:

- **Tailbreeze.Samples.RazorPages**: Razor Pages with Tailwind CSS
- **Tailbreeze.Samples.BlazorServer**: Blazor Server with Tailwind CSS
- **Tailbreeze.Samples.BlazorWasm**: Blazor WebAssembly with Tailwind CSS

## Troubleshooting

### Tailwind CLI Not Found

If the CLI fails to download automatically:
1. Check your internet connection
2. Verify proxy settings
3. Manually download from [Tailwind Releases](https://github.com/tailwindlabs/tailwindcss/releases)
4. Place in `%LOCALAPPDATA%/Tailbreeze/cli/[version]/`

### CSS Not Updating

1. Ensure `EnableHotReload` is `true` in development
2. Check that your content paths are correct
3. Verify the Tailwind CLI process is running
4. Try rebuilding the project

### Classes Not Working

1. Verify your content paths in `tailwind.config.js`
2. Check that the file extensions match (`.cshtml`, `.razor`)
3. Ensure the CSS link is in your layout file
4. Clear browser cache

## Performance Tips

1. **Use Specific Content Paths**: Don't scan your entire project - target specific directories
2. **Enable Minification**: Always minify in production
3. **Use JIT Mode**: Tailwind v3+ uses JIT by default for faster builds
4. **Cache CLI**: The CLI is cached locally - no need to re-download

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

MIT License - see LICENSE file for details

## Acknowledgments

- **[Kalleby Santos](https://github.com/kallebysantos)** for the original [tailwind-dotnet](https://github.com/kallebysantos/tailwind-dotnet) project
- **[Tailwind Labs](https://tailwindlabs.com/)** for Tailwind CSS
- The .NET community for feedback and support

## Support

- **Issues**: [GitHub Issues](https://github.com/yourusername/tailbreeze/issues)
- **Discussions**: [GitHub Discussions](https://github.com/yourusername/tailbreeze/discussions)
- **Documentation**: [Full Documentation](https://github.com/yourusername/tailbreeze/wiki)

---

Made with ❤️ for the .NET community
