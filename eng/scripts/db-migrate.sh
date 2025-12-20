#!/bin/bash
# Add a new database migration

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$(cd "$SCRIPT_DIR/../.." && pwd)"

if [ -z "$1" ]; then
    echo "Usage: ./db-migrate.sh <MigrationName>"
    echo "Example: ./db-migrate.sh AddUserTable"
    exit 1
fi

MIGRATION_NAME=$1

echo "========================================="
echo "Creating Migration: $MIGRATION_NAME"
echo "========================================="

cd "$ROOT_DIR"

dotnet ef migrations add "$MIGRATION_NAME" \
    --project src/StartupStarter.Infrastructure \
    --startup-project src/StartupStarter.Api

echo ""
echo "========================================="
echo "Migration '$MIGRATION_NAME' created successfully!"
echo "========================================="
