# API Reference

Complete API documentation for Tailbreeze.

## Table of Contents

- [Configuration](#configuration)
  - [TailbreezeOptions](#tailbreezeoptions)
  - [MSBuild Properties](#msbuild-properties)
- [Extension Methods](#extension-methods)
- [Tag Helpers](#tag-helpers)
- [Blazor Components](#blazor-components)
- [Services](#services)

---

## Configuration

### TailbreezeOptions

Configuration class for Tailbreeze runtime behavior.

#### Properties

##### `TailwindVersion`
```csharp
public string TailwindVersion { get; set; } = "latest";
```
Specifies the Tailwind CSS version to use.
- **Values**: `"3"`, `"4"`, `"latest"`, or specific versions like `"3.4.1"`
- **Default**: `"latest"` (currently v4)

##### `InputCssPath`
```csharp
public string InputCssPath { get; set; } = "Styles/app.css";
```
Path to the input CSS file relative to project root.
- **Default**: `"Styles/app.css"`

##### `OutputCssPath`
```csharp
public string OutputCssPath { get; set; } = "css/app.css";
```
Path to the output CSS file relative to `wwwroot`.
- **Default**: `"css/app.css"`

##### `ConfigPath`
```csharp
public string ConfigPath { get; set; } = "tailwind.config.js";
```
Path to the Tailwind configuration file relative to project root.
- **Default**: `"tailwind.config.js"`

##### `EnableHotReload`
```csharp
public bool EnableHotReload { get; set; } = true;
```
Enables hot reload during development.
- **Default**: `true`

##### `MinifyCss`
```csharp
public bool? MinifyCss { get; set; }
```
Whether to minify CSS output.
- **Default**: `null` (false in development, true in production)

##### `AutoInstallCli`
```csharp
public bool AutoInstallCli { get; set; } = true;
```
Automatically downloads and installs Tailwind CLI if not found.
- **Default**: `true`

##### `ContentPaths`
```csharp
public List<string> ContentPaths { get; set; } = [];
```
Content paths to scan for Tailwind classes.
- **Default**: Empty (auto-detected based on project structure)
- **Example**:
  ```csharp
  options.ContentPaths = new List<string>
  {
      "./Pages/**/*.cshtml",
      "./Views/**/*.cshtml",
      "./Components/**/*.razor"
  };
  ```

##### `AdditionalArguments`
```csharp
public string? AdditionalArguments { get; set; }
```
Additional arguments to pass to Tailwind CLI.
- **Default**: `null`
- **Example**: `"--verbose"`, `"--watch --poll"`

##### `UseCdnFallback`
```csharp
public bool? UseCdnFallback { get; set; }
```
Uses CDN fallback if CLI installation fails.
- **Default**: `null` (true in development, false in production)

##### `ServePath`
```csharp
public string ServePath { get; set; } = "/tailbreeze/app.css";
```
Route path for serving generated CSS.
- **Default**: `"/tailbreeze/app.css"`

##### `ServeViaMiddleware`
```csharp
public bool ServeViaMiddleware { get; set; } = true;
```
Whether to serve CSS through middleware.
- **Default**: `true`

#### Usage Example

```csharp
builder.Services.AddTailbreeze(options =>
{
    options.TailwindVersion = "4";
    options.InputCssPath = "Styles/tailwind.css";
    options.OutputCssPath = "css/styles.css";
    options.EnableHotReload = true;
    options.MinifyCss = false;
    options.ContentPaths = new List<string>
    {
        "./Pages/**/*.cshtml",
        "./Components/**/*.razor"
    };
});
```

---

### MSBuild Properties

Configuration properties for MSBuild integration (use in `.csproj` file).

#### Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `TailbreezeTailwindVersion` | string | `"latest"` | Tailwind CSS version |
| `TailbreezeInputCss` | string | `"Styles/app.css"` | Input CSS file path |
| `TailbreezeOutputCss` | string | `"css/app.css"` | Output CSS file path |
| `TailbreezeConfigPath` | string | `"tailwind.config.js"` | Config file path |
| `TailbreezeMinify` | bool | `false` | Minify during build |
| `TailbreezeMinifyOnPublish` | bool | `true` | Minify during publish |
| `TailbreezeAutoInstall` | bool | `true` | Auto-install CLI |
| `TailbreezeAdditionalArgs` | string | `""` | Additional CLI arguments |
| `TailbreezeCompileOnBuild` | bool | `true` | Compile on build |

#### Usage Example

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>

    <!-- Tailbreeze Configuration -->
    <TailbreezeTailwindVersion>4</TailbreezeTailwindVersion>
    <TailbreezeInputCss>Styles/app.css</TailbreezeInputCss>
    <TailbreezeOutputCss>css/app.css</TailbreezeOutputCss>
    <TailbreezeMinifyOnPublish>true</TailbreezeMinifyOnPublish>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Tailbreeze" Version="1.0.0" />
    <PackageReference Include="Tailbreeze.Build" Version="1.0.0" />
  </ItemGroup>
</Project>
```

---

## Extension Methods

### ServiceCollectionExtensions

#### `AddTailbreeze()`

Adds Tailbreeze services with default configuration.

```csharp
public static IServiceCollection AddTailbreeze(
    this IServiceCollection services)
```

**Example:**
```csharp
builder.Services.AddTailbreeze();
```

#### `AddTailbreeze(Action<TailbreezeOptions>)`

Adds Tailbreeze services with custom configuration.

```csharp
public static IServiceCollection AddTailbreeze(
    this IServiceCollection services,
    Action<TailbreezeOptions> configure)
```

**Parameters:**
- `configure`: Configuration action

**Example:**
```csharp
builder.Services.AddTailbreeze(options =>
{
    options.TailwindVersion = "4";
    options.EnableHotReload = true;
});
```

### ApplicationBuilderExtensions

#### `UseTailbreeze()`

Adds Tailwind CSS middleware to the application pipeline.

```csharp
public static IApplicationBuilder UseTailbreeze(
    this IApplicationBuilder app)
```

**Example:**
```csharp
app.UseTailbreeze();
```

**Important**: Add before `UseStaticFiles()` to serve CSS properly.

---

## Tag Helpers

### TailwindLinkTagHelper

Tag helper for adding Tailwind CSS link tag in Razor Pages/MVC.

#### Element

```html
<tailwind-link />
```

#### Attributes

##### `cdn-fallback`
```html
<tailwind-link cdn-fallback="true" />
```
Enables CDN fallback if CSS file fails to load.
- **Type**: `bool`
- **Default**: `false`

#### Usage

**_ViewImports.cshtml:**
```cshtml
@addTagHelper *, Tailbreeze
```

**_Layout.cshtml:**
```html
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>@ViewData["Title"]</title>

    <tailwind-link cdn-fallback="true" />
</head>
```

#### Output

```html
<link rel="stylesheet" href="/tailbreeze/app.css" data-tailbreeze="true" />
<script>
  // Tailbreeze CDN fallback
  (function() {
    var link = document.querySelector('link[data-tailbreeze]');
    if (link) {
      link.onerror = function() {
        console.warn('Tailbreeze: Loading Tailwind CSS from CDN fallback');
        var cdn = document.createElement('script');
        cdn.src = 'https://cdn.tailwindcss.com';
        document.head.appendChild(cdn);
      };
    }
  })();
</script>
```

---

## Blazor Components

### TailwindLink

Blazor component for adding Tailwind CSS to your application.

#### Syntax

```razor
<TailwindLink />
```

#### Parameters

##### `CdnFallback`
```razor
<TailwindLink CdnFallback="true" />
```
Enables CDN fallback if CSS file fails to load.
- **Type**: `bool`
- **Default**: `true`

#### Usage

**_Imports.razor:**
```razor
@using Tailbreeze.Components
```

**MainLayout.razor:**
```razor
@inherits LayoutComponentBase

<TailwindLink CdnFallback="true" />

<div class="page">
    <div class="sidebar">
        <NavMenu />
    </div>

    <main>
        @Body
    </main>
</div>
```

---

## Services

### ITailwindCliService

Service interface for managing Tailwind CLI.

#### Methods

##### `EnsureCliInstalledAsync`
```csharp
Task<string> EnsureCliInstalledAsync(
    TailwindVersion version,
    CancellationToken cancellationToken = default)
```
Ensures Tailwind CLI is installed and returns the path.

##### `IsCliInstalledAsync`
```csharp
Task<bool> IsCliInstalledAsync(
    TailwindVersion version,
    CancellationToken cancellationToken = default)
```
Checks if CLI is installed for the specified version.

##### `GetCliPath`
```csharp
string GetCliPath(TailwindVersion version)
```
Gets the path to the installed CLI executable.

##### `ExecuteCliAsync`
```csharp
Task<int> ExecuteCliAsync(
    TailwindVersion version,
    string arguments,
    CancellationToken cancellationToken = default)
```
Executes the Tailwind CLI with specified arguments.

##### `StartWatchModeAsync`
```csharp
Task<IDisposable> StartWatchModeAsync(
    TailwindVersion version,
    string inputPath,
    string outputPath,
    string? configPath = null,
    CancellationToken cancellationToken = default)
```
Starts Tailwind CLI in watch mode for hot reload.

#### Usage

This service is automatically registered and used internally. You typically don't need to use it directly, but it's available for advanced scenarios:

```csharp
[ApiController]
[Route("api/[controller]")]
public class TailwindController : ControllerBase
{
    private readonly ITailwindCliService _cliService;

    public TailwindController(ITailwindCliService cliService)
    {
        _cliService = cliService;
    }

    [HttpPost("compile")]
    public async Task<IActionResult> CompileCss()
    {
        var version = TailwindVersion.Parse("latest");
        var exitCode = await _cliService.ExecuteCliAsync(
            version,
            "-i ./Styles/app.css -o ./wwwroot/css/app.css --minify"
        );

        return exitCode == 0 ? Ok() : StatusCode(500);
    }
}
```

---

## TailwindVersion

Represents a Tailwind CSS version.

### Static Methods

#### `Parse`
```csharp
public static TailwindVersion Parse(string version)
```
Parses a version string into a `TailwindVersion`.

**Examples:**
```csharp
var v3 = TailwindVersion.Parse("3");
var v4 = TailwindVersion.Parse("4");
var specific = TailwindVersion.Parse("3.4.1");
var latest = TailwindVersion.Parse("latest");
```

### Properties

#### `Major`
```csharp
public int Major { get; }
```
Major version number (3 or 4).

#### `Minor`
```csharp
public int? Minor { get; }
```
Minor version number (optional).

#### `Patch`
```csharp
public int? Patch { get; }
```
Patch version number (optional).

#### `IsLatest`
```csharp
public bool IsLatest { get; }
```
Whether this is the latest version.

#### `IsV4`
```csharp
public bool IsV4 { get; }
```
Whether this is Tailwind CSS v4.x.

#### `IsV3`
```csharp
public bool IsV3 { get; }
```
Whether this is Tailwind CSS v3.x.

### Methods

#### `GetPackageVersion`
```csharp
public string GetPackageVersion()
```
Gets the version string for package installation.

#### `GetPackageName`
```csharp
public string GetPackageName()
```
Gets the npm package name for the version.

---

## See Also

- [Getting Started Guide](GETTING-STARTED.md)
- [Configuration Guide](CONFIGURATION.md)
- [Sample Projects](../samples/)
- [Tailwind CSS Documentation](https://tailwindcss.com/docs)
