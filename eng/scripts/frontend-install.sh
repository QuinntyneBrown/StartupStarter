#!/bin/bash
# Install frontend dependencies for the Angular application

set -e  # Exit on error

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$(cd "$SCRIPT_DIR/../.." && pwd)"
WEBAPP_DIR="$ROOT_DIR/src/StartupStarter.WebApp"

echo "========================================="
echo "Installing Frontend Dependencies"
echo "========================================="

if [ ! -d "$WEBAPP_DIR" ]; then
    echo "Error: WebApp directory not found at $WEBAPP_DIR"
    exit 1
fi

cd "$WEBAPP_DIR"

echo ""
echo "Installing npm packages..."
npm install

echo ""
echo "========================================="
echo "Frontend dependencies installed!"
echo "========================================="
