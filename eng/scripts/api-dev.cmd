@echo off
REM Start the API development server

setlocal

set SCRIPT_DIR=%~dp0
set ROOT_DIR=%SCRIPT_DIR%..\..

echo =========================================
echo Starting API Development Server
echo =========================================

cd /d "%ROOT_DIR%"

echo.
echo Starting API with hot reload...
echo Press Ctrl+C to stop
echo.

dotnet watch run --project src\StartupStarter.Api\StartupStarter.Api.csproj

endlocal
