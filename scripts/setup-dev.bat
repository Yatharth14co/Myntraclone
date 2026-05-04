@echo off
REM E-Commerce System Development Setup Script for Windows

echo.
echo 🚀 E-Commerce System Development Setup
echo ======================================

setlocal enabledelayedexpansion

REM Check .NET SDK
dotnet --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ⚠ .NET SDK not found. Please install .NET 8 SDK from https://dotnet.microsoft.com/download
    exit /b 1
)
for /f "tokens=1" %%a in ('dotnet --version') do set DOTNET_VERSION=%%a
echo ✓ .NET SDK %DOTNET_VERSION% installed

REM Check Node.js
node --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ⚠ Node.js not found. Please install Node.js 18+ from https://nodejs.org
    exit /b 1
)
for /f "tokens=1" %%a in ('node --version') do set NODE_VERSION=%%a
echo ✓ Node.js %NODE_VERSION% installed

REM Check Git
git --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ⚠ Git not found. Please install Git from https://git-scm.com
    exit /b 1
)
echo ✓ Git installed

REM Setup Backend
echo.
echo ▶ Setting up backend...
cd backend

if not exist "appsettings.Development.json" (
    copy appsettings.json appsettings.Development.json
    echo ✓ Created appsettings.Development.json
)

echo ▶ Restoring backend dependencies...
dotnet restore
if %errorlevel% neq 0 (
    echo ⚠ Failed to restore backend dependencies
    exit /b 1
)
echo ✓ Backend dependencies restored

cd ..

REM Setup Frontend
echo.
echo ▶ Setting up frontend...
cd frontend

if not exist ".env.development" (
    copy .env.example .env.development
    echo ✓ Created .env.development
)

echo ▶ Installing frontend dependencies...
call npm ci
if %errorlevel% neq 0 (
    echo ⚠ Failed to install frontend dependencies
    exit /b 1
)
echo ✓ Frontend dependencies installed

cd ..

REM Create .vscode directory if not exists
if not exist ".vscode" mkdir .vscode

REM Create VS Code settings
echo ▶ Creating VS Code settings...
(
    echo {
    echo   "editor.defaultFormatter": "esbenp.prettier-vscode",
    echo   "[javascript]": {
    echo     "editor.defaultFormatter": "esbenp.prettier-vscode"
    echo   },
    echo   "[typescript]": {
    echo     "editor.defaultFormatter": "esbenp.prettier-vscode"
    echo   },
    echo   "[json]": {
    echo     "editor.defaultFormatter": "esbenp.prettier-vscode"
    echo   },
    echo   "[csharp]": {
    echo     "editor.defaultFormatter": "ms-dotnettools.csharp"
    echo   },
    echo   "editor.formatOnSave": true,
    echo   "editor.codeActionsOnSave": {
    echo     "source.organizeImports": true
    echo   },
    echo   "typescript.tsdk": "frontend/node_modules/typescript/lib",
    echo   "dotnet.defaultSolutionOrFolder": "backend"
    echo }
) > .vscode\settings.json
echo ✓ VS Code settings created

echo.
echo ✓ Setup complete!
echo.
echo Next steps:
echo   1. Open VS Code: code .
echo   2. Terminal 1 - Start backend: cd backend ^&^& dotnet run
echo   3. Terminal 2 - Start frontend: cd frontend ^&^& npm run dev
echo.
echo Access the application:
echo   Frontend:  http://localhost:5173
echo   API:       http://localhost:5000
echo   Swagger:   http://localhost:5000/swagger
echo.
pause
