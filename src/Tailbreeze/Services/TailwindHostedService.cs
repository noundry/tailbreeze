using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Tailbreeze.Services;

/// <summary>
/// Hosted service that manages Tailwind CLI watch mode during development.
/// </summary>
public sealed class TailwindHostedService : IHostedService, IDisposable
{
    private readonly ITailwindCliService _cliService;
    private readonly IOptions<TailbreezeOptions> _options;
    private readonly IHostEnvironment _environment;
    private readonly ILogger<TailwindHostedService> _logger;
    private IDisposable? _watchHandle;
    private TailwindVersion? _version;

    public TailwindHostedService(
        ITailwindCliService cliService,
        IOptions<TailbreezeOptions> options,
        IHostEnvironment environment,
        ILogger<TailwindHostedService> logger)
    {
        _cliService = cliService;
        _options = options;
        _environment = environment;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var opts = _options.Value;

        if (!opts.EnableHotReload || !_environment.IsDevelopment())
        {
            _logger.LogDebug("Hot reload is disabled or not in development mode");
            return;
        }

        try
        {
            _version = TailwindVersion.Parse(opts.TailwindVersion);
            _logger.LogInformation("Starting Tailbreeze with Tailwind CSS v{Version}", _version);

            var contentRoot = _environment.ContentRootPath;
            var inputPath = Path.Combine(contentRoot, opts.InputCssPath);
            var outputPath = Path.Combine(contentRoot, "wwwroot", opts.OutputCssPath);
            var configPath = Path.Combine(contentRoot, opts.ConfigPath);

            EnsureInputFileExists(inputPath, _version);
            EnsureConfigFileExists(configPath, _version, contentRoot);

            var outputDir = Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrEmpty(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            if (opts.AutoInstallCli)
            {
                await _cliService.EnsureCliInstalledAsync(_version, cancellationToken);
            }

            _watchHandle = await _cliService.StartWatchModeAsync(
                _version,
                inputPath,
                outputPath,
                File.Exists(configPath) ? configPath : null,
                cancellationToken);

            _logger.LogInformation("Tailwind watch mode started successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start Tailwind watch mode");

            if (opts.UseCdnFallback ?? _environment.IsDevelopment())
            {
                _logger.LogWarning("Falling back to CDN mode");
            }
            else
            {
                throw;
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _watchHandle?.Dispose();
        _logger.LogInformation("Tailwind watch mode stopped");
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _watchHandle?.Dispose();
    }

    private void EnsureInputFileExists(string inputPath, TailwindVersion version)
    {
        if (File.Exists(inputPath))
        {
            return;
        }

        var directory = Path.GetDirectoryName(inputPath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var content = version.IsV4
            ? """
              @import "tailwindcss";
              """
            : """
              @tailwind base;
              @tailwind components;
              @tailwind utilities;
              """;

        File.WriteAllText(inputPath, content);
        _logger.LogInformation("Created default input CSS file at {Path}", inputPath);
    }

    private void EnsureConfigFileExists(string configPath, TailwindVersion version, string contentRoot)
    {
        if (File.Exists(configPath))
        {
            return;
        }

        var opts = _options.Value;
        var contentPaths = opts.ContentPaths.Count > 0
            ? opts.ContentPaths
            : DetectContentPaths(contentRoot);

        var content = version.IsV4
            ? GenerateV4Config(contentPaths)
            : GenerateV3Config(contentPaths);

        File.WriteAllText(configPath, content);
        _logger.LogInformation("Created default Tailwind config at {Path}", configPath);
    }

    private static List<string> DetectContentPaths(string contentRoot)
    {
        var paths = new List<string>();

        if (Directory.Exists(Path.Combine(contentRoot, "Pages")))
        {
            paths.Add("./Pages/**/*.{cshtml,razor}");
        }

        if (Directory.Exists(Path.Combine(contentRoot, "Views")))
        {
            paths.Add("./Views/**/*.cshtml");
        }

        if (Directory.Exists(Path.Combine(contentRoot, "Components")))
        {
            paths.Add("./Components/**/*.{cshtml,razor}");
        }

        if (paths.Count == 0)
        {
            paths.Add("./**/*.{cshtml,razor,html}");
        }

        return paths;
    }

    private static string GenerateV3Config(List<string> contentPaths)
    {
        var contentArray = string.Join(",\n    ", contentPaths.Select(p => $"\"{p}\""));

        return $$"""
        /** @type {import('tailwindcss').Config} */
        module.exports = {
          content: [
            {{contentArray}}
          ],
          theme: {
            extend: {},
          },
          plugins: [],
        }
        """;
    }

    private static string GenerateV4Config(List<string> contentPaths)
    {
        var contentArray = string.Join(",\n    ", contentPaths.Select(p => $"\"{p}\""));

        return $$"""
        export default {
          content: [
            {{contentArray}}
          ],
        }
        """;
    }
}
