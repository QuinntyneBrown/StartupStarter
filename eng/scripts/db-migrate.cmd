@echo off
REM Add a new database migration

setlocal

set SCRIPT_DIR=%~dp0
set ROOT_DIR=%SCRIPT_DIR%..\..

if "%~1"=="" (
    echo Usage: db-migrate.cmd ^<MigrationName^>
    echo Example: db-migrate.cmd AddUserTable
    exit /b 1
)

set MIGRATION_NAME=%~1

echo =========================================
echo Creating Migration: %MIGRATION_NAME%
echo =========================================

cd /d "%ROOT_DIR%"

dotnet ef migrations add "%MIGRATION_NAME%" --project src\StartupStarter.Infrastructure --startup-project src\StartupStarter.Api
if %errorlevel% neq 0 exit /b %errorlevel%

echo.
echo =========================================
echo Migration '%MIGRATION_NAME%' created successfully!
echo =========================================

endlocal
