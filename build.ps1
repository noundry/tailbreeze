# Tailbreeze Build Script
# Usage: .\build.ps1 [-Configuration Release|Debug] [-Pack] [-Clean]

param(
    [string]$Configuration = "Release",
    [switch]$Pack,
    [switch]$Clean,
    [switch]$Help
)

if ($Help) {
    Write-Host "Tailbreeze Build Script" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Usage: .\build.ps1 [-Configuration Release|Debug] [-Pack] [-Clean]" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Parameters:"
    Write-Host "  -Configuration  Build configuration (Release or Debug). Default: Release"
    Write-Host "  -Pack           Create NuGet packages after build"
    Write-Host "  -Clean          Clean build output before building"
    Write-Host "  -Help           Show this help message"
    Write-Host ""
    Write-Host "Examples:"
    Write-Host "  .\build.ps1                          # Build in Release mode"
    Write-Host "  .\build.ps1 -Pack                    # Build and create NuGet packages"
    Write-Host "  .\build.ps1 -Configuration Debug     # Build in Debug mode"
    Write-Host "  .\build.ps1 -Clean -Pack             # Clean, build, and pack"
    exit 0
}

Write-Host "Tailbreeze Build Script" -ForegroundColor Cyan
Write-Host "Configuration: $Configuration" -ForegroundColor Yellow
Write-Host ""

# Clean if requested
if ($Clean) {
    Write-Host "Cleaning..." -ForegroundColor Yellow
    dotnet clean -c $Configuration
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Clean failed!" -ForegroundColor Red
        exit $LASTEXITCODE
    }
    Write-Host "Clean completed successfully!" -ForegroundColor Green
    Write-Host ""
}

# Restore dependencies
Write-Host "Restoring dependencies..." -ForegroundColor Yellow
dotnet restore
if ($LASTEXITCODE -ne 0) {
    Write-Host "Restore failed!" -ForegroundColor Red
    exit $LASTEXITCODE
}
Write-Host "Restore completed successfully!" -ForegroundColor Green
Write-Host ""

# Build
Write-Host "Building..." -ForegroundColor Yellow
dotnet build -c $Configuration --no-restore
if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed!" -ForegroundColor Red
    exit $LASTEXITCODE
}
Write-Host "Build completed successfully!" -ForegroundColor Green
Write-Host ""

# Pack if requested
if ($Pack) {
    Write-Host "Creating NuGet packages..." -ForegroundColor Yellow

    $outputDir = ".\artifacts\packages"
    if (!(Test-Path $outputDir)) {
        New-Item -ItemType Directory -Path $outputDir -Force | Out-Null
    }

    dotnet pack .\src\Tailbreeze\Tailbreeze.csproj -c $Configuration --no-build -o $outputDir
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Pack failed for Tailbreeze!" -ForegroundColor Red
        exit $LASTEXITCODE
    }

    dotnet pack .\src\Tailbreeze.Build\Tailbreeze.Build.csproj -c $Configuration --no-build -o $outputDir
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Pack failed for Tailbreeze.Build!" -ForegroundColor Red
        exit $LASTEXITCODE
    }

    Write-Host "NuGet packages created successfully in $outputDir!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Packages created:" -ForegroundColor Cyan
    Get-ChildItem $outputDir -Filter "*.nupkg" | ForEach-Object {
        Write-Host "  - $($_.Name)" -ForegroundColor White
    }
}

Write-Host ""
Write-Host "All operations completed successfully!" -ForegroundColor Green
