#!/bin/bash

# Start the development environment with Docker Compose

set -e

echo "🚀 Starting E-Commerce Development Environment with Docker"
echo "=========================================================="

# Check if Docker is installed
if ! command -v docker &> /dev/null; then
    echo "❌ Docker is not installed. Please install Docker from https://docker.com"
    exit 1
fi

# Check if Docker Compose is installed
if ! command -v docker-compose &> /dev/null; then
    echo "❌ Docker Compose is not installed. Please install Docker Compose"
    exit 1
fi

# Navigate to deploy directory
cd deploy

echo "📦 Building and starting containers..."
docker-compose up --build

echo "✓ Containers started successfully!"
echo ""
echo "Access the application:"
echo "  Frontend:  http://localhost:3000"
echo "  API:       http://localhost:5000"
echo "  Swagger:   http://localhost:5000/swagger"
echo "  Redis:     localhost:6379"
echo ""
echo "To stop, press Ctrl+C"
