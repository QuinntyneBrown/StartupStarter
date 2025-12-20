@echo off
REM Update database to latest migration

setlocal

set SCRIPT_DIR=%~dp0
set ROOT_DIR=%SCRIPT_DIR%..\..

echo =========================================
echo Updating Database
echo =========================================

cd /d "%ROOT_DIR%"

dotnet ef database update --project src\StartupStarter.Infrastructure --startup-project src\StartupStarter.Api
if %errorlevel% neq 0 exit /b %errorlevel%

echo.
echo =========================================
echo Database updated successfully!
echo =========================================

endlocal
