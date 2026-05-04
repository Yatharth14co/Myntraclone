.PHONY: help setup clean build run test docker-up docker-down

# Colors for output
BLUE := \033[0;34m
GREEN := \033[0;32m
YELLOW := \033[1;33m
NC := \033[0m # No Color

help: ## Show this help message
	@echo "$(BLUE)E-Commerce System - Makefile Commands$(NC)"
	@echo "========================================"
	@echo ""
	@grep -E '^[a-zA-Z_-]+:.*?## .*$$' $(MAKEFILE_LIST) | sort | awk 'BEGIN {FS = ":.*?## "}; {printf "$(GREEN)%-20s$(NC) %s\n", $$1, $$2}'

setup: ## Setup development environment
	@echo "$(BLUE)Setting up development environment...$(NC)"
	@cd backend && dotnet restore
	@cd frontend && npm ci
	@echo "$(GREEN)✓ Setup complete$(NC)"

clean: ## Clean build artifacts
	@echo "$(BLUE)Cleaning build artifacts...$(NC)"
	@cd backend && dotnet clean
	@cd frontend && rm -rf node_modules dist
	@echo "$(GREEN)✓ Clean complete$(NC)"

build: ## Build backend and frontend
	@echo "$(BLUE)Building backend...$(NC)"
	@cd backend && dotnet build
	@echo "$(BLUE)Building frontend...$(NC)"
	@cd frontend && npm run build
	@echo "$(GREEN)✓ Build complete$(NC)"

run: ## Run backend and frontend (requires two terminals)
	@echo "$(YELLOW)⚠ Run in two separate terminals:$(NC)"
	@echo "  Terminal 1: cd backend && dotnet run"
	@echo "  Terminal 2: cd frontend && npm run dev"

backend-run: ## Run backend only
	@echo "$(BLUE)Starting backend...$(NC)"
	@cd backend && dotnet run

frontend-run: ## Run frontend only
	@echo "$(BLUE)Starting frontend...$(NC)"
	@cd frontend && npm run dev

test: ## Run all tests
	@echo "$(BLUE)Running tests...$(NC)"
	@cd backend && dotnet test
	@echo "$(GREEN)✓ Tests complete$(NC)"

test-backend: ## Run backend tests only
	@echo "$(BLUE)Running backend tests...$(NC)"
	@cd backend && dotnet test

test-watch: ## Run tests in watch mode
	@echo "$(BLUE)Running tests in watch mode...$(NC)"
	@cd backend && dotnet watch test

lint: ## Lint frontend code
	@echo "$(BLUE)Linting frontend...$(NC)"
	@cd frontend && npm run lint

format: ## Format code
	@echo "$(BLUE)Formatting backend...$(NC)"
	@cd backend && dotnet format
	@echo "$(BLUE)Formatting frontend...$(NC)"
	@cd frontend && npx prettier --write src

docker-build: ## Build Docker image
	@echo "$(BLUE)Building Docker image...$(NC)"
	@docker build -t ecommerce-app:latest .
	@echo "$(GREEN)✓ Docker build complete$(NC)"

docker-up: ## Start Docker Compose services
	@echo "$(BLUE)Starting Docker Compose services...$(NC)"
	@cd deploy && docker-compose up --build
	@echo "$(GREEN)✓ Services started$(NC)"

docker-down: ## Stop Docker Compose services
	@echo "$(BLUE)Stopping Docker Compose services...$(NC)"
	@cd deploy && docker-compose down
	@echo "$(GREEN)✓ Services stopped$(NC)"

docker-logs: ## View Docker Compose logs
	@cd deploy && docker-compose logs -f

docker-clean: ## Remove all Docker containers and images
	@echo "$(YELLOW)⚠ Removing Docker containers and images...$(NC)"
	@docker-compose -f deploy/docker-compose.yml down -v
	@docker rmi ecommerce-app:latest 2>/dev/null || true
	@echo "$(GREEN)✓ Docker cleanup complete$(NC)"

migrate: ## Run database migrations
	@echo "$(BLUE)Running database migrations...$(NC)"
	@cd backend && dotnet ef database update
	@echo "$(GREEN)✓ Migrations complete$(NC)"

seed: ## Seed database with sample data
	@echo "$(BLUE)Seeding database...$(NC)"
	@cd backend && dotnet run -- seed
	@echo "$(GREEN)✓ Database seeded$(NC)"

install: ## Install dependencies
	@echo "$(BLUE)Installing dependencies...$(NC)"
	@cd backend && dotnet restore
	@cd frontend && npm install
	@echo "$(GREEN)✓ Dependencies installed$(NC)"

update: ## Update dependencies
	@echo "$(BLUE)Updating dependencies...$(NC)"
	@cd backend && dotnet nuget update
	@cd frontend && npm update
	@echo "$(GREEN)✓ Dependencies updated$(NC)"

dev-setup: ## Run setup script
	@echo "$(BLUE)Running development setup...$(NC)"
	@bash scripts/setup-dev.sh

.DEFAULT_GOAL := help
