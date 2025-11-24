using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using BuildTask = Microsoft.Build.Utilities.Task;

namespace Tailbreeze.Build.Tasks;

/// <summary>
/// MSBuild task for compiling Tailwind CSS.
/// </summary>
public sealed class CompileTailwindTask : BuildTask, ICancelableTask
{
    private readonly CancellationTokenSource _cts = new();

    [Required]
    public string? InputPath { get; set; }

    [Required]
    public string? OutputPath { get; set; }

    public string? ConfigPath { get; set; }

    public string Version { get; set; } = "latest";

    public bool Minify { get; set; }

    public bool AutoInstall { get; set; } = true;

    public string? AdditionalArguments { get; set; }

    public string? ProjectDirectory { get; set; }

    public override bool Execute()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(InputPath) || string.IsNullOrWhiteSpace(OutputPath))
            {
                Log.LogError("InputPath and OutputPath are required");
                return false;
            }

            EnsureInputFileExists();
            EnsureConfigFileExists();

            var cliPath = GetCliPath();

            if (!File.Exists(cliPath))
            {
                if (!AutoInstall)
                {
                    Log.LogError($"Tailwind CLI not found at {cliPath}. Set TailbreezeAutoInstall=true to download automatically.");
                    return false;
                }

                Log.LogMessage(MessageImportance.High, $"Installing Tailwind CLI v{Version}...");
                InstallCli(cliPath);
            }

            var outputDir = Path.GetDirectoryName(OutputPath);
            if (!string.IsNullOrEmpty(outputDir) && !Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            Log.LogMessage(MessageImportance.High, "Compiling Tailwind CSS...");

            var arguments = $"-i \"{InputPath}\" -o \"{OutputPath}\"";

            if (!string.IsNullOrWhiteSpace(ConfigPath) && File.Exists(ConfigPath))
            {
                arguments += $" -c \"{ConfigPath}\"";
            }

            if (Minify)
            {
                arguments += " --minify";
            }

            if (!string.IsNullOrWhiteSpace(AdditionalArguments))
            {
                arguments += $" {AdditionalArguments}";
            }

            var exitCode = ExecuteCli(cliPath, arguments);

            if (exitCode != 0)
            {
                Log.LogError($"Tailwind CLI exited with code {exitCode}");
                return false;
            }

            Log.LogMessage(MessageImportance.High, $"Tailwind CSS compiled successfully to {OutputPath}");
            return true;
        }
        catch (Exception ex)
        {
            Log.LogErrorFromException(ex);
            return false;
        }
    }

    public void Cancel()
    {
        _cts.Cancel();
    }

    private void EnsureInputFileExists()
    {
        if (File.Exists(InputPath))
        {
            return;
        }

        var directory = Path.GetDirectoryName(InputPath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var isV4 = Version.StartsWith("4") || Version.Equals("latest", StringComparison.OrdinalIgnoreCase);
        var content = isV4
            ? "@import \"tailwindcss\";"
            : "@tailwind base;\n@tailwind components;\n@tailwind utilities;";

        File.WriteAllText(InputPath!, content);
        Log.LogMessage(MessageImportance.Normal, $"Created default input CSS file at {InputPath}");
    }

    private void EnsureConfigFileExists()
    {
        if (string.IsNullOrWhiteSpace(ConfigPath) || File.Exists(ConfigPath))
        {
            return;
        }

        var contentPaths = DetectContentPaths();
        var isV4 = Version.StartsWith("4") || Version.Equals("latest", StringComparison.OrdinalIgnoreCase);

        var content = isV4
            ? GenerateV4Config(contentPaths)
            : GenerateV3Config(contentPaths);

        File.WriteAllText(ConfigPath, content);
        Log.LogMessage(MessageImportance.Normal, $"Created default Tailwind config at {ConfigPath}");
    }

    private string[] DetectContentPaths()
    {
        if (string.IsNullOrWhiteSpace(ProjectDirectory))
        {
            return new[] { "./**/*.{cshtml,razor,html}" };
        }

        var paths = new System.Collections.Generic.List<string>();

        if (Directory.Exists(Path.Combine(ProjectDirectory, "Pages")))
        {
            paths.Add("./Pages/**/*.{cshtml,razor}");
        }

        if (Directory.Exists(Path.Combine(ProjectDirectory, "Views")))
        {
            paths.Add("./Views/**/*.cshtml");
        }

        if (Directory.Exists(Path.Combine(ProjectDirectory, "Components")))
        {
            paths.Add("./Components/**/*.{cshtml,razor}");
        }

        return paths.Count > 0 ? paths.ToArray() : new[] { "./**/*.{cshtml,razor,html}" };
    }

    private string GenerateV3Config(string[] contentPaths)
    {
        var contentArray = string.Join(",\n    ", contentPaths.Select(p => $"\"{p}\""));

        return $@"/** @type {{import('tailwindcss').Config}} */
module.exports = {{
  content: [
    {contentArray}
  ],
  theme: {{
    extend: {{}},
  }},
  plugins: [],
}}";
    }

    private string GenerateV4Config(string[] contentPaths)
    {
        var contentArray = string.Join(",\n    ", contentPaths.Select(p => $"\"{p}\""));

        return $@"export default {{
  content: [
    {contentArray}
  ],
}}";
    }

    private string GetCliPath()
    {
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var cliDirectory = Path.Combine(appDataPath, "Tailbreeze", "cli", Version);
        Directory.CreateDirectory(cliDirectory);

        var fileName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? "tailwindcss.exe"
            : "tailwindcss";

        return Path.Combine(cliDirectory, fileName);
    }

    private void InstallCli(string cliPath)
    {
        var downloadUrl = GetDownloadUrl();
        Log.LogMessage(MessageImportance.Normal, $"Downloading from {downloadUrl}");

        using var client = new HttpClient { Timeout = TimeSpan.FromMinutes(5) };
        var response = client.GetAsync(downloadUrl, _cts.Token).Result;
        response.EnsureSuccessStatusCode();

        using var fileStream = File.Create(cliPath);
        response.Content.CopyToAsync(fileStream).Wait();
        fileStream.Close();

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            var chmod = Process.Start(new ProcessStartInfo
            {
                FileName = "chmod",
                Arguments = $"+x \"{cliPath}\"",
                CreateNoWindow = true,
                UseShellExecute = false
            });
            chmod?.WaitForExit();
        }

        Log.LogMessage(MessageImportance.High, "Tailwind CLI installed successfully");
    }

    private string GetDownloadUrl()
    {
        var (os, arch) = GetPlatformIdentifier();
        var version = Version.TrimStart('v', 'V');

        // Handle "latest" - points to latest stable (currently v3.x)
        if (version.Equals("latest", StringComparison.OrdinalIgnoreCase))
        {
            return $"https://github.com/tailwindlabs/tailwindcss/releases/latest/download/tailwindcss-{os}-{arch}";
        }

        // Handle major version only (e.g., "3" or "4")
        if (version == "4")
        {
            // Latest v4 alpha
            return $"https://github.com/tailwindlabs/tailwindcss/releases/download/v4.0.0-alpha.32/tailwindcss-{os}-{arch}";
        }

        if (version == "3")
        {
            // Latest v3 stable
            return $"https://github.com/tailwindlabs/tailwindcss/releases/latest/download/tailwindcss-{os}-{arch}";
        }

        // Handle specific versions (e.g., "3.4.1", "4.0.0-alpha.25")
        return $"https://github.com/tailwindlabs/tailwindcss/releases/download/v{version}/tailwindcss-{os}-{arch}";
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

    private int ExecuteCli(string cliPath, string arguments)
    {
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
                Log.LogMessage(MessageImportance.Normal, e.Data);
            }
        };

        process.ErrorDataReceived += (_, e) =>
        {
            if (!string.IsNullOrWhiteSpace(e.Data))
            {
                Log.LogWarning(e.Data);
            }
        };

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        process.WaitForExit();
        return process.ExitCode;
    }
}
