namespace Tailbreeze.Services;

/// <summary>
/// Service for managing Tailwind CLI installation and execution.
/// </summary>
public interface ITailwindCliService
{
    /// <summary>
    /// Ensures the Tailwind CLI is installed for the specified version.
    /// </summary>
    Task<string> EnsureCliInstalledAsync(TailwindVersion version, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if the CLI is installed for the specified version.
    /// </summary>
    Task<bool> IsCliInstalledAsync(TailwindVersion version, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the path to the installed CLI executable.
    /// </summary>
    string GetCliPath(TailwindVersion version);

    /// <summary>
    /// Executes the Tailwind CLI with the specified arguments.
    /// </summary>
    Task<int> ExecuteCliAsync(TailwindVersion version, string arguments, CancellationToken cancellationToken = default);

    /// <summary>
    /// Starts the Tailwind CLI in watch mode for hot reload.
    /// </summary>
    Task<IDisposable> StartWatchModeAsync(TailwindVersion version, string inputPath, string outputPath, string? configPath = null, CancellationToken cancellationToken = default);
}
