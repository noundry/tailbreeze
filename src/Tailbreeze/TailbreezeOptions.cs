namespace Tailbreeze;

/// <summary>
/// Configuration options for Tailbreeze integration.
/// </summary>
public sealed class TailbreezeOptions
{
    /// <summary>
    /// Gets or sets the Tailwind CSS version to use. Defaults to "latest".
    /// Supports "3" for v3.x, "4" for v4.x, or specific versions like "3.4.1".
    /// </summary>
    public string TailwindVersion { get; set; } = "latest";

    /// <summary>
    /// Gets or sets the input CSS file path relative to the project root.
    /// Defaults to "Styles/app.css".
    /// </summary>
    public string InputCssPath { get; set; } = "Styles/app.css";

    /// <summary>
    /// Gets or sets the output CSS file path relative to wwwroot.
    /// Defaults to "css/app.css".
    /// </summary>
    public string OutputCssPath { get; set; } = "css/app.css";

    /// <summary>
    /// Gets or sets the Tailwind configuration file path relative to the project root.
    /// Defaults to "tailwind.config.js".
    /// </summary>
    public string ConfigPath { get; set; } = "tailwind.config.js";

    /// <summary>
    /// Gets or sets whether to enable hot reload during development.
    /// Defaults to true.
    /// </summary>
    public bool EnableHotReload { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to minify CSS output.
    /// Defaults to false in development, true in production.
    /// </summary>
    public bool? MinifyCss { get; set; }

    /// <summary>
    /// Gets or sets whether to automatically install Tailwind CLI if not found.
    /// Defaults to true.
    /// </summary>
    public bool AutoInstallCli { get; set; } = true;

    /// <summary>
    /// Gets or sets the content paths to scan for Tailwind classes.
    /// If empty, will automatically detect based on project type.
    /// </summary>
    public List<string> ContentPaths { get; set; } = [];

    /// <summary>
    /// Gets or sets additional Tailwind CLI arguments.
    /// </summary>
    public string? AdditionalArguments { get; set; }

    /// <summary>
    /// Gets or sets whether to use the CDN fallback if CLI installation fails.
    /// Defaults to true in development, false in production.
    /// </summary>
    public bool? UseCdnFallback { get; set; }

    /// <summary>
    /// Gets or sets the route path for serving the generated CSS.
    /// Defaults to "/tailbreeze/app.css".
    /// </summary>
    public string ServePath { get; set; } = "/tailbreeze/app.css";

    /// <summary>
    /// Gets or sets whether to serve the CSS through middleware.
    /// Defaults to true.
    /// </summary>
    public bool ServeViaMiddleware { get; set; } = true;
}
