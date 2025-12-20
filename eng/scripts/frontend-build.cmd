@echo off
REM Build the Angular frontend application for production

setlocal

set SCRIPT_DIR=%~dp0
set ROOT_DIR=%SCRIPT_DIR%..\..
set WEBAPP_DIR=%ROOT_DIR%\src\StartupStarter.WebApp

echo =========================================
echo Building Frontend Application
echo =========================================

if not exist "%WEBAPP_DIR%" (
    echo Error: WebApp directory not found at %WEBAPP_DIR%
    exit /b 1
)

cd /d "%WEBAPP_DIR%"

echo.
echo Building Angular application for production...
call npm run build -- --configuration production
if %errorlevel% neq 0 exit /b %errorlevel%

echo.
echo =========================================
echo Frontend build completed!
echo Output directory: %WEBAPP_DIR%\dist
echo =========================================

endlocal
