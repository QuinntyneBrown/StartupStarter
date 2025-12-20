# StartupStarter Scripts

This directory contains automation scripts to help with common development tasks for the StartupStarter project.

## Table of Contents

- [Quick Start](#quick-start)
- [Available Scripts](#available-scripts)
  - [Setup Scripts](#setup-scripts)
  - [Build Scripts](#build-scripts)
  - [Development Scripts](#development-scripts)
  - [Frontend Scripts](#frontend-scripts)
  - [Storybook Scripts](#storybook-scripts)
  - [Database Scripts](#database-scripts)
  - [Test Scripts](#test-scripts)
- [Platform-Specific Usage](#platform-specific-usage)
- [Examples](#examples)
- [Troubleshooting](#troubleshooting)

---

## Quick Start

### First-Time Setup

**Windows:**
```cmd
eng\scripts\setup.cmd
```

**Linux/Mac/Git Bash:**
```bash
./eng/scripts/setup.sh
```

This will:
1. Restore all .NET dependencies
2. Install frontend npm packages
3. Build the entire solution
4. Update the database to the latest migration

### Running the Application

After setup, start the development servers:

**Windows - Terminal 1 (API):**
```cmd
eng\scripts\api-dev.cmd
```

**Windows - Terminal 2 (Frontend):**
```cmd
eng\scripts\frontend-dev.cmd
```

**Linux/Mac/Git Bash - Terminal 1 (API):**
```bash
./eng/scripts/api-dev.sh
```

**Linux/Mac/Git Bash - Terminal 2 (Frontend):**
```bash
./eng/scripts/frontend-dev.sh
```

---

## Available Scripts

All scripts are available in both Windows (`.cmd`) and Unix (`.sh`) formats.

| Script | Windows | Linux/Mac/Bash | Description |
|--------|---------|----------------|-------------|
| **Setup** | `setup.cmd` | `setup.sh` | Complete environment setup |
| **Build** | `build.cmd` | `build.sh` | Build entire solution |
| **Test** | `test.cmd` | `test.sh` | Run all tests |
| **API Dev** | `api-dev.cmd` | `api-dev.sh` | Start API development server |
| **Frontend Install** | `frontend-install.cmd` | `frontend-install.sh` | Install frontend dependencies |
| **Frontend Dev** | `frontend-dev.cmd` | `frontend-dev.sh` | Start frontend dev server |
| **Frontend Build** | `frontend-build.cmd` | `frontend-build.sh` | Build frontend for production |
| **Storybook Dev** | `storybook-dev.cmd` | `storybook-dev.sh` | Start Storybook server |
| **Storybook Build** | `storybook-build.cmd` | `storybook-build.sh` | Build Storybook static site |
| **DB Migrate** | `db-migrate.cmd` | `db-migrate.sh` | Create database migration |
| **DB Update** | `db-update.cmd` | `db-update.sh` | Apply database migrations |

---

### Setup Scripts

#### setup - Complete Environment Setup

Sets up the entire development environment from scratch.

**Windows:**
```cmd
eng\scripts\setup.cmd
```

**Linux/Mac/Git Bash:**
```bash
./eng/scripts/setup.sh
```

**What it does:**
- Restores all NuGet packages
- Installs all npm dependencies
- Builds the solution in Debug mode
- Applies all database migrations

**When to use:**
- First time setting up the project
- After cloning the repository
- When switching branches with major changes

---

### Build Scripts

#### build - Build Entire Solution

Cleans, restores, and builds the entire .NET solution.

**Windows:**
```cmd
eng\scripts\build.cmd
```

**Linux/Mac/Git Bash:**
```bash
./eng/scripts/build.sh
```

**What it does:**
- Cleans previous build artifacts
- Restores NuGet packages
- Builds all projects in Release mode

**When to use:**
- Before committing code
- After major code changes
- To verify solution compiles

---

### Development Scripts

#### api-dev - Start API Development Server

Starts the ASP.NET Core API with hot reload enabled.

**Windows:**
```cmd
eng\scripts\api-dev.cmd
```

**Linux/Mac/Git Bash:**
```bash
./eng/scripts/api-dev.sh
```

**What it does:**
- Starts the API (typically on https://localhost:7001)
- Enables hot reload (auto-recompile on changes)
- Shows console logging

**When to use:**
- Daily development
- Testing API endpoints
- Making backend changes

---

### Frontend Scripts

#### frontend-install - Install Frontend Dependencies

Installs all npm packages for the Angular application.

**Windows:**
```cmd
eng\scripts\frontend-install.cmd
```

**Linux/Mac/Git Bash:**
```bash
./eng/scripts/frontend-install.sh
```

**What it does:**
- Runs `npm install` in WebApp directory
- Installs Angular CLI and dependencies

**When to use:**
- After cloning repository
- After package.json changes
- When dependencies are out of sync

---

#### frontend-dev - Start Frontend Development Server

Starts the Angular development server.

**Windows:**
```cmd
eng\scripts\frontend-dev.cmd
```

**Linux/Mac/Git Bash:**
```bash
./eng/scripts/frontend-dev.sh
```

**What it does:**
- Starts Angular dev server on http://localhost:4200
- Enables hot reload
- Auto-refreshes browser on changes

**When to use:**
- Daily frontend development
- Testing UI changes
- Working on Angular components

---

#### frontend-build - Build Frontend for Production

Builds the Angular application for production deployment.

**Windows:**
```cmd
eng\scripts\frontend-build.cmd
```

**Linux/Mac/Git Bash:**
```bash
./eng/scripts/frontend-build.sh
```

**What it does:**
- Builds with production optimizations
- Minifies JavaScript and CSS
- Outputs to `src/StartupStarter.WebApp/dist`

**When to use:**
- Before production deployment
- Testing production builds
- Performance testing

---

### Storybook Scripts

#### storybook-dev - Start Storybook Development Server

Starts the Storybook development server for component documentation.

**Windows:**
```cmd
eng\scripts\storybook-dev.cmd
```

**Linux/Mac/Git Bash:**
```bash
./eng/scripts/storybook-dev.sh
```

**What it does:**
- Starts Storybook on http://localhost:6006
- Shows all documented UI components
- Enables interactive component testing

**When to use:**
- Developing UI components
- Documenting new components
- Reviewing component library

---

#### storybook-build - Build Storybook Static Site

Builds a static version of Storybook for deployment.

**Windows:**
```cmd
eng\scripts\storybook-build.cmd
```

**Linux/Mac/Git Bash:**
```bash
./eng/scripts/storybook-build.sh
```

**What it does:**
- Builds static Storybook site
- Outputs to `src/StartupStarter.WebApp/storybook-static`
- Can be deployed to static hosting

**When to use:**
- Deploying component documentation
- Sharing components with team
- Design reviews

---

### Database Scripts

#### db-migrate - Create New Database Migration

Creates a new Entity Framework migration.

**Windows:**
```cmd
eng\scripts\db-migrate.cmd <MigrationName>
```

**Linux/Mac/Git Bash:**
```bash
./eng/scripts/db-migrate.sh <MigrationName>
```

**Examples:**
```bash
# Windows
eng\scripts\db-migrate.cmd AddUserTable

# Linux/Mac/Bash
./eng/scripts/db-migrate.sh AddUserTable
```

**What it does:**
- Generates new migration file
- Creates Up and Down methods
- Updates model snapshot

**When to use:**
- After adding/modifying entities
- Changing database schema
- Before deploying schema changes

---

#### db-update - Apply Database Migrations

Applies all pending migrations to the database.

**Windows:**
```cmd
eng\scripts\db-update.cmd
```

**Linux/Mac/Git Bash:**
```bash
./eng/scripts/db-update.sh
```

**What it does:**
- Connects to database
- Applies unapplied migrations
- Updates database schema

**When to use:**
- After creating new migrations
- After pulling migrations from team
- Setting up new environment

---

### Test Scripts

#### test - Run All Tests

Runs all unit and integration tests with code coverage.

**Windows:**
```cmd
eng\scripts\test.cmd
```

**Linux/Mac/Git Bash:**
```bash
./eng/scripts/test.sh
```

**What it does:**
- Runs all tests in solution
- Generates code coverage reports
- Shows test results

**When to use:**
- Before committing code
- Before creating pull requests
- During CI/CD pipeline
- Verifying bug fixes

---

## Platform-Specific Usage

### Windows

Use `.cmd` scripts from Command Prompt or PowerShell:

```cmd
# From project root
eng\scripts\build.cmd
eng\scripts\test.cmd
eng\scripts\api-dev.cmd
```

### Linux/Mac

Make scripts executable first (one-time setup):

```bash
chmod +x eng/scripts/*.sh
```

Then run:

```bash
./eng/scripts/build.sh
./eng/scripts/test.sh
./eng/scripts/api-dev.sh
```

### Git Bash (Windows)

Use the `.sh` scripts:

```bash
./eng/scripts/build.sh
./eng/scripts/test.sh
./eng/scripts/api-dev.sh
```

---

## Examples

### Full Development Workflow

**Starting development on a new feature:**

**Windows:**
```cmd
REM Update codebase
git pull origin main

REM Setup environment
eng\scripts\setup.cmd

REM Run tests
eng\scripts\test.cmd

REM Start API (Terminal 1)
eng\scripts\api-dev.cmd

REM Start Frontend (Terminal 2)
eng\scripts\frontend-dev.cmd
```

**Linux/Mac/Git Bash:**
```bash
# Update codebase
git pull origin main

# Setup environment
./eng/scripts/setup.sh

# Run tests
./eng/scripts/test.sh

# Start API (Terminal 1)
./eng/scripts/api-dev.sh

# Start Frontend (Terminal 2)
./eng/scripts/frontend-dev.sh
```

---

### Adding New Database Entity

**Example: Adding a Product entity**

**Windows:**
```cmd
REM 1. Create entity class in Core project
REM 2. Add DbSet to DbContext
REM 3. Create migration
eng\scripts\db-migrate.cmd AddProductEntity

REM 4. Review migration
REM    (Check src\StartupStarter.Infrastructure\Migrations\)

REM 5. Apply to database
eng\scripts\db-update.cmd

REM 6. Run tests
eng\scripts\test.cmd
```

**Linux/Mac/Git Bash:**
```bash
# 1-2. Create entity and DbSet
# 3. Create migration
./eng/scripts/db-migrate.sh AddProductEntity

# 4. Review migration

# 5. Apply to database
./eng/scripts/db-update.sh

# 6. Run tests
./eng/scripts/test.sh
```

---

### Production Deployment

**Windows:**
```cmd
REM Run tests
eng\scripts\test.cmd

REM Build solution
eng\scripts\build.cmd

REM Build frontend
eng\scripts\frontend-build.cmd

REM Build Storybook
eng\scripts\storybook-build.cmd

REM Outputs:
REM - Frontend: src\StartupStarter.WebApp\dist
REM - Storybook: src\StartupStarter.WebApp\storybook-static
```

**Linux/Mac/Git Bash:**
```bash
# Run tests
./eng/scripts/test.sh

# Build solution
./eng/scripts/build.sh

# Build frontend
./eng/scripts/frontend-build.sh

# Build Storybook
./eng/scripts/storybook-build.sh
```

---

### Working on UI Components

**Windows:**
```cmd
REM Terminal 1: Storybook
eng\scripts\storybook-dev.cmd

REM Terminal 2: Frontend
eng\scripts\frontend-dev.cmd

REM View:
REM - Storybook: http://localhost:6006
REM - App: http://localhost:4200
```

**Linux/Mac/Git Bash:**
```bash
# Terminal 1: Storybook
./eng/scripts/storybook-dev.sh

# Terminal 2: Frontend
./eng/scripts/frontend-dev.sh
```

---

### Quick Test Before Commit

**Windows:**
```cmd
eng\scripts\build.cmd && eng\scripts\test.cmd
```

**Linux/Mac/Git Bash:**
```bash
./eng/scripts/build.sh && ./eng/scripts/test.sh
```

---

## Troubleshooting

### Script Won't Execute (Unix/Mac)

**Error:** "Permission denied"

**Solution:**
```bash
chmod +x eng/scripts/*.sh
```

---

### Build Fails

**Try cleaning and restoring:**

**Windows:**
```cmd
dotnet clean StartupStarter.sln
dotnet restore StartupStarter.sln
eng\scripts\build.cmd
```

**Linux/Mac:**
```bash
dotnet clean StartupStarter.sln
dotnet restore StartupStarter.sln
./eng/scripts/build.sh
```

---

### Frontend Won't Start

**Clean install dependencies:**

**Windows:**
```cmd
cd src\StartupStarter.WebApp
rmdir /s /q node_modules
del package-lock.json
cd ..\..
eng\scripts\frontend-install.cmd
```

**Linux/Mac:**
```bash
cd src/StartupStarter.WebApp
rm -rf node_modules package-lock.json
cd ../..
./eng/scripts/frontend-install.sh
```

---

### Database Connection Errors

**Check connection string in:**
- `src/StartupStarter.Api/appsettings.Development.json`

**Then update database:**
```bash
# Windows
eng\scripts\db-update.cmd

# Linux/Mac
./eng/scripts/db-update.sh
```

---

### Storybook Build Fails

**Common issue:** CSS/SCSS compatibility

**Solution:** The project includes a simplified CSS file for Storybook. If you encounter build errors:

1. Check [.storybook/preview-styles.css](../../src/StartupStarter.WebApp/projects/startupstarter-admin/.storybook/preview-styles.css)
2. Ensure fonts are loading in [.storybook/main.ts](../../src/StartupStarter.WebApp/projects/startupstarter-admin/.storybook/main.ts)

---

## Advanced Tips

### Running Multiple Services (PowerShell)

```powershell
# Start API in new window
Start-Process pwsh -ArgumentList "-NoExit", "-Command", "eng\scripts\api-dev.cmd"

# Start Frontend in new window
Start-Process pwsh -ArgumentList "-NoExit", "-Command", "eng\scripts\frontend-dev.cmd"

# Start Storybook in new window
Start-Process pwsh -ArgumentList "-NoExit", "-Command", "eng\scripts\storybook-dev.cmd"
```

### Running Multiple Services (Bash)

```bash
# Start all in background
./eng/scripts/api-dev.sh &
./eng/scripts/frontend-dev.sh &
./eng/scripts/storybook-dev.sh &

# View running jobs
jobs

# Kill all background jobs
kill $(jobs -p)
```

---

## Script Structure

Each script follows this pattern:

**Bash (.sh):**
```bash
#!/bin/bash
set -e  # Exit on error

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$(cd "$SCRIPT_DIR/../.." && pwd)"

cd "$ROOT_DIR"
# ... commands ...
```

**Windows (.cmd):**
```cmd
@echo off
setlocal

set SCRIPT_DIR=%~dp0
set ROOT_DIR=%SCRIPT_DIR%..\..

cd /d "%ROOT_DIR%"
REM ... commands ...

endlocal
```

---

## Contributing

When adding new scripts:

1. Create both `.sh` and `.cmd` versions
2. Update this README with:
   - Description
   - Usage examples
   - When to use
3. Test on Windows and Linux/Mac
4. Follow naming conventions

---

## Prerequisites

Ensure you have installed:

- [.NET SDK 8.0+](https://dotnet.microsoft.com/download)
- [Node.js 18+](https://nodejs.org/)
- [npm 10+](https://www.npmjs.com/)
- [Git](https://git-scm.com/)

Optional:
- [Git Bash](https://git-scm.com/) (for Windows users who prefer bash)
- [Visual Studio Code](https://code.visualstudio.com/)

---

## Support

For issues:
1. Check [Troubleshooting](#troubleshooting)
2. Verify you're in project root
3. Ensure prerequisites are installed
4. Check script permissions (Unix/Mac)

For project-specific issues, refer to the main project README.
