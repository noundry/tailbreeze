using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;

namespace Tailbreeze.Services;

/// <summary>
/// Default implementation of ITailwindCliService.
/// </summary>
public sealed class TailwindCliService : ITailwindCliService
{
    private readonly ILogger<TailwindCliService> _logger;
    private readonly string _cliDirectory;
    private static readonly SemaphoreSlim _installLock = new(1, 1);

    public TailwindCliService(ILogger<TailwindCliService> logger)
    {
        _logger = logger;

        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        _cliDirectory = Path.Combine(appDataPath, "Tailbreeze", "cli");

        Directory.CreateDirectory(_cliDirectory);
    }

    public async Task<string> EnsureCliInstalledAsync(TailwindVersion version, CancellationToken cancellationToken = default)
    {
        var cliPath = GetCliPath(version);

        if (await IsCliInstalledAsync(version, cancellationToken))
        {
            _logger.LogDebug("Tailwind CLI v{Version} is already installed at {Path}", version, cliPath);
            return cliPath;
        }

        await _installLock.WaitAsync(cancellationToken);
        try
        {
            if (await IsCliInstalledAsync(version, cancellationToken))
            {
                return cliPath;
            }

            _logger.LogInformation("Installing Tailwind CLI v{Version}...", version);

            await DownloadCliAsync(version, cliPath, cancellationToken);

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                var chmod = Process.Start(new ProcessStartInfo
                {
                    FileName = "chmod",
                    Arguments = $"+x \"{cliPath}\"",
                    CreateNoWindow = true,
                    UseShellExecute = false
                });
                await (chmod?.WaitForExitAsync(cancellationToken) ?? Task.CompletedTask);
            }

            _logger.LogInformation("Tailwind CLI v{Version} installed successfully", version);
            return cliPath;
        }
        finally
        {
            _installLock.Release();
        }
    }

    public async Task<bool> IsCliInstalledAsync(TailwindVersion version, CancellationToken cancellationToken = default)
    {
        var cliPath = GetCliPath(version);
        if (!File.Exists(cliPath))
        {
            return false;
        }

        try
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = cliPath,
                    Arguments = "--version",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            await process.WaitForExitAsync(cancellationToken);
            return process.ExitCode == 0;
        }
        catch
        {
            return false;
        }
    }

    public string GetCliPath(TailwindVersion version)
    {
        var fileName = GetPlatformExecutableName();
        var versionDir = Path.Combine(_cliDirectory, version.ToString());
        Directory.CreateDirectory(versionDir);
        return Path.Combine(versionDir, fileName);
    }

    public async Task<int> ExecuteCliAsync(TailwindVersion version, string arguments, CancellationToken cancellationToken = default)
    {
        var cliPath = await EnsureCliInstalledAsync(version, cancellationToken);

        _logger.LogDebug("Executing: {CliPath} {Arguments}", cliPath, arguments);

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = cliPath,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.OutputDataReceived += (_, e) =>
        {
            if (!string.IsNullOrWhiteSpace(e.Data))
            {
                _logger.LogInformation("{Output}", e.Data);
            }
        };

        process.ErrorDataReceived += (_, e) =>
        {
            if (!string.IsNullOrWhiteSpace(e.Data))
            {
                _logger.LogWarning("{Error}", e.Data);
            }
        };

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        await process.WaitForExitAsync(cancellationToken);
        return process.ExitCode;
    }

    public async Task<IDisposable> StartWatchModeAsync(TailwindVersion version, string inputPath, string outputPath, string? configPath = null, CancellationToken cancellationToken = default)
    {
        var cliPath = await EnsureCliInstalledAsync(version, cancellationToken);

        var arguments = $"-i \"{inputPath}\" -o \"{outputPath}\" --watch";
        if (!string.IsNullOrWhiteSpace(configPath) && File.Exists(configPath))
        {
            arguments += $" -c \"{configPath}\"";
        }

        _logger.LogInformation("Starting Tailwind watch mode: {Arguments}", arguments);

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = cliPath,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.OutputDataReceived += (_, e) =>
        {
            if (!string.IsNullOrWhiteSpace(e.Data))
            {
                _logger.LogDebug("Tailwind: {Output}", e.Data);
            }
        };

        process.ErrorDataReceived += (_, e) =>
        {
            if (!string.IsNullOrWhiteSpace(e.Data))
            {
                _logger.LogWarning("Tailwind: {Error}", e.Data);
            }
        };

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        return new WatchModeHandle(process, _logger);
    }

    private async Task DownloadCliAsync(TailwindVersion version, string destinationPath, CancellationToken cancellationToken)
    {
        var downloadUrl = GetDownloadUrl(version);
        _logger.LogDebug("Downloading from {Url}", downloadUrl);

        using var client = new HttpClient();
        client.Timeout = TimeSpan.FromMinutes(5);

        var response = await client.GetAsync(downloadUrl, cancellationToken);
        response.EnsureSuccessStatusCode();

        await using var fileStream = File.Create(destinationPath);
        await response.Content.CopyToAsync(fileStream, cancellationToken);
    }

    private string GetDownloadUrl(TailwindVersion version)
    {
        var (os, arch) = GetPlatformIdentifier();

        // If latest, use the latest release URL
        if (version.IsLatest)
        {
            // Latest currently points to v3.x stable
            return $"https://github.com/tailwindlabs/tailwindcss/releases/latest/download/tailwindcss-{os}-{arch}";
        }

        // For v4, use the latest alpha if no specific version
        if (version.IsV4 && !version.Minor.HasValue)
        {
            return $"https://github.com/tailwindlabs/tailwindcss/releases/download/v4.0.0-alpha.32/tailwindcss-{os}-{arch}";
        }

        // For specific versions, construct the exact URL
        var versionString = version.GetPackageVersion();
        if (version.IsV4)
        {
            // v4 versions are alpha releases
            return $"https://github.com/tailwindlabs/tailwindcss/releases/download/v{versionString}/tailwindcss-{os}-{arch}";
        }

        // v3 specific versions
        return $"https://github.com/tailwindlabs/tailwindcss/releases/download/v{versionString}/tailwindcss-{os}-{arch}";
    }

    private (string os, string arch) GetPlatformIdentifier()
    {
        var os = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "windows" :
                 RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "linux" :
                 RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? "macos" :
                 throw new PlatformNotSupportedException("Unsupported operating system");

        var arch = RuntimeInformation.ProcessArchitecture switch
        {
            Architecture.X64 => "x64",
            Architecture.Arm64 => "arm64",
            Architecture.X86 => "x86",
            _ => throw new PlatformNotSupportedException($"Unsupported architecture: {RuntimeInformation.ProcessArchitecture}")
        };

        return (os, arch);
    }

    private string GetPlatformExecutableName()
    {
        return RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? "tailwindcss.exe"
            : "tailwindcss";
    }

    private sealed class WatchModeHandle : IDisposable
    {
        private readonly Process _process;
        private readonly ILogger _logger;

        public WatchModeHandle(Process process, ILogger logger)
        {
            _process = process;
            _logger = logger;
        }

        public void Dispose()
        {
            if (!_process.HasExited)
            {
                _logger.LogInformation("Stopping Tailwind watch mode");
                _process.Kill(entireProcessTree: true);
            }
            _process.Dispose();
        }
    }
}
