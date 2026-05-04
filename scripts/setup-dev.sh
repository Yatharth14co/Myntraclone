#!/bin/bash

# E-Commerce System Development Setup Script
# This script sets up the development environment for both backend and frontend

set -e

echo "🚀 E-Commerce System Development Setup"
echo "======================================"

# Colors for output
GREEN='\033[0;32m'
BLUE='\033[0;34m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Function to print colored output
print_step() {
    echo -e "${BLUE}▶${NC} $1"
}

print_success() {
    echo -e "${GREEN}✓${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}⚠${NC} $1"
}

# Check prerequisites
print_step "Checking prerequisites..."

# Check .NET SDK
if ! command -v dotnet &> /dev/null; then
    print_warning ".NET SDK not found. Please install .NET 8 SDK"
    exit 1
fi
DOTNET_VERSION=$(dotnet --version | cut -d. -f1)
if [ "$DOTNET_VERSION" -lt 8 ]; then
    print_warning ".NET 8+ required. Current version: $(dotnet --version)"
    exit 1
fi
print_success ".NET SDK $(dotnet --version) installed"

# Check Node.js
if ! command -v node &> /dev/null; then
    print_warning "Node.js not found. Please install Node.js 18+"
    exit 1
fi
NODE_VERSION=$(node -v | cut -d'v' -f2 | cut -d'.' -f1)
if [ "$NODE_VERSION" -lt 18 ]; then
    print_warning "Node.js 18+ required. Current version: $(node -v)"
    exit 1
fi
print_success "Node.js $(node -v) installed"

# Check Git
if ! command -v git &> /dev/null; then
    print_warning "Git not found. Please install Git"
    exit 1
fi
print_success "Git $(git --version | cut -d' ' -f3) installed"

# Setup Backend
print_step "Setting up backend..."
cd backend

if [ ! -f "appsettings.Development.json" ]; then
    cp appsettings.json appsettings.Development.json
    print_success "Created appsettings.Development.json"
fi

print_step "Restoring backend dependencies..."
dotnet restore
print_success "Backend dependencies restored"

cd ..

# Setup Frontend
print_step "Setting up frontend..."
cd frontend

if [ ! -f ".env.development" ]; then
    cp .env.example .env.development
    print_success "Created .env.development"
fi

print_step "Installing frontend dependencies..."
npm ci
print_success "Frontend dependencies installed"

cd ..

# Create .vscode settings
print_step "Creating VS Code settings..."
cat > .vscode/settings.json << 'EOF'
{
  "editor.defaultFormatter": "esbenp.prettier-vscode",
  "[javascript]": {
    "editor.defaultFormatter": "esbenp.prettier-vscode"
  },
  "[typescript]": {
    "editor.defaultFormatter": "esbenp.prettier-vscode"
  },
  "[json]": {
    "editor.defaultFormatter": "esbenp.prettier-vscode"
  },
  "[csharp]": {
    "editor.defaultFormatter": "ms-dotnettools.csharp"
  },
  "editor.formatOnSave": true,
  "editor.codeActionsOnSave": {
    "source.organizeImports": true
  },
  "typescript.tsdk": "frontend/node_modules/typescript/lib",
  "dotnet.defaultSolutionOrFolder": "backend"
}
EOF
print_success "VS Code settings created"

# Create launch configuration
print_step "Creating VS Code launch configuration..."
mkdir -p .vscode
cat > .vscode/launch.json << 'EOF'
{
  "version": "0.2.0",
  "configurations": [
    {
      "name": ".NET Backend",
      "type": "coreclr",
      "request": "launch",
      "program": "${workspaceFolder}/backend/bin/Debug/net8.0/ECommerceApi.dll",
      "args": [],
      "cwd": "${workspaceFolder}/backend",
      "preLaunchTask": "build-backend",
      "stopAtEntry": false,
      "serverReadyAction": {
        "action": "openExternally",
        "pattern": "\\bNow listening on:\\s+(https?://\\S+)",
        "uriFormat": "$1"
      }
    },
    {
      "name": "Frontend - Chrome",
      "type": "chrome",
      "request": "launch",
      "url": "http://localhost:5173",
      "webRoot": "${workspaceFolder}/frontend/src",
      "preLaunchTask": "dev-frontend"
    }
  ]
}
EOF
print_success "VS Code launch configuration created"

echo ""
echo -e "${GREEN}✓ Setup complete!${NC}"
echo ""
echo "Next steps:"
echo "  1. Open VS Code: code ."
echo "  2. Terminal 1 - Start backend: cd backend && dotnet run"
echo "  3. Terminal 2 - Start frontend: cd frontend && npm run dev"
echo ""
echo "Access the application:"
echo "  Frontend:  http://localhost:5173"
echo "  API:       http://localhost:5000"
echo "  Swagger:   http://localhost:5000/swagger"
echo ""
