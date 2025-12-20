@echo off
REM Start the Angular development server

setlocal

set SCRIPT_DIR=%~dp0
set ROOT_DIR=%SCRIPT_DIR%..\..
set WEBAPP_DIR=%ROOT_DIR%\src\StartupStarter.WebApp

echo =========================================
echo Starting Frontend Development Server
echo =========================================

if not exist "%WEBAPP_DIR%" (
    echo Error: WebApp directory not found at %WEBAPP_DIR%
    exit /b 1
)

cd /d "%WEBAPP_DIR%"

echo.
echo Starting Angular dev server on http://localhost:4200
echo Press Ctrl+C to stop
echo.

call npm run start

endlocal
