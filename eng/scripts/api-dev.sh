#!/bin/bash
# Start the API development server

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$(cd "$SCRIPT_DIR/../.." && pwd)"

echo "========================================="
echo "Starting API Development Server"
echo "========================================="

cd "$ROOT_DIR"

echo ""
echo "Starting API with hot reload..."
echo "Press Ctrl+C to stop"
echo ""

dotnet watch run --project src/StartupStarter.Api/StartupStarter.Api.csproj
