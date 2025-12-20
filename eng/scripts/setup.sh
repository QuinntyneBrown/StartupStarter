#!/bin/bash
# Complete development environment setup

set -e  # Exit on error

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$(cd "$SCRIPT_DIR/../.." && pwd)"

echo "========================================="
echo "Setting Up Development Environment"
echo "========================================="

cd "$ROOT_DIR"

# Restore .NET dependencies
echo ""
echo "Step 1/4: Restoring .NET dependencies..."
dotnet restore StartupStarter.sln

# Install frontend dependencies
echo ""
echo "Step 2/4: Installing frontend dependencies..."
cd src/StartupStarter.WebApp
npm install
cd "$ROOT_DIR"

# Build solution
echo ""
echo "Step 3/4: Building solution..."
dotnet build StartupStarter.sln --configuration Debug

# Update database
echo ""
echo "Step 4/4: Updating database..."
dotnet ef database update \
    --project src/StartupStarter.Infrastructure \
    --startup-project src/StartupStarter.Api

echo ""
echo "========================================="
echo "Development environment setup complete!"
echo "========================================="
echo ""
echo "Next steps:"
echo "  - Run API:      ./eng/scripts/api-dev.sh"
echo "  - Run Frontend: ./eng/scripts/frontend-dev.sh"
echo "  - Run Storybook: ./eng/scripts/storybook-dev.sh"
echo ""
