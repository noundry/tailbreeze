# Contributing to Tailbreeze

Thank you for your interest in contributing to Tailbreeze! We welcome contributions from the community.

## Code of Conduct

By participating in this project, you agree to maintain a respectful and inclusive environment for everyone.

## How Can I Contribute?

### Reporting Bugs

Before creating bug reports, please check existing issues to avoid duplicates. When creating a bug report, include:

- **Clear title and description**
- **Steps to reproduce**
- **Expected vs actual behavior**
- **.NET version and OS**
- **Tailwind CSS version**
- **Sample code if possible**

### Suggesting Enhancements

Enhancement suggestions are welcome! Please provide:

- **Clear use case**
- **Expected behavior**
- **Examples of how it would work**
- **Why this would be useful**

### Pull Requests

1. **Fork the repository**
2. **Create a feature branch** (`git checkout -b feature/amazing-feature`)
3. **Make your changes**
4. **Add tests if applicable**
5. **Update documentation**
6. **Commit your changes** (`git commit -m 'Add amazing feature'`)
7. **Push to the branch** (`git push origin feature/amazing-feature`)
8. **Open a Pull Request**

## Development Setup

### Prerequisites

- .NET 8.0+ SDK
- Git
- Your favorite IDE (Visual Studio, Rider, or VS Code)

### Building the Project

```bash
# Clone the repository
git clone https://github.com/noundry/tailbreeze.git
cd tailbreeze

# Restore dependencies
dotnet restore

# Build the solution
dotnet build

# Run tests (when available)
dotnet test

# Pack NuGet packages
dotnet pack -c Release
```

### Project Structure

```
Tailbreeze/
├── src/
│   ├── Tailbreeze/              # Main library
│   └── Tailbreeze.Build/        # MSBuild integration
├── samples/
│   ├── Tailbreeze.Samples.RazorPages/
│   ├── Tailbreeze.Samples.BlazorServer/
│   └── Tailbreeze.Samples.BlazorWasm/
├── docs/                        # Documentation
└── README.md
```

## Coding Guidelines

### C# Style

- Follow [Microsoft C# Coding Conventions](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- Use modern C# features appropriately
- Enable nullable reference types
- Add XML documentation for public APIs

### Example

```csharp
namespace Tailbreeze.Services;

/// <summary>
/// Service for managing Tailwind CLI operations.
/// </summary>
public interface ITailwindService
{
    /// <summary>
    /// Compiles Tailwind CSS from the specified input file.
    /// </summary>
    /// <param name="inputPath">Path to the input CSS file.</param>
    /// <param name="outputPath">Path to the output CSS file.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Exit code from the Tailwind CLI.</returns>
    Task<int> CompileAsync(
        string inputPath,
        string outputPath,
        CancellationToken cancellationToken = default);
}
```

### Commit Messages

- Use clear, descriptive commit messages
- Start with a verb in present tense (Add, Fix, Update, etc.)
- Reference issues when applicable

**Good:**
```
Add support for custom content paths
Fix hot reload not triggering on file changes
Update documentation for Blazor integration (#123)
```

**Bad:**
```
fixed stuff
updates
wip
```

## Testing

When adding new features:

1. **Add unit tests** for core functionality
2. **Add integration tests** for end-to-end scenarios
3. **Test on multiple platforms** (Windows, macOS, Linux)
4. **Test with multiple .NET versions** (8.0, 9.0, 10.0)
5. **Test with both Tailwind v3 and v4**

## Documentation

Update documentation when:

- Adding new features
- Changing existing behavior
- Fixing bugs that affect usage
- Adding configuration options

Documentation to update:
- `README.md` - Main documentation
- `docs/GETTING-STARTED.md` - Getting started guide
- `docs/API-REFERENCE.md` - API documentation
- XML comments in code
- Sample projects

## Release Process

1. Update version in `Directory.Build.props`
2. Update `CHANGELOG.md`
3. Create a pull request
4. After merge, tag the release
5. Build and publish NuGet packages
6. Create GitHub release with notes

## Attribution

Tailbreeze is inspired by [tailwind-dotnet](https://github.com/kallebysantos/tailwind-dotnet) by Kalleby Santos. When contributing, ensure that:

- New features maintain compatibility where possible
- Breaking changes are clearly documented
- Attribution is maintained in LICENSE and documentation

## Questions?

Feel free to:
- Open an issue for discussion
- Start a discussion on GitHub Discussions
- Reach out to maintainers

## License

By contributing, you agree that your contributions will be licensed under the MIT License.

---

Thank you for contributing to Tailbreeze!
