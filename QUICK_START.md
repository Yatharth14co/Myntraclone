# ⚡ Quick Reference Guide

## 🚀 Get Started in 60 Seconds

### Option A: Auto Setup (Recommended)
```bash
# Windows
.\scripts\setup-dev.bat

# Linux/macOS
bash scripts/setup-dev.sh
```

### Option B: Docker
```bash
cd deploy && docker-compose up --build
```

### Option C: Manual
```bash
# Terminal 1
cd backend && dotnet run

# Terminal 2
cd frontend && npm install && npm run dev
```

**Access:**
- Frontend: http://localhost:5173
- API: http://localhost:5000
- Swagger: http://localhost:5000/swagger

---

## 📂 Where to Find Things

| What | Where |
|------|-------|
| **API Endpoints** | `/backend/Controllers/` |
| **Database Models** | `/backend/Models/` |
| **Business Logic** | `/backend/Services/` |
| **React Components** | `/frontend/src/components/` |
| **Pages** | `/frontend/src/pages/` |
| **API Client** | `/frontend/src/services/api.ts` |
| **State Store** | `/frontend/src/store/store.ts` |
| **Quick Start** | `docs/SETUP.md` |
| **Troubleshooting** | `docs/TROUBLESHOOTING.md` |
| **Tests** | `/backend/Tests/` |

---

## 🎯 Common Tasks

### Starting Development
```bash
# Backend: Terminal 1
cd backend && dotnet run

# Frontend: Terminal 2
cd frontend && npm run dev
```

### Running Tests
```bash
cd backend && dotnet test
```

### Building for Production
```bash
# Backend
cd backend && dotnet publish -c Release

# Frontend
cd frontend && npm run build
```

### Docker Operations
```bash
# Start
cd deploy && docker-compose up --build

# Stop
docker-compose down

# View logs
docker-compose logs -f
```

### Using Make
```bash
make setup          # One-time setup
make backend-run    # Run backend
make frontend-run   # Run frontend
make test           # Run tests
make docker-up      # Start Docker
make help           # All commands
```

---

## 🔧 Configuration

### Backend Settings
```json
// appsettings.json (Production)
// appsettings.Development.json (Development)
{
  "ConnectionStrings": "...",
  "AppSettings": {
    "JwtSecret": "...",
    "RateLimit": "..."
  }
}
```

### Frontend Environment
```bash
# .env.development
VITE_API_BASE_URL=http://localhost:5000/api
```

---

## 📚 Documentation

| Guide | Link | Read Time |
|-------|------|-----------|
| Quick Start | `docs/SETUP.md` | 10 min |
| Testing | `docs/TESTING.md` | 15 min |
| Troubleshooting | `docs/TROUBLESHOOTING.md` | 20 min |
| Architecture | `docs/ARCHITECTURE.md` | 15 min |
| Deployment | `docs/DEPLOYMENT.md` | 15 min |
| API Reference | `docs/API_TESTING.md` | 10 min |
| **All Guides** | `docs/` | 85 min |

---

## 🔍 File Structure Reference

```
backend/
├── Controllers/     ← API endpoints
├── Models/          ← Database entities
├── Services/        ← Business logic
├── Repositories/    ← Data access
├── DTOs/            ← Data transfer objects
├── Middleware/      ← Request processing
├── Data/            ← Database context
├── Exceptions/      ← Custom exceptions
├── Tests/           ← Unit tests
└── Program.cs       ← Startup

frontend/
├── src/
│   ├── components/  ← Reusable components
│   ├── pages/       ← Page components
│   ├── services/    ← API calls
│   ├── store/       ← State management
│   ├── App.tsx      ← Root component
│   └── main.tsx     ← Entry point
└── public/          ← Static files

docs/
├── SETUP.md         ← Getting started
├── TESTING.md       ← Test guide
├── TROUBLESHOOTING.md ← Common issues
├── ARCHITECTURE.md  ← System design
└── ...

.vscode/
├── settings.json    ← Editor config
├── launch.json      ← Debug config
├── tasks.json       ← Build tasks
└── extensions.json  ← Extensions

scripts/
├── setup-dev.sh/bat ← Setup automation
└── docker-dev.sh/bat ← Docker launcher
```

---

## 🎓 Key Technologies

| Component | Tech | Version |
|-----------|------|---------|
| Backend | .NET Core | 8.0 |
| API | ASP.NET Core | 8.0 |
| Frontend | React | 18.2 |
| Language | TypeScript | 5.0+ |
| Build Tool | Vite | 5.0+ |
| State | Zustand | Latest |
| Container | Docker | Latest |
| Orchestration | Docker Compose | 3.8+ |

---

## 🆘 Troubleshooting Quick Links

| Problem | Solution |
|---------|----------|
| Port in use | `docs/TROUBLESHOOTING.md` → "Port Already in Use" |
| DB connection | `docs/TROUBLESHOOTING.md` → "Database Connection Failed" |
| Dependencies | `docs/TROUBLESHOOTING.md` → "Node Modules Issues" |
| CORS errors | `docs/TROUBLESHOOTING.md` → "CORS Errors" |
| 500 errors | `docs/TROUBLESHOOTING.md` → "Internal Server Error" |
| Docker issues | `docs/TROUBLESHOOTING.md` → "Docker Issues" |

---

## 💡 Pro Tips

1. **Use the scripts**: `setup-dev.bat` (Windows) or `setup-dev.sh` (Linux/macOS)
2. **Check Swagger**: http://localhost:5000/swagger for API docs
3. **Watch mode**: Use `dotnet watch run` for auto-reload
4. **Hot reload**: Frontend has hot reload with Vite
5. **Debug**: Use VS Code breakpoints with `.vscode/launch.json`
6. **Make commands**: Run `make` or `make help` for tasks
7. **Docker logs**: Use `docker-compose logs -f` to debug

---

## ✅ Verification Steps

After setup, verify everything works:

```bash
# 1. Check backend is running
curl http://localhost:5000/swagger

# 2. Check frontend is running
curl http://localhost:5173

# 3. Test API
curl http://localhost:5000/api/products

# 4. Run tests
cd backend && dotnet test
```

---

## 📖 Documentation Index

- **Getting Started**: `docs/SETUP.md`
- **API Testing**: `docs/API_TESTING.md`
- **Architecture**: `docs/ARCHITECTURE.md`
- **Testing**: `docs/TESTING.md`
- **Deployment**: `docs/DEPLOYMENT.md`
- **Troubleshooting**: `docs/TROUBLESHOOTING.md`
- **Contributing**: `CONTRIBUTING.md`
- **Workspace Info**: `WORKSPACE.md`
- **Completion Info**: `COMPLETION_SUMMARY.md`

---

## 🎯 Next Steps

1. **Run setup**: `.\scripts\setup-dev.bat` (Windows)
2. **Start services**: `cd backend && dotnet run` + `cd frontend && npm run dev`
3. **Open browser**: http://localhost:5173
4. **Read docs**: Start with `docs/SETUP.md`
5. **Start coding**: Happy developing! 🚀

---

**Last Updated**: May 5, 2026  
**Project Status**: ✅ Production Ready
