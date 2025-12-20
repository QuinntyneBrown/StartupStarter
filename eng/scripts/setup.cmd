@echo off
REM Complete development environment setup

setlocal

set SCRIPT_DIR=%~dp0
set ROOT_DIR=%SCRIPT_DIR%..\..

echo =========================================
echo Setting Up Development Environment
echo =========================================

cd /d "%ROOT_DIR%"

REM Restore .NET dependencies
echo.
echo Step 1/4: Restoring .NET dependencies...
dotnet restore StartupStarter.sln
if %errorlevel% neq 0 exit /b %errorlevel%

REM Install frontend dependencies
echo.
echo Step 2/4: Installing frontend dependencies...
cd /d "src\StartupStarter.WebApp"
call npm install
if %errorlevel% neq 0 exit /b %errorlevel%
cd /d "%ROOT_DIR%"

REM Build solution
echo.
echo Step 3/4: Building solution...
dotnet build StartupStarter.sln --configuration Debug
if %errorlevel% neq 0 exit /b %errorlevel%

REM Update database
echo.
echo Step 4/4: Updating database...
dotnet ef database update --project src\StartupStarter.Infrastructure --startup-project src\StartupStarter.Api
if %errorlevel% neq 0 exit /b %errorlevel%

echo.
echo =========================================
echo Development environment setup complete!
echo =========================================
echo.
echo Next steps:
echo   - Run API:       eng\scripts\api-dev.cmd
echo   - Run Frontend:  eng\scripts\frontend-dev.cmd
echo   - Run Storybook: eng\scripts\storybook-dev.cmd
echo.

endlocal
