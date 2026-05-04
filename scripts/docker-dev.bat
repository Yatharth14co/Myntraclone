@echo off
REM Start the development environment with Docker Compose

echo.
echo 🚀 Starting E-Commerce Development Environment with Docker
echo =========================================================

REM Check if Docker is installed
docker --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ❌ Docker is not installed. Please install Docker from https://docker.com
    exit /b 1
)

REM Check if Docker Compose is installed
docker-compose --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ❌ Docker Compose is not installed. Please install Docker Compose
    exit /b 1
)

REM Navigate to deploy directory
cd deploy

echo 📦 Building and starting containers...
call docker-compose up --build

echo.
echo ✓ Containers started successfully!
echo.
echo Access the application:
echo   Frontend:  http://localhost:3000
echo   API:       http://localhost:5000
echo   Swagger:   http://localhost:5000/swagger
echo   Redis:     localhost:6379
echo.
echo To stop, press Ctrl+C
