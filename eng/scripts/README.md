# StartupStarter Scripts

This directory contains automation scripts to help with common development tasks for the StartupStarter project.

## Table of Contents

- [Available Scripts](#available-scripts)
  - [Build Scripts](#build-scripts)
  - [Test Scripts](#test-scripts)
  - [Database Scripts](#database-scripts)
  - [Development Scripts](#development-scripts)
  - [Deployment Scripts](#deployment-scripts)
- [Script Usage Examples](#script-usage-examples)

---

## Available Scripts

Currently, this repository uses .NET CLI commands and npm scripts rather than standalone script files. Below is a comprehensive guide to all available commands.

### Build Scripts

#### Build Entire Solution
**Command (PowerShell/Bash):**
```bash
dotnet build StartupStarter.sln
```

**Description:** Builds the entire solution including all projects (Core, Infrastructure, API, WebApp).

**Example Usage:**
```bash
# From repository root
dotnet build StartupStarter.sln --configuration Release
```

---

#### Build Specific Project
**Command (PowerShell/Bash):**
```bash
dotnet build src/StartupStarter.Api/StartupStarter.Api.csproj
```

**Description:** Builds a specific project in the solution.

**Example Usage:**
```bash
# Build only the API project
dotnet build src/StartupStarter.Api/StartupStarter.Api.csproj

# Build only the Core project
dotnet build src/StartupStarter.Core/StartupStarter.Core.csproj
```

---

#### Clean Build Artifacts
**Command (PowerShell/Bash):**
```bash
dotnet clean StartupStarter.sln
```

**Description:** Removes all build outputs and intermediate files.

**Example Usage:**
```bash
# Clean and rebuild
dotnet clean StartupStarter.sln
dotnet build StartupStarter.sln
```

---

### Test Scripts

#### Run All Tests
**Command (PowerShell/Bash):**
```bash
dotnet test StartupStarter.sln
```

**Description:** Runs all unit and integration tests in the solution.

**Example Usage:**
```bash
# Run all tests with verbose output
dotnet test StartupStarter.sln --verbosity normal

# Run tests and generate code coverage
dotnet test StartupStarter.sln --collect:"XPlat Code Coverage"
```

---

#### Run Core Tests
**Command (PowerShell/Bash):**
```bash
dotnet test tests/StartupStarter.Core.Tests/StartupStarter.Core.Tests.csproj
```

**Description:** Runs only the Core layer unit tests.

**Example Usage:**
```bash
dotnet test tests/StartupStarter.Core.Tests/StartupStarter.Core.Tests.csproj --logger "console;verbosity=detailed"
```

---

#### Run API Tests
**Command (PowerShell/Bash):**
```bash
dotnet test tests/StartupStarter.Api.Tests/StartupStarter.Api.Tests.csproj
```

**Description:** Runs only the API layer integration tests.

**Example Usage:**
```bash
dotnet test tests/StartupStarter.Api.Tests/StartupStarter.Api.Tests.csproj
```

---

### Database Scripts

#### Add Migration
**Command (PowerShell/Bash):**
```bash
dotnet ef migrations add <MigrationName> --project src/StartupStarter.Infrastructure --startup-project src/StartupStarter.Api
```

**Description:** Creates a new Entity Framework Core migration.

**Example Usage:**
```bash
# Add a new migration for account features
dotnet ef migrations add AddAccountManagement --project src/StartupStarter.Infrastructure --startup-project src/StartupStarter.Api

# Add migration with specific context
dotnet ef migrations add InitialCreate --project src/StartupStarter.Infrastructure --startup-project src/StartupStarter.Api --context StartupStarterDbContext
```

---

#### Update Database
**Command (PowerShell/Bash):**
```bash
dotnet ef database update --project src/StartupStarter.Infrastructure --startup-project src/StartupStarter.Api
```

**Description:** Applies pending migrations to the database.

**Example Usage:**
```bash
# Update to latest migration
dotnet ef database update --project src/StartupStarter.Infrastructure --startup-project src/StartupStarter.Api

# Update to specific migration
dotnet ef database update MigrationName --project src/StartupStarter.Infrastructure --startup-project src/StartupStarter.Api

# Rollback all migrations
dotnet ef database update 0 --project src/StartupStarter.Infrastructure --startup-project src/StartupStarter.Api
```

---

#### Remove Last Migration
**Command (PowerShell/Bash):**
```bash
dotnet ef migrations remove --project src/StartupStarter.Infrastructure --startup-project src/StartupStarter.Api
```

**Description:** Removes the last migration that hasn't been applied to the database.

**Example Usage:**
```bash
dotnet ef migrations remove --project src/StartupStarter.Infrastructure --startup-project src/StartupStarter.Api --force
```

---

#### Generate SQL Script from Migrations
**Command (PowerShell/Bash):**
```bash
dotnet ef migrations script --project src/StartupStarter.Infrastructure --startup-project src/StartupStarter.Api --output migrations.sql
```

**Description:** Generates a SQL script from migrations for manual deployment.

**Example Usage:**
```bash
# Generate script for all migrations
dotnet ef migrations script --project src/StartupStarter.Infrastructure --startup-project src/StartupStarter.Api --output migrations.sql

# Generate script from specific migration to latest
dotnet ef migrations script InitialCreate --project src/StartupStarter.Infrastructure --startup-project src/StartupStarter.Api --output update.sql

# Generate idempotent script (can be run multiple times)
dotnet ef migrations script --project src/StartupStarter.Infrastructure --startup-project src/StartupStarter.Api --idempotent --output migrations.sql
```

---

### Development Scripts

#### Run API Server
**Command (PowerShell/Bash):**
```bash
dotnet run --project src/StartupStarter.Api/StartupStarter.Api.csproj
```

**Description:** Runs the ASP.NET Core API in development mode.

**Example Usage:**
```bash
# Run with specific environment
dotnet run --project src/StartupStarter.Api/StartupStarter.Api.csproj --environment Development

# Run and watch for changes
dotnet watch run --project src/StartupStarter.Api/StartupStarter.Api.csproj
```

---

#### Run Angular Admin Frontend
**Command (PowerShell/Bash):**
```bash
cd src/StartupStarter.WebApp
ng serve
```

**Description:** Starts the Angular development server for the admin frontend.

**Example Usage:**
```bash
# Run on default port (4200)
cd src/StartupStarter.WebApp
ng serve

# Run on custom port
cd src/StartupStarter.WebApp
ng serve --port 4300

# Run with proxy to API
cd src/StartupStarter.WebApp
ng serve --proxy-config proxy.conf.json

# Run and open in browser
cd src/StartupStarter.WebApp
ng serve --open
```

---

#### Install Frontend Dependencies
**Command (PowerShell/Bash):**
```bash
cd src/StartupStarter.WebApp
npm install
```

**Description:** Installs all npm packages for the Angular application.

**Example Usage:**
```bash
cd src/StartupStarter.WebApp
npm install

# Clean install (removes node_modules first)
cd src/StartupStarter.WebApp
rm -rf node_modules package-lock.json
npm install
```

---

#### Build Angular Application
**Command (PowerShell/Bash):**
```bash
cd src/StartupStarter.WebApp
ng build
```

**Description:** Builds the Angular application for production.

**Example Usage:**
```bash
# Production build
cd src/StartupStarter.WebApp
ng build --configuration production

# Development build with source maps
cd src/StartupStarter.WebApp
ng build --configuration development
```

---

#### Run All Services (API + Frontend)
**Command (PowerShell - Run in separate terminals):**
```powershell
# Terminal 1: API
dotnet run --project src/StartupStarter.Api/StartupStarter.Api.csproj

# Terminal 2: Frontend
cd src/StartupStarter.WebApp
ng serve
```

**Command (Bash - Run in separate terminals):**
```bash
# Terminal 1: API
dotnet run --project src/StartupStarter.Api/StartupStarter.Api.csproj

# Terminal 2: Frontend
cd src/StartupStarter.WebApp && ng serve
```

**Description:** Runs both the API and frontend development servers.

---

#### Format Code
**Command (PowerShell/Bash):**
```bash
# Format .NET code
dotnet format StartupStarter.sln

# Format Angular code
cd src/StartupStarter.WebApp
npm run format
```

**Description:** Formats code according to project style guidelines.

**Example Usage:**
```bash
# Format with verification
dotnet format StartupStarter.sln --verify-no-changes

# Format specific project
dotnet format src/StartupStarter.Core/StartupStarter.Core.csproj
```

---

### Deployment Scripts

#### Publish API
**Command (PowerShell/Bash):**
```bash
dotnet publish src/StartupStarter.Api/StartupStarter.Api.csproj --configuration Release --output ./publish/api
```

**Description:** Publishes the API for deployment.

**Example Usage:**
```bash
# Publish for production
dotnet publish src/StartupStarter.Api/StartupStarter.Api.csproj --configuration Release --output ./publish/api

# Publish self-contained for Windows
dotnet publish src/StartupStarter.Api/StartupStarter.Api.csproj --configuration Release --runtime win-x64 --self-contained --output ./publish/api-win

# Publish self-contained for Linux
dotnet publish src/StartupStarter.Api/StartupStarter.Api.csproj --configuration Release --runtime linux-x64 --self-contained --output ./publish/api-linux
```

---

#### Build Docker Image (if Dockerfile exists)
**Command (PowerShell/Bash):**
```bash
docker build -t startupstarter-api:latest -f src/StartupStarter.Api/Dockerfile .
```

**Description:** Builds a Docker image for the API.

**Example Usage:**
```bash
# Build with tag
docker build -t startupstarter-api:1.0.0 -f src/StartupStarter.Api/Dockerfile .

# Build and run
docker build -t startupstarter-api:latest -f src/StartupStarter.Api/Dockerfile .
docker run -p 5000:80 startupstarter-api:latest
```

---

## Script Usage Examples

### Complete Development Workflow

**PowerShell:**
```powershell
# 1. Restore dependencies
dotnet restore StartupStarter.sln
cd src/StartupStarter.WebApp
npm install
cd ../..

# 2. Build everything
dotnet build StartupStarter.sln

# 3. Run tests
dotnet test StartupStarter.sln

# 4. Update database
dotnet ef database update --project src/StartupStarter.Infrastructure --startup-project src/StartupStarter.Api

# 5. Run services (in separate terminals)
# Terminal 1:
dotnet run --project src/StartupStarter.Api/StartupStarter.Api.csproj

# Terminal 2:
cd src/StartupStarter.WebApp
ng serve
```

**Bash:**
```bash
# 1. Restore dependencies
dotnet restore StartupStarter.sln
cd src/StartupStarter.WebApp && npm install && cd ../..

# 2. Build everything
dotnet build StartupStarter.sln

# 3. Run tests
dotnet test StartupStarter.sln

# 4. Update database
dotnet ef database update --project src/StartupStarter.Infrastructure --startup-project src/StartupStarter.Api

# 5. Run services (in separate terminals)
# Terminal 1:
dotnet run --project src/StartupStarter.Api/StartupStarter.Api.csproj

# Terminal 2:
cd src/StartupStarter.WebApp && ng serve
```

---

### Production Deployment Workflow

**PowerShell:**
```powershell
# 1. Run all tests
dotnet test StartupStarter.sln --configuration Release

# 2. Build frontend for production
cd src/StartupStarter.WebApp
ng build --configuration production
cd ../..

# 3. Publish API
dotnet publish src/StartupStarter.Api/StartupStarter.Api.csproj --configuration Release --output ./publish/api

# 4. Generate migration scripts
dotnet ef migrations script --project src/StartupStarter.Infrastructure --startup-project src/StartupStarter.Api --idempotent --output ./publish/migrations.sql
```

**Bash:**
```bash
# 1. Run all tests
dotnet test StartupStarter.sln --configuration Release

# 2. Build frontend for production
cd src/StartupStarter.WebApp && ng build --configuration production && cd ../..

# 3. Publish API
dotnet publish src/StartupStarter.Api/StartupStarter.Api.csproj --configuration Release --output ./publish/api

# 4. Generate migration scripts
dotnet ef migrations script --project src/StartupStarter.Infrastructure --startup-project src/StartupStarter.Api --idempotent --output ./publish/migrations.sql
```

---

## Creating Custom Scripts

If you want to create custom automation scripts, add them to this directory:

- **PowerShell scripts:** Use `.ps1` extension
- **Bash scripts:** Use `.sh` extension
- **Batch scripts:** Use `.bat` or `.cmd` extension

### Example PowerShell Script Template

Create `scripts/build-all.ps1`:
```powershell
#!/usr/bin/env pwsh

Write-Host "Building StartupStarter Solution..." -ForegroundColor Green

# Clean
Write-Host "Cleaning..." -ForegroundColor Yellow
dotnet clean StartupStarter.sln

# Restore
Write-Host "Restoring packages..." -ForegroundColor Yellow
dotnet restore StartupStarter.sln

# Build
Write-Host "Building..." -ForegroundColor Yellow
dotnet build StartupStarter.sln --configuration Release

Write-Host "Build complete!" -ForegroundColor Green
```

### Example Bash Script Template

Create `scripts/build-all.sh`:
```bash
#!/bin/bash

echo "Building StartupStarter Solution..."

# Clean
echo "Cleaning..."
dotnet clean StartupStarter.sln

# Restore
echo "Restoring packages..."
dotnet restore StartupStarter.sln

# Build
echo "Building..."
dotnet build StartupStarter.sln --configuration Release

echo "Build complete!"
```

Make bash scripts executable:
```bash
chmod +x scripts/build-all.sh
```

---

## Notes

- All paths are relative to the repository root
- For PowerShell scripts, you may need to set execution policy: `Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser`
- For bash scripts on Windows, use Git Bash or WSL
- Always test scripts in a development environment before running in production
- Database commands require proper connection strings in `appsettings.json`

---

## Contributing

When adding new scripts:
1. Place them in the `scripts/` directory
2. Update this README with documentation
3. Include usage examples
4. Test on both Windows and Linux/Mac if possible
