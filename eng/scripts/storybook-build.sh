#!/bin/bash
# Build Storybook static site

set -e  # Exit on error

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$(cd "$SCRIPT_DIR/../.." && pwd)"
WEBAPP_DIR="$ROOT_DIR/src/StartupStarter.WebApp"

echo "========================================="
echo "Building Storybook Static Site"
echo "========================================="

if [ ! -d "$WEBAPP_DIR" ]; then
    echo "Error: WebApp directory not found at $WEBAPP_DIR"
    exit 1
fi

cd "$WEBAPP_DIR"

echo ""
echo "Building Storybook..."
npm run build-storybook

echo ""
echo "========================================="
echo "Storybook build completed!"
echo "Output directory: $WEBAPP_DIR/storybook-static"
echo "========================================="
