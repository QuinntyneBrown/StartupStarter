#!/bin/bash
# Start Storybook development server

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$(cd "$SCRIPT_DIR/../.." && pwd)"
WEBAPP_DIR="$ROOT_DIR/src/StartupStarter.WebApp"

echo "========================================="
echo "Starting Storybook Development Server"
echo "========================================="

if [ ! -d "$WEBAPP_DIR" ]; then
    echo "Error: WebApp directory not found at $WEBAPP_DIR"
    exit 1
fi

cd "$WEBAPP_DIR"

echo ""
echo "Starting Storybook on http://localhost:6006"
echo "Press Ctrl+C to stop"
echo ""

npm run storybook
