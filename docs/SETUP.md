# Quick Start Guide

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 18+](https://nodejs.org/)
- [Git](https://git-scm.com/)
- SQL Server or use in-memory database for development

## Backend Setup

### 1. Navigate to backend directory
```bash
cd backend
```

### 2. Restore dependencies
```bash
dotnet restore
```

### 3. Configure database (optional)
By default, the app uses an in-memory database. To use SQL Server:
- Update `appsettings.Development.json` with your connection string
- Run migrations: `dotnet ef database update`

### 4. Run the application
```bash
dotnet run
```

The API will be available at `https://localhost:5001` or `http://localhost:5000`

### 5. View API Documentation
Open Swagger UI at: `http://localhost:5000/swagger`

## Frontend Setup

### 1. Navigate to frontend directory
```bash
cd frontend
```

### 2. Install dependencies
```bash
npm install
```

### 3. Configure environment
Copy `.env.example` to `.env.development`:
```bash
cp .env.example .env.development
```

### 4. Start development server
```bash
npm run dev
```

The frontend will be available at `http://localhost:5173`

## Running Both Services

You can run both services simultaneously:

**Terminal 1 - Backend:**
```bash
cd backend
dotnet run
```

**Terminal 2 - Frontend:**
```bash
cd frontend
npm run dev
```

## Building for Production

### Backend
```bash
cd backend
dotnet publish -c Release -o ./publish
```

### Frontend
```bash
cd frontend
npm run build
```

## Docker Setup

Build and run with Docker:
```bash
docker build -t ecommerce-app .
docker run -p 5000:5000 ecommerce-app
```

## Troubleshooting

See [TROUBLESHOOTING.md](TROUBLESHOOTING.md) for common issues and solutions.

## Next Steps

- Read the [API Testing Guide](API_TESTING.md)
- Review the [Architecture Documentation](ARCHITECTURE.md)
- Check out [Contributing Guidelines](../CONTRIBUTING.md)
