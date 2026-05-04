# 🛒 E-Commerce Smart Cart & Checkout System

> Production-grade full-stack e-commerce application with smart cart management, flexible coupon system, and atomic checkout processing. Built with .NET 8, React, TypeScript, and modern DevOps practices.

![Status](https://img.shields.io/badge/Status-Production%20Ready-brightgreen)
![License](https://img.shields.io/badge/License-MIT-blue)
![.NET Version](https://img.shields.io/badge/.NET-8.0-blue)
![Node Version](https://img.shields.io/badge/Node-18+-green)

## 📋 Quick Navigation

- [Overview](#-overview)
- [Quick Start](#-quick-start)
- [Project Structure](#-project-structure)
- [Tech Stack](#-tech-stack)
- [Architecture](#-architecture)
- [Features](#-features)
- [API Documentation](#-api-documentation)
- [Testing](#-testing)
- [Deployment](#-deployment)
- [Contributing](#-contributing)
- [Support](#-support)

---

## 🎯 Overview

A complete, production-ready e-commerce system demonstrating enterprise architecture patterns, comprehensive testing, and modern DevOps practices. The system handles product management, intelligent cart operations, flexible coupon management, and atomic checkout processing.

### ✨ Key Highlights

- **Smart Cart** - Real-time inventory validation and management
- **Flexible Coupons** - Flat and percentage-based discount support
- **Atomic Transactions** - Reliable order processing with stock reduction
- **Performance** - Redis caching for optimized responses
- **Security** - Rate limiting, input validation, exception handling
- **Testing** - Comprehensive unit and integration test suites
- **API First** - OpenAPI/Swagger documentation
- **Containerized** - Docker & Docker Compose ready
- **CI/CD** - Automated GitHub Actions pipeline
- **Developer Friendly** - Clear structure, detailed documentation

---

## 🚀 Quick Start

### Requirements
- [.NET 8 SDK](https://dotnet.microsoft.com/download) or Docker
- [Node.js 18+](https://nodejs.org/) or Docker
- SQL Server (optional, uses in-memory by default)
- Docker & Docker Compose (for containerized setup)

### 60-Second Setup

**Option 1: Docker (Recommended)**
```bash
cd deploy && docker-compose up --build
```

**Option 2: Native Development**

Terminal 1 (Backend):
```bash
cd backend && dotnet restore && dotnet run
```

Terminal 2 (Frontend):
```bash
cd frontend && npm install && npm run dev
```

**Option 3: Automated Script**
```bash
# Windows
.\scripts\setup-dev.bat

# Linux/macOS
bash scripts/setup-dev.sh
```

**Access Points:**
- 🌐 Frontend: http://localhost:5173
- 🔌 API: http://localhost:5000
- 📚 Swagger: http://localhost:5000/swagger
- 🔴 Redis: localhost:6379 (if using Docker)

For detailed setup, see [Quick Start Guide](docs/SETUP.md)

---

## 📂 Project Structure

```
ecommerce-system/
├── 📁 backend/                      # .NET 8 API
│   ├── Controllers/                 # API endpoints
│   ├── Models/                      # Domain entities
│   ├── Services/                    # Business logic
│   ├── Repositories/                # Data access
│   ├── DTOs/                        # Data transfer objects
│   ├── Middleware/                  # Request/response handling
│   ├── Exceptions/                  # Custom exceptions
│   ├── Data/                        # Database context & seeding
│   ├── Configuration/               # App configuration
│   ├── Tests/                       # Unit tests
│   ├── Program.cs                   # Startup configuration
│   ├── ECommerceApi.csproj          # Project file
│   ├── appsettings.json             # Configuration
│   └── appsettings.Development.json # Dev settings
│
├── 📁 frontend/                     # React + TypeScript
│   ├── src/
│   │   ├── components/              # Reusable components
│   │   │   ├── ProductCard.tsx
│   │   │   └── CouponForm.tsx
│   │   ├── pages/                   # Page components
│   │   │   ├── ProductsPage.tsx
│   │   │   └── CartPage.tsx
│   │   ├── services/                # API integration
│   │   │   └── api.ts
│   │   ├── store/                   # State management (Zustand)
│   │   │   └── store.ts
│   │   ├── App.tsx                  # Root component
│   │   └── main.tsx                 # Entry point
│   ├── public/                      # Static assets
│   ├── index.html                   # HTML template
│   ├── package.json                 # Dependencies
│   ├── tsconfig.json                # TypeScript config
│   ├── vite.config.ts               # Build config
│   └── .env.example                 # Example environment
│
├── 📁 deploy/                       # Deployment configs
│   └── docker-compose.yml           # Multi-container setup
│
├── 📁 .github/workflows/            # CI/CD pipeline
│   └── ci-cd.yml                    # GitHub Actions
│
├── 📁 docs/                         # Documentation
│   ├── SETUP.md                     # Setup guide
│   ├── TESTING.md                   # Testing guide
│   ├── ARCHITECTURE.md              # System design
│   ├── DEPLOYMENT.md                # Deployment guide
│   ├── API_TESTING.md               # API reference
│   ├── INDEX.md                     # Project index
│   └── PROJECT_SUMMARY.md           # Summary
│
├── 📄 README.md                     # This file
├── 📄 CONTRIBUTING.md               # Contribution guide
├── 📄 LICENSE                       # MIT License
├── 📄 Dockerfile                    # Container image
├── 📄 .gitignore                    # Git ignore rules
├── 📄 .dockerignore                 # Docker ignore rules
├── 📄 .editorconfig                 # Editor settings
└── 📄 .env.example                  # Example environment

---

## 🔧 Tech Stack

### Backend
| Component | Technology | Purpose |
|-----------|-----------|---------|
| **Runtime** | .NET 8 | Enterprise framework |
| **API** | ASP.NET Core | RESTful API |
| **Database** | SQL Server / In-Memory | Data persistence |
| **ORM** | Entity Framework Core 8 | Database access |
| **Cache** | Redis | Performance |
| **Testing** | xUnit + Moq | Unit testing |

### Frontend
| Component | Technology | Purpose |
|-----------|-----------|---------|
| **Framework** | React 18 | UI library |
| **Language** | TypeScript | Type safety |
| **Build** | Vite | Fast bundler |
| **State** | Zustand | State management |
| **HTTP** | Fetch API | API calls |
| **Styling** | CSS Modules | Component styling |

### DevOps
| Component | Technology | Purpose |
|-----------|-----------|---------|
| **Containerization** | Docker | Application containerization |
| **Orchestration** | Docker Compose | Multi-container management |
| **CI/CD** | GitHub Actions | Automated pipelines |
| **Version Control** | Git | Code management |

---

## 🏗️ Architecture

### System Architecture

```
┌─────────────────────────────────────┐
│      Presentation Layer             │
│  (Controllers, DTOs, Validators)    │
├─────────────────────────────────────┤
│      Business Logic Layer           │
│     (Services, Repositories)        │
├─────────────────────────────────────┤
│      Data Access Layer              │
│  (EF Core, DbContext, Models)       │
├─────────────────────────────────────┤
│      Infrastructure Layer           │
│  (Middleware, Caching, Logging)     │
└─────────────────────────────────────┘
```

### Frontend Architecture

```
┌──────────────────────────────┐
│     React Components         │
│  (Pages, Components, Hooks)  │
├──────────────────────────────┤
│    State Management (Zustand)│
├──────────────────────────────┤
│      API Service Layer       │
├──────────────────────────────┤
│    HTTP Client (Axios)       │
└──────────────────────────────┘
```

---

## 🛠️ Tech Stack

### Backend
- **.NET 8** - Latest LTS framework
- **ASP.NET Core** - Web API framework
- **Entity Framework Core** - ORM with In-Memory and SQL Server support
- **FluentValidation** - Input validation
- **StackExchange.Redis** - Caching layer
- **xUnit** - Unit testing framework
- **Moq** - Mocking library
- **Swagger/OpenAPI** - API documentation

### Frontend
- **React 18** - UI library
- **TypeScript** - Type safety
- **Zustand** - State management
- **Axios** - HTTP client
- **Tailwind CSS** - Styling
- **Vite** - Build tool
- **Jest** - Testing framework

### DevOps
- **Docker** - Containerization
- **Docker Compose** - Container orchestration
- **GitHub Actions** - CI/CD automation
- **Redis** - Caching service

---

## 📁 Project Structure

```
.
├── backend/
│   ├── Controllers/           # API endpoints
│   ├── Services/              # Business logic
│   ├── Repositories/          # Data access
│   ├── Models/                # Domain entities
│   ├── DTOs/                  # Data transfer objects
│   ├── Validators/            # Input validation
│   ├── Middleware/            # Custom middleware
│   ├── Exceptions/            # Custom exceptions
│   ├── Data/                  # Database context and seeding
│   ├── Tests/                 # Unit tests
│   ├── ECommerceApi.csproj    # Project file
│   ├── Program.cs             # Application startup
│   └── appsettings.json       # Configuration
│
├── frontend/
│   ├── src/
│   │   ├── pages/             # Page components
│   │   ├── components/        # Reusable components
│   │   ├── services/          # API service
│   │   ├── store/             # State management
│   │   ├── types/             # TypeScript types
│   │   ├── hooks/             # Custom React hooks
│   │   ├── App.tsx            # Root component
│   │   ├── main.tsx           # Entry point
│   │   └── index.css          # Global styles
│   ├── public/                # Static assets
│   ├── package.json           # Dependencies
│   ├── tsconfig.json          # TypeScript config
│   ├── vite.config.ts         # Vite config
│   └── Dockerfile             # Container image
│
├── .github/
│   └── workflows/
│       └── ci-cd.yml          # GitHub Actions pipeline
│
├── docker-compose.yml         # Multi-container setup
└── README.md                  # This file
```

---

## 📦 Prerequisites

### Required
- **.NET 8 SDK** - [Download](https://dotnet.microsoft.com/download/dotnet/8.0)
- **Node.js 18+** - [Download](https://nodejs.org/)
- **Docker & Docker Compose** - [Download](https://www.docker.com/products/docker-desktop)
- **Git** - [Download](https://git-scm.com/)

### Optional
- **Visual Studio Code** - [Download](https://code.visualstudio.com/)
- **Postman** - For API testing
- **Redis Desktop Manager** - For caching inspection

---

## 🚀 Setup Instructions

### 1. Clone the Repository

```bash
git clone https://github.com/yourusername/ecommerce-system.git
cd ecommerce-system
```

### 2. Backend Setup

#### Option A: Local Development

```bash
cd backend

# Restore NuGet packages
dotnet restore

# Build the project
dotnet build

# Run migrations (if using SQL Server)
# dotnet ef database update

# Run the application
dotnet run
```

The backend will start at `http://localhost:5000`

#### Option B: Using Docker

```bash
docker-compose up backend
```

### 3. Frontend Setup

#### Option A: Local Development

```bash
cd frontend

# Install dependencies
npm install

# Start development server
npm run dev
```

The frontend will start at `http://localhost:5173`

#### Option B: Build for Production

```bash
cd frontend

npm install
npm run build
```

### 4. Run Complete Stack with Docker Compose

```bash
# Start all services
docker-compose up

# Or run in background
docker-compose up -d

# Stop services
docker-compose down
```

Access the application:
- **Frontend**: http://localhost:3000
- **Backend API**: http://localhost:5000
- **Swagger UI**: http://localhost:5000

---

## 📡 Running the Application

### Development Mode

#### Terminal 1 - Backend
```bash
cd backend
dotnet run
```

#### Terminal 2 - Frontend
```bash
cd frontend
npm run dev
```

#### Terminal 3 - Redis (Optional, for caching)
```bash
docker run -d -p 6379:6379 redis:7-alpine
```

### Production Mode

```bash
docker-compose -f docker-compose.yml up -d
```

---

## 🔌 API Endpoints

### Base URL
```
http://localhost:5000/api/v1
```

### Products

#### Get All Products (Paginated)
```
GET /products
?pageNumber=1&pageSize=10&searchTerm=laptop

Response: 200 OK
{
  "success": true,
  "message": "Products retrieved successfully",
  "data": {
    "items": [
      {
        "id": 1,
        "name": "Laptop Pro",
        "price": 1299.99,
        "stock": 50,
        "description": "High-performance laptop"
      }
    ],
    "totalCount": 8,
    "pageNumber": 1,
    "pageSize": 10,
    "totalPages": 1
  }
}
```

#### Get Product by ID
```
GET /products/{id}

Response: 200 OK
{
  "success": true,
  "data": {
    "id": 1,
    "name": "Laptop Pro",
    "price": 1299.99,
    "stock": 50,
    "description": "High-performance laptop"
  }
}
```

### Cart

#### Get Cart
```
GET /cart/{cartId}

Response: 200 OK
{
  "success": true,
  "data": {
    "id": 1,
    "items": [
      {
        "id": 1,
        "productId": 1,
        "productName": "Laptop Pro",
        "unitPrice": 1299.99,
        "quantity": 1,
        "lineTotal": 1299.99
      }
    ],
    "couponCode": null,
    "subtotal": 1299.99,
    "discount": 0.00,
    "total": 1299.99
  }
}
```

#### Add/Update Cart Item
```
POST /cart/{cartId}/items

Request Body:
{
  "productId": 1,
  "quantity": 2
}

Response: 200 OK
(Returns updated cart)
```

#### Apply Coupon
```
POST /cart/{cartId}/apply-coupon

Request Body:
{
  "couponCode": "FLAT50"
}

Response: 200 OK
(Returns cart with discount applied)
```

### Orders

#### Checkout
```
POST /orders/checkout/{cartId}

Response: 200 OK
{
  "success": true,
  "data": {
    "orderId": 1,
    "items": [
      {
        "productId": 1,
        "productName": "Laptop Pro",
        "unitPrice": 1299.99,
        "quantity": 1,
        "lineTotal": 1299.99
      }
    ],
    "subtotal": 1299.99,
    "discount": 50.00,
    "tax": 233.99,
    "totalAmount": 1483.98,
    "couponCode": "FLAT50",
    "status": "Confirmed",
    "orderedAt": "2024-05-04T10:30:00Z"
  }
}
```

#### Get Order
```
GET /orders/{orderId}

Response: 200 OK
(Returns order details)
```

---

## ✅ Testing

### Backend Unit Tests

```bash
cd backend

# Run all tests
dotnet test

# Run specific test class
dotnet test --filter "ClassName=CouponServiceTests"

# Run with coverage
dotnet test /p:CollectCoverage=true
```

### Test Coverage

#### Coupon Service Tests
- ✅ Valid flat coupon calculation
- ✅ Expired coupon validation
- ✅ Minimum cart value validation
- ✅ Percentage coupon with max discount cap
- ✅ Discount calculation accuracy

#### Order Service Tests
- ✅ Successful order creation
- ✅ Empty cart validation
- ✅ Insufficient stock handling
- ✅ Atomic transaction processing
- ✅ Order retrieval

#### Cart Service Tests
- ✅ Add item to cart
- ✅ Zero quantity validation
- ✅ Stock availability check
- ✅ Cart retrieval
- ✅ Empty cart coupon rejection

### Frontend Tests

```bash
cd frontend

# Run tests
npm test

# Watch mode
npm test:watch

# Coverage report
npm test -- --coverage
```

---

## 🐳 Docker Deployment

### Build Images

```bash
# Build backend
docker build -f backend/Dockerfile -t ecommerce-backend:latest .

# Build frontend
docker build -f frontend/Dockerfile -t ecommerce-frontend:latest .
```

### Run with Docker Compose

```bash
# Start all services
docker-compose up -d

# View logs
docker-compose logs -f

# Stop services
docker-compose down

# Remove volumes
docker-compose down -v
```

### Environment Variables

Create `.env` file:
```
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__DefaultConnection=Server=sql-server;Database=ECommerceDb;...
ConnectionStrings__Redis=redis:6379
VITE_API_BASE_URL=http://api.example.com/api/v1
```

---

## 🔄 CI/CD Pipeline

### GitHub Actions Workflow

The pipeline performs:

1. **Backend Tests** - Run xUnit tests
2. **Frontend Tests** - Run Jest tests
3. **Security Scan** - Check for vulnerable packages
4. **Code Quality** - Verify code formatting
5. **Build Docker Images** - Create container images
6. **Push to Registry** - Push to GitHub Container Registry

### Trigger

- Push to `main` or `develop` branches
- Pull requests to `main` or `develop`

### View Workflow

```bash
.github/workflows/ci-cd.yml
```

---

## 💰 Business Rules

### Pricing Rules

#### Coupons
| Code | Type | Value | Minimum Cart | Max Discount |
|------|------|-------|--------------|--------------|
| FLAT50 | Flat | ₹50 | ₹500 | - |
| SAVE10 | Percentage | 10% | ₹1000 | ₹200 |
| WELCOME20 | Percentage | 20% | ₹100 | ₹100 |
| FLASH100 | Flat | ₹100 | ₹1500 | - |

#### Tax
- Standard GST: **18%** applied after discount

### Stock Management
- Stock is reduced atomically during checkout
- Prevents negative stock scenarios
- Validates stock availability before order creation
- Rolls back on checkout failure

### Cart Rules
- Cart items must have quantity > 0
- Maximum quantity per item: 1000
- Duplicate products update existing item quantity
- Empty cart cannot be checked out

---

## 📊 Sample API Responses

### Successful Product Listing
```json
{
  "success": true,
  "message": "Products retrieved successfully",
  "data": {
    "items": [
      {
        "id": 1,
        "name": "Laptop Pro",
        "price": 1299.99,
        "stock": 50,
        "description": "High-performance laptop with latest specs"
      },
      {
        "id": 2,
        "name": "Wireless Mouse",
        "price": 29.99,
        "stock": 200,
        "description": "Ergonomic wireless mouse with 2.4GHz connectivity"
      }
    ],
    "totalCount": 8,
    "pageNumber": 1,
    "pageSize": 10,
    "totalPages": 1
  }
}
```

### Coupon Validation Error
```json
{
  "success": false,
  "message": "Coupon is valid only for cart value >= 500",
  "errors": null
}
```

### Insufficient Stock Error
```json
{
  "success": false,
  "message": "Insufficient stock: Insufficient stock. Requested: 100, Available: 10",
  "errors": null
}
```

### Validation Error
```json
{
  "success": false,
  "message": "Validation failed",
  "errors": {
    "Quantity": ["Quantity must be greater than 0"]
  }
}
```

### Successful Checkout
```json
{
  "success": true,
  "message": "Order created successfully",
  "data": {
    "orderId": 1,
    "items": [
      {
        "productId": 1,
        "productName": "Laptop Pro",
        "unitPrice": 1299.99,
        "quantity": 1,
        "lineTotal": 1299.99
      }
    ],
    "subtotal": 1299.99,
    "discount": 50.00,
    "tax": 224.99,
    "totalAmount": 1474.98,
    "couponCode": "FLAT50",
    "status": "Confirmed",
    "orderedAt": "2024-05-04T10:30:00Z"
  }
}
```

---

## ⚡ Performance & Scalability

### Optimization Strategies

1. **Caching**
   - Redis caching for coupon lookups (60-minute TTL)
   - Reduces database queries significantly

2. **Database**
   - Indexed queries for products and coupons
   - In-memory DB for development, SQL Server for production
   - Efficient pagination implementation

3. **API Design**
   - Pagination limits (max 100 items)
   - Minimal data transfer with DTOs
   - Gzip compression support

4. **Rate Limiting**
   - Global: 100 requests/minute per user
   - Checkout: 10 requests/minute (prevents abuse)
   - Cart operations: 50 requests/minute

5. **Frontend**
   - Component memoization
   - Lazy loading of routes
   - Tailwind CSS for minimal bundle size

### Scalability Considerations

- **Horizontal Scaling**: Stateless API design allows load balancing
- **Database Replication**: SQL Server can be replicated across nodes
- **Redis Clustering**: Supports distributed caching
- **Container Orchestration**: Ready for Kubernetes deployment
- **CDN Integration**: Frontend assets can be served via CDN

### Benchmarks (Estimated)

| Operation | Response Time | Throughput |
|-----------|---------------|-----------|
| Get Products | 50-100ms | 5000 req/sec |
| Add to Cart | 80-150ms | 3000 req/sec |
| Apply Coupon (cached) | 10-30ms | 8000 req/sec |
| Checkout | 200-400ms | 1000 req/sec |

---

## 🔐 Security Features

✅ **Input Validation** - FluentValidation for all inputs  
✅ **Rate Limiting** - Prevents brute force attacks  
✅ **Error Handling** - Generic error messages to clients  
✅ **CORS** - Configured for specific origins  
✅ **SQL Injection Prevention** - ORM prevents injection  
✅ **XSS Protection** - React escapes by default  
✅ **HTTPS Ready** - Support for SSL/TLS  

---

## 📝 Sample Data

### Pre-seeded Products
- Laptop Pro - ₹1,299.99
- Wireless Mouse - ₹29.99
- USB-C Hub - ₹49.99
- Mechanical Keyboard - ₹159.99
- 4K Monitor - ₹599.99
- Portable SSD - ₹199.99
- Webcam HD - ₹79.99
- Gaming Headset - ₹129.99

### Available Coupons
- **FLAT50**: ₹50 off (min ₹500)
- **SAVE10**: 10% off, max ₹200 (min ₹1000)
- **WELCOME20**: 20% off, max ₹100 (min ₹100)
- **FLASH100**: ₹100 off (min ₹1500)

---

## 🐛 Troubleshooting

### Backend won't start
```bash
# Clear package cache
dotnet nuget locals all --clear

# Clean and rebuild
dotnet clean
dotnet build
```

### Frontend build errors
```bash
# Clear node modules
rm -rf node_modules package-lock.json
npm install
npm run build
```

### Redis connection issues
```bash
# Check if Redis is running
redis-cli ping

# Restart Redis
docker restart $(docker ps | grep redis | awk '{print $1}')
```

### Docker compose issues
```bash
# Remove all containers and start fresh
docker-compose down -v
docker-compose up --build
```

---

## 📚 Additional Resources

- [.NET 8 Documentation](https://learn.microsoft.com/en-us/dotnet/)
- [React Documentation](https://react.dev/)
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)
- [Docker Documentation](https://docs.docker.com/)
- [GitHub Actions](https://docs.github.com/en/actions)

---

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

---

## 👨‍💻 Author

**Your Name**  
Senior Full-Stack Engineer

---

## 🤝 Contributing

Contributions are welcome! Please follow these steps:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit changes (`git commit -m 'Add amazing feature'`)
4. Push to branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

---

## 📞 Support

For issues or questions:
- Open an issue on GitHub
- Email: support@example.com
- Documentation: See README.md

---

**Last Updated**: May 4, 2024
