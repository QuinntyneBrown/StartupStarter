@echo off
REM Install frontend dependencies for the Angular application

setlocal

set SCRIPT_DIR=%~dp0
set ROOT_DIR=%SCRIPT_DIR%..\..
set WEBAPP_DIR=%ROOT_DIR%\src\StartupStarter.WebApp

echo =========================================
echo Installing Frontend Dependencies
echo =========================================

if not exist "%WEBAPP_DIR%" (
    echo Error: WebApp directory not found at %WEBAPP_DIR%
    exit /b 1
)

cd /d "%WEBAPP_DIR%"

echo.
echo Installing npm packages...
call npm install
if %errorlevel% neq 0 exit /b %errorlevel%

echo.
echo =========================================
echo Frontend dependencies installed!
echo =========================================

endlocal
