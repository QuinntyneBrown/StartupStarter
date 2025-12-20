#!/bin/bash
# Build the Angular frontend application for production

set -e  # Exit on error

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$(cd "$SCRIPT_DIR/../.." && pwd)"
WEBAPP_DIR="$ROOT_DIR/src/StartupStarter.WebApp"

echo "========================================="
echo "Building Frontend Application"
echo "========================================="

if [ ! -d "$WEBAPP_DIR" ]; then
    echo "Error: WebApp directory not found at $WEBAPP_DIR"
    exit 1
fi

cd "$WEBAPP_DIR"

echo ""
echo "Building Angular application for production..."
npm run build -- --configuration production

echo ""
echo "========================================="
echo "Frontend build completed!"
echo "Output directory: $WEBAPP_DIR/dist"
echo "========================================="
