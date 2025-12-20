#!/bin/bash
# Build the entire StartupStarter solution

set -e  # Exit on error

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$(cd "$SCRIPT_DIR/../.." && pwd)"

echo "========================================="
echo "Building StartupStarter Solution"
echo "========================================="

cd "$ROOT_DIR"

# Clean previous builds
echo ""
echo "Cleaning previous builds..."
dotnet clean StartupStarter.sln

# Restore dependencies
echo ""
echo "Restoring NuGet packages..."
dotnet restore StartupStarter.sln

# Build solution
echo ""
echo "Building solution..."
dotnet build StartupStarter.sln --configuration Release --no-restore

echo ""
echo "========================================="
echo "Build completed successfully!"
echo "========================================="
