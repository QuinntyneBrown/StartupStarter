@echo off
REM Build Storybook static site

setlocal

set SCRIPT_DIR=%~dp0
set ROOT_DIR=%SCRIPT_DIR%..\..
set WEBAPP_DIR=%ROOT_DIR%\src\StartupStarter.WebApp

echo =========================================
echo Building Storybook Static Site
echo =========================================

if not exist "%WEBAPP_DIR%" (
    echo Error: WebApp directory not found at %WEBAPP_DIR%
    exit /b 1
)

cd /d "%WEBAPP_DIR%"

echo.
echo Building Storybook...
call npm run build-storybook
if %errorlevel% neq 0 exit /b %errorlevel%

echo.
echo =========================================
echo Storybook build completed!
echo Output directory: %WEBAPP_DIR%\storybook-static
echo =========================================

endlocal
