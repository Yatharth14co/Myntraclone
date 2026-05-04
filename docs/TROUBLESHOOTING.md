# Troubleshooting Guide

## Common Issues and Solutions

### Backend Issues

#### 1. Port Already in Use

**Problem:** "Address already in use" error

**Solution:**
```bash
# Find process using port 5000
netstat -ano | findstr :5000

# Kill the process (Windows)
taskkill /PID <PID> /F

# Or change port in launchSettings.json
```

#### 2. Database Connection Failed

**Problem:** "Cannot connect to database"

**Solution:**
- Verify SQL Server is running
- Check connection string in `appsettings.Development.json`
- For in-memory database, ensure no other instance is running

#### 3. EF Core Migrations Not Applied

**Problem:** "Database schema mismatch"

**Solution:**
```bash
# Add migration
dotnet ef migrations add MigrationName

# Update database
dotnet ef database update

# View migrations
dotnet ef migrations list
```

#### 4. NuGet Package Restore Fails

**Problem:** "Unable to resolve dependency"

**Solution:**
```bash
# Clear NuGet cache
dotnet nuget locals all --clear

# Restore packages
dotnet restore

# Or update packages
dotnet package update
```

### Frontend Issues

#### 1. Node Modules Issues

**Problem:** "node_modules corruption" or missing dependencies

**Solution:**
```bash
# Clean install
rm -r node_modules package-lock.json
npm install

# Or use npm ci for CI/CD
npm ci
```

#### 2. Port 5173 Already in Use

**Problem:** Development server won't start

**Solution:**
```bash
# Use different port
npm run dev -- --port 3000

# Or kill existing process
npx kill-port 5173
```

#### 3. Environment Variables Not Loading

**Problem:** API requests fail or go to wrong endpoint

**Solution:**
- Ensure `.env.development` exists
- Verify `VITE_API_BASE_URL` is set correctly
- Restart dev server after changing .env files

#### 4. TypeScript Compilation Errors

**Problem:** Type errors in IDE or build

**Solution:**
```bash
# Update TypeScript
npm install --save-dev typescript@latest

# Generate type definitions
npm run build -- --emitDeclarationOnly

# Check for type errors
npx tsc --noEmit
```

### API Connection Issues

#### 1. CORS Errors

**Problem:** "Access to XMLHttpRequest blocked by CORS policy"

**Solution:**
- Backend CORS is configured in `Program.cs`
- Verify frontend URL matches allowed origins
- Check browser DevTools Network tab

#### 2. 404 Not Found

**Problem:** API endpoints return 404

**Solution:**
- Verify endpoint paths in controllers
- Check API version prefix (e.g., `/api/v1/`)
- Ensure controllers are properly registered

#### 3. 500 Internal Server Error

**Problem:** Server returns 500 error

**Solution:**
- Check backend console for error details
- Review application logs
- Verify request payload matches schema
- Check database connectivity

### Docker Issues

#### 1. Container Won't Start

**Problem:** Docker container exits immediately

**Solution:**
```bash
# Check logs
docker logs <container_id>

# Build fresh image
docker build --no-cache -t ecommerce-app .

# Run with verbose output
docker run -it ecommerce-app
```

#### 2. Port Mapping Issues

**Problem:** Cannot connect to container

**Solution:**
```bash
# Verify port mapping
docker ps

# Update docker-compose.yml ports
# Rebuild and restart
docker-compose down
docker-compose up --build
```

### Performance Issues

#### 1. Slow API Responses

**Problem:** API requests taking too long

**Solution:**
- Check database queries (use profiler)
- Verify indexes are created
- Enable caching (Redis)
- Check network latency

#### 2. High Memory Usage

**Problem:** Application consuming too much memory

**Solution:**
- Profile memory usage
- Check for memory leaks
- Review large object heap
- Optimize LINQ queries

#### 3. Frontend Performance

**Problem:** Slow page loads or interactions

**Solution:**
```bash
# Build analysis
npm run build -- --analyze

# Check bundle size
npm run build -- --report

# Optimize images and assets
```

### Git Issues

#### 1. Large Files Tracking

**Problem:** Repository becoming too large

**Solution:**
- Use `.gitignore` properly
- Remove node_modules, bin, obj folders
- Consider Git LFS for binaries

#### 2. Merge Conflicts

**Problem:** Conflicts when pulling changes

**Solution:**
```bash
# Abort merge
git merge --abort

# Resolve conflicts manually
# Then complete merge
git add .
git commit -m "Resolve merge conflicts"
```

## Getting Help

If your issue isn't listed here:

1. **Check existing issues** on GitHub
2. **Review logs** for error messages
3. **Search documentation** for related topics
4. **Create an issue** with:
   - Detailed error message
   - Steps to reproduce
   - System information
   - Relevant code/logs

## Useful Commands

### Backend
```bash
# Run with detailed logging
dotnet run --verbosity=diagnostic

# Clean and rebuild
dotnet clean && dotnet build

# Run specific test
dotnet test --filter "TestName"
```

### Frontend
```bash
# Clear cache
npm cache clean --force

# Reinstall dependencies
npm ci

# Run build analysis
npm run build -- --report

# Check for security vulnerabilities
npm audit
```

### Docker
```bash
# List running containers
docker ps

# View container logs
docker logs -f <container_id>

# Execute command in container
docker exec -it <container_id> /bin/bash

# Remove unused resources
docker system prune
```
