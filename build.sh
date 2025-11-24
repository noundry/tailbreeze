#!/bin/bash
# Tailbreeze Build Script
# Usage: ./build.sh [-c|--configuration Release|Debug] [-p|--pack] [--clean] [-h|--help]

set -e

CONFIGURATION="Release"
PACK=false
CLEAN=false

# Parse arguments
while [[ $# -gt 0 ]]; do
    case $1 in
        -c|--configuration)
            CONFIGURATION="$2"
            shift 2
            ;;
        -p|--pack)
            PACK=true
            shift
            ;;
        --clean)
            CLEAN=true
            shift
            ;;
        -h|--help)
            echo "Tailbreeze Build Script"
            echo ""
            echo "Usage: ./build.sh [-c|--configuration Release|Debug] [-p|--pack] [--clean] [-h|--help]"
            echo ""
            echo "Parameters:"
            echo "  -c, --configuration  Build configuration (Release or Debug). Default: Release"
            echo "  -p, --pack           Create NuGet packages after build"
            echo "  --clean              Clean build output before building"
            echo "  -h, --help           Show this help message"
            echo ""
            echo "Examples:"
            echo "  ./build.sh                          # Build in Release mode"
            echo "  ./build.sh -p                       # Build and create NuGet packages"
            echo "  ./build.sh -c Debug                 # Build in Debug mode"
            echo "  ./build.sh --clean -p               # Clean, build, and pack"
            exit 0
            ;;
        *)
            echo "Unknown option: $1"
            echo "Use -h or --help for usage information"
            exit 1
            ;;
    esac
done

echo "Tailbreeze Build Script"
echo "Configuration: $CONFIGURATION"
echo ""

# Clean if requested
if [ "$CLEAN" = true ]; then
    echo "Cleaning..."
    dotnet clean -c "$CONFIGURATION"
    echo "Clean completed successfully!"
    echo ""
fi

# Restore dependencies
echo "Restoring dependencies..."
dotnet restore
echo "Restore completed successfully!"
echo ""

# Build
echo "Building..."
dotnet build -c "$CONFIGURATION" --no-restore
echo "Build completed successfully!"
echo ""

# Pack if requested
if [ "$PACK" = true ]; then
    echo "Creating NuGet packages..."

    OUTPUT_DIR="./artifacts/packages"
    mkdir -p "$OUTPUT_DIR"

    dotnet pack ./src/Tailbreeze/Tailbreeze.csproj -c "$CONFIGURATION" --no-build -o "$OUTPUT_DIR"
    dotnet pack ./src/Tailbreeze.Build/Tailbreeze.Build.csproj -c "$CONFIGURATION" --no-build -o "$OUTPUT_DIR"

    echo "NuGet packages created successfully in $OUTPUT_DIR!"
    echo ""
    echo "Packages created:"
    ls -1 "$OUTPUT_DIR"/*.nupkg | xargs -n 1 basename
fi

echo ""
echo "All operations completed successfully!"
