@echo off
REM Start Storybook development server

setlocal

set SCRIPT_DIR=%~dp0
set ROOT_DIR=%SCRIPT_DIR%..\..
set WEBAPP_DIR=%ROOT_DIR%\src\StartupStarter.WebApp

echo =========================================
echo Starting Storybook Development Server
echo =========================================

if not exist "%WEBAPP_DIR%" (
    echo Error: WebApp directory not found at %WEBAPP_DIR%
    exit /b 1
)

cd /d "%WEBAPP_DIR%"

echo.
echo Starting Storybook on http://localhost:6006
echo Press Ctrl+C to stop
echo.

call npm run storybook

endlocal
