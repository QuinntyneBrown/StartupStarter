#!/bin/bash
# Update database to latest migration

set -e  # Exit on error

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$(cd "$SCRIPT_DIR/../.." && pwd)"

echo "========================================="
echo "Updating Database"
echo "========================================="

cd "$ROOT_DIR"

dotnet ef database update \
    --project src/StartupStarter.Infrastructure \
    --startup-project src/StartupStarter.Api

echo ""
echo "========================================="
echo "Database updated successfully!"
echo "========================================="
