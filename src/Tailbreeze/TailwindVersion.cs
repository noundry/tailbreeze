namespace Tailbreeze;

/// <summary>
/// Represents a Tailwind CSS version.
/// </summary>
public sealed class TailwindVersion
{
    /// <summary>
    /// Gets the major version (3 or 4).
    /// </summary>
    public int Major { get; }

    /// <summary>
    /// Gets the minor version.
    /// </summary>
    public int? Minor { get; }

    /// <summary>
    /// Gets the patch version.
    /// </summary>
    public int? Patch { get; }

    /// <summary>
    /// Gets whether this is the latest version.
    /// </summary>
    public bool IsLatest { get; }

    /// <summary>
    /// Gets whether this is Tailwind CSS v4.x.
    /// </summary>
    public bool IsV4 => Major == 4;

    /// <summary>
    /// Gets whether this is Tailwind CSS v3.x.
    /// </summary>
    public bool IsV3 => Major == 3;

    private TailwindVersion(int major, int? minor = null, int? patch = null, bool isLatest = false)
    {
        Major = major;
        Minor = minor;
        Patch = patch;
        IsLatest = isLatest;
    }

    /// <summary>
    /// Parses a version string into a TailwindVersion.
    /// </summary>
    public static TailwindVersion Parse(string version)
    {
        if (string.IsNullOrWhiteSpace(version) || version.Equals("latest", StringComparison.OrdinalIgnoreCase))
        {
            return new TailwindVersion(4, isLatest: true);
        }

        version = version.TrimStart('v', 'V');

        var parts = version.Split('.');
        if (parts.Length == 0 || !int.TryParse(parts[0], out var major))
        {
            throw new ArgumentException($"Invalid Tailwind version: {version}", nameof(version));
        }

        int? minor = parts.Length > 1 && int.TryParse(parts[1], out var m) ? m : null;
        int? patch = parts.Length > 2 && int.TryParse(parts[2], out var p) ? p : null;

        return new TailwindVersion(major, minor, patch);
    }

    /// <summary>
    /// Gets the version string for package installation.
    /// </summary>
    public string GetPackageVersion()
    {
        if (IsLatest)
        {
            return "latest";
        }

        if (Minor.HasValue && Patch.HasValue)
        {
            return $"{Major}.{Minor}.{Patch}";
        }

        if (Minor.HasValue)
        {
            return $"{Major}.{Minor}";
        }

        return $"{Major}";
    }

    /// <summary>
    /// Gets the npm package name for the specified version.
    /// </summary>
    public string GetPackageName()
    {
        return IsV4 ? "tailwindcss@next" : "tailwindcss";
    }

    public override string ToString() => IsLatest ? "latest" : GetPackageVersion();
}
