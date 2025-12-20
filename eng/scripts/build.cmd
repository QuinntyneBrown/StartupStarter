@echo off
REM Build the entire StartupStarter solution

setlocal

set SCRIPT_DIR=%~dp0
set ROOT_DIR=%SCRIPT_DIR%..\..

echo =========================================
echo Building StartupStarter Solution
echo =========================================

cd /d "%ROOT_DIR%"

REM Clean previous builds
echo.
echo Cleaning previous builds...
dotnet clean StartupStarter.sln
if %errorlevel% neq 0 exit /b %errorlevel%

REM Restore dependencies
echo.
echo Restoring NuGet packages...
dotnet restore StartupStarter.sln
if %errorlevel% neq 0 exit /b %errorlevel%

REM Build solution
echo.
echo Building solution...
dotnet build StartupStarter.sln --configuration Release --no-restore
if %errorlevel% neq 0 exit /b %errorlevel%

echo.
echo =========================================
echo Build completed successfully!
echo =========================================

endlocal
