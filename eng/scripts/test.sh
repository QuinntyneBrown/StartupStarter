#!/bin/bash
# Run all tests in the StartupStarter solution

set -e  # Exit on error

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$(cd "$SCRIPT_DIR/../.." && pwd)"

echo "========================================="
echo "Running Tests"
echo "========================================="

cd "$ROOT_DIR"

# Run tests with coverage
echo ""
echo "Running all tests..."
dotnet test StartupStarter.sln --configuration Release --verbosity normal --collect:"XPlat Code Coverage"

echo ""
echo "========================================="
echo "Tests completed successfully!"
echo "========================================="
