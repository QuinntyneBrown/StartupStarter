#!/bin/bash
# Start the Angular development server

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$(cd "$SCRIPT_DIR/../.." && pwd)"
WEBAPP_DIR="$ROOT_DIR/src/StartupStarter.WebApp"

echo "========================================="
echo "Starting Frontend Development Server"
echo "========================================="

if [ ! -d "$WEBAPP_DIR" ]; then
    echo "Error: WebApp directory not found at $WEBAPP_DIR"
    exit 1
fi

cd "$WEBAPP_DIR"

echo ""
echo "Starting Angular dev server on http://localhost:4200"
echo "Press Ctrl+C to stop"
echo ""

npm run start
