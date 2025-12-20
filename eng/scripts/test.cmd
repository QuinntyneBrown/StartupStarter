@echo off
REM Run all tests in the StartupStarter solution

setlocal

set SCRIPT_DIR=%~dp0
set ROOT_DIR=%SCRIPT_DIR%..\..

echo =========================================
echo Running Tests
echo =========================================

cd /d "%ROOT_DIR%"

REM Run tests with coverage
echo.
echo Running all tests...
dotnet test StartupStarter.sln --configuration Release --verbosity normal --collect:"XPlat Code Coverage"
if %errorlevel% neq 0 exit /b %errorlevel%

echo.
echo =========================================
echo Tests completed successfully!
echo =========================================

endlocal
