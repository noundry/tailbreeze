# Getting Started with Tailbreeze

This guide will walk you through setting up Tailbreeze in a new or existing ASP.NET project.

## Prerequisites

- .NET 8.0, 9.0, or 10.0 SDK
- An ASP.NET Core project (Razor Pages, MVC, or Blazor)
- Basic knowledge of Tailwind CSS (helpful but not required)

## Step 1: Install NuGet Packages

Add both Tailbreeze packages to your project:

```bash
dotnet add package Tailbreeze
dotnet add package Tailbreeze.Build
```

Or via Package Manager Console:

```powershell
Install-Package Tailbreeze
Install-Package Tailbreeze.Build
```

Or add to your `.csproj` file:

```xml
<ItemGroup>
  <PackageReference Include="Tailbreeze" Version="1.0.0" />
  <PackageReference Include="Tailbreeze.Build" Version="1.0.0" />
</ItemGroup>
```

## Step 2: Configure Services

### For Razor Pages or MVC

Open `Program.cs` and add Tailbreeze:

```csharp
using Tailbreeze.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add your existing services
builder.Services.AddRazorPages(); // or AddControllersWithViews()

// Add Tailbreeze - minimal configuration
builder.Services.AddTailbreeze();

// Or with custom configuration
builder.Services.AddTailbreeze(options =>
{
    options.TailwindVersion = "latest"; // Use Tailwind CSS v4
    options.EnableHotReload = true;
    options.UseCdnFallback = true;
});

var app = builder.Build();

// Add Tailbreeze middleware BEFORE static files
app.UseTailbreeze();

// Your existing middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapRazorPages(); // or MapControllers()

app.Run();
```

### For Blazor Server

```csharp
using Tailbreeze.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add your existing services
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add Tailbreeze
builder.Services.AddTailbreeze(options =>
{
    options.TailwindVersion = "latest";
});

var app = builder.Build();

// Add Tailbreeze middleware
app.UseTailbreeze();

// Your existing middleware
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
```

### For Blazor WebAssembly (Hosted)

In the **Server** project's `Program.cs`, add the same configuration as Blazor Server above.

## Step 3: Add Tag Helpers / Components

### For Razor Pages or MVC

Add to `Pages/_ViewImports.cshtml` (or `Views/_ViewImports.cshtml` for MVC):

```cshtml
@addTagHelper *, Tailbreeze
```

Then in your `_Layout.cshtml`:

```html
<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>@ViewData["Title"]</title>

    <!-- Add Tailwind CSS -->
    <tailwind-link cdn-fallback="true" />
</head>
<body>
    @RenderBody()
</body>
</html>
```

### For Blazor

Add to `_Imports.razor`:

```razor
@using Tailbreeze.Components
```

Then in your `MainLayout.razor` or `App.razor`:

```razor
@inherits LayoutComponentBase

<!-- Add Tailwind CSS in the head -->
<HeadContent>
    <TailwindLink CdnFallback="true" />
</HeadContent>

<!-- Your layout content -->
<div class="page">
    @Body
</div>
```

## Step 4: Use Tailwind CSS

You're now ready to use Tailwind CSS in your application! Try adding some classes:

### Razor Pages Example

**Pages/Index.cshtml:**
```html
@page
@model IndexModel

<div class="container mx-auto px-4 py-8">
    <h1 class="text-4xl font-bold text-blue-600">
        Welcome to Tailbreeze!
    </h1>
    <p class="mt-4 text-gray-600">
        Start building with Tailwind CSS in ASP.NET.
    </p>
    <button class="mt-6 bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded">
        Get Started
    </button>
</div>
```

### Blazor Example

**Pages/Index.razor:**
```razor
@page "/"

<div class="container mx-auto px-4 py-8">
    <h1 class="text-4xl font-bold text-blue-600">
        Welcome to Tailbreeze!
    </h1>
    <p class="mt-4 text-gray-600">
        Start building with Tailwind CSS in Blazor.
    </p>
    <button class="mt-6 bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded"
            @onclick="HandleClick">
        Click Me
    </button>
</div>

@code {
    private void HandleClick()
    {
        Console.WriteLine("Button clicked!");
    }
}
```

## Step 5: Run Your Application

Start your application in development mode:

```bash
dotnet watch run
```

### What Happens Automatically

1. **First Run**: Tailbreeze will:
   - Create `Styles/app.css` with Tailwind directives
   - Create `tailwind.config.js` with your project structure
   - Download the Tailwind CLI (first time only)
   - Start watching for file changes

2. **During Development**:
   - CSS automatically regenerates when you change classes
   - Changes appear immediately with hot reload
   - No manual build steps required

3. **On Publish**:
   - CSS is compiled and minified
   - Unused classes are purged
   - Output is optimized for production

## Step 6: Customize (Optional)

### Customize Tailwind Configuration

Edit the auto-generated `tailwind.config.js`:

**For Tailwind v3:**
```javascript
/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./Pages/**/*.{cshtml,razor}",
    "./Views/**/*.cshtml",
    "./Components/**/*.{cshtml,razor}"
  ],
  theme: {
    extend: {
      colors: {
        'brand-blue': '#1E40AF',
      },
    },
  },
  plugins: [],
}
```

**For Tailwind v4:**
```javascript
export default {
  content: [
    "./Pages/**/*.{cshtml,razor}",
    "./Views/**/*.cshtml",
    "./Components/**/*.{cshtml,razor}"
  ],
  theme: {
    extend: {
      colors: {
        'brand-blue': '#1E40AF',
      },
    },
  },
}
```

### Add Custom CSS

Edit `Styles/app.css`:

```css
/* For Tailwind v3 */
@tailwind base;
@tailwind components;
@tailwind utilities;

/* Custom styles */
@layer components {
  .btn-primary {
    @apply bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded;
  }
}

/* For Tailwind v4 */
@import "tailwindcss";

/* Custom styles */
@layer components {
  .btn-primary {
    @apply bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded;
  }
}
```

## Next Steps

- **Explore the [Configuration Guide](CONFIGURATION.md)** for advanced options
- **Check out the [API Reference](API-REFERENCE.md)** for detailed documentation
- **Browse the [Sample Projects](../samples/)** for complete examples
- **Read the [Tailwind CSS Documentation](https://tailwindcss.com/docs)** to learn more about Tailwind

## Common Issues

### CSS Not Loading

1. Check browser console for errors
2. Verify the CSS link tag is in your layout
3. Check that the middleware is added (`app.UseTailbreeze()`)
4. Try enabling CDN fallback for development

### Classes Not Applying

1. Verify content paths in `tailwind.config.js`
2. Check that file extensions match your project
3. Restart the application
4. Check browser cache

### CLI Download Fails

1. Check internet connection
2. Verify no proxy/firewall blocking GitHub
3. Try setting `UseCdnFallback = true` for development
4. Manually download CLI and place in `%LOCALAPPDATA%/Tailbreeze/cli/`

## Getting Help

- **Issues**: [GitHub Issues](https://github.com/yourusername/tailbreeze/issues)
- **Discussions**: [GitHub Discussions](https://github.com/yourusername/tailbreeze/discussions)
- **Documentation**: [Full Documentation](https://github.com/yourusername/tailbreeze/wiki)
