# Deployment Guide

## Table of Contents
- [Prerequisites](#prerequisites)
- [Development Deployment](#development-deployment)
- [Production Deployment](#production-deployment)
- [Cloud Deployment](#cloud-deployment)
- [Troubleshooting](#troubleshooting)

---

## Prerequisites

- Docker & Docker Compose installed
- Git repository access
- SSL certificate (for production HTTPS)
- Database backups configured
- Monitoring tools configured

---

## Development Deployment

### Local Development Setup

#### Using Docker Compose

```bash
# Clone repository
git clone <repository-url>
cd ecommerce-system

# Start all services
docker-compose up -d

# View logs
docker-compose logs -f

# Stop services
docker-compose down
```

#### Manual Setup

**Terminal 1 - Backend**
```bash
cd backend
dotnet restore
dotnet run --configuration Debug
```

**Terminal 2 - Frontend**
```bash
cd frontend
npm install
npm run dev
```

**Terminal 3 - Redis**
```bash
docker run -d -p 6379:6379 redis:7-alpine
```

### Access Points
- Frontend: http://localhost:3000 (or http://localhost:5173 without docker)
- Backend API: http://localhost:5000
- Swagger UI: http://localhost:5000
- Redis: localhost:6379

---

## Production Deployment

### 1. Prepare Environment

```bash
# Create production directory
mkdir /opt/ecommerce-prod
cd /opt/ecommerce-prod

# Create .env file
cat > .env << EOF
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:5000
ConnectionStrings__DefaultConnection=Server=YOUR_SQL_SERVER;Database=ECommerceDb;User Id=sa;Password=YOUR_PASSWORD;
ConnectionStrings__Redis=your-redis-host:6379
VITE_API_BASE_URL=https://api.yourdomain.com/api/v1
EOF
```

### 2. Build Docker Images

```bash
# Clone and setup
git clone <repository-url>
cd ecommerce-system

# Build images
docker build -f backend/Dockerfile -t ecommerce-backend:latest .
docker build -f frontend/Dockerfile -t ecommerce-frontend:latest .

# Or push to container registry
docker tag ecommerce-backend:latest myregistry/ecommerce-backend:v1.0.0
docker tag ecommerce-frontend:latest myregistry/ecommerce-frontend:v1.0.0
docker push myregistry/ecommerce-backend:v1.0.0
docker push myregistry/ecommerce-frontend:v1.0.0
```

### 3. Deploy with Docker Compose

```bash
# Production docker-compose
cat > docker-compose.prod.yml << 'EOF'
version: '3.8'

services:
  backend:
    image: ecommerce-backend:latest
    restart: always
    ports:
      - "5000:5000"
    environment:
      ASPNETCORE_ENVIRONMENT: Production
      ConnectionStrings__DefaultConnection: ${DB_CONNECTION}
      ConnectionStrings__Redis: ${REDIS_CONNECTION}
    depends_on:
      - redis
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:5000/swagger"]
      interval: 30s
      timeout: 10s
      retries: 3
    networks:
      - ecommerce-prod

  frontend:
    image: ecommerce-frontend:latest
    restart: always
    ports:
      - "3000:3000"
    environment:
      VITE_API_BASE_URL: ${API_URL}
    depends_on:
      - backend
    networks:
      - ecommerce-prod

  redis:
    image: redis:7-alpine
    restart: always
    ports:
      - "6379:6379"
    volumes:
      - redis-data:/data
    command: redis-server --appendonly yes
    networks:
      - ecommerce-prod

  nginx:
    image: nginx:latest
    restart: always
    ports:
      - "80:80"
      - "443:443"
    volumes:
      - ./nginx.conf:/etc/nginx/nginx.conf
      - ./ssl:/etc/nginx/ssl
    depends_on:
      - backend
      - frontend
    networks:
      - ecommerce-prod

volumes:
  redis-data:

networks:
  ecommerce-prod:
    driver: bridge
EOF

# Deploy
docker-compose -f docker-compose.prod.yml up -d --pull always
```

### 4. Configure Nginx Reverse Proxy

```bash
cat > nginx.conf << 'EOF'
worker_processes auto;
error_log /var/log/nginx/error.log warn;
pid /var/run/nginx.pid;

events {
    worker_connections 1024;
}

http {
    include /etc/nginx/mime.types;
    default_type application/octet-stream;

    log_format main '$remote_addr - $remote_user [$time_local] "$request" '
                    '$status $body_bytes_sent "$http_referer" '
                    '"$http_user_agent" "$http_x_forwarded_for"';

    access_log /var/log/nginx/access.log main;
    sendfile on;
    keepalive_timeout 65;
    gzip on;
    gzip_types text/plain text/css application/json application/javascript;

    # Upstream servers
    upstream backend {
        server backend:5000;
    }

    upstream frontend {
        server frontend:3000;
    }

    server {
        listen 80;
        server_name yourdomain.com www.yourdomain.com;
        
        # Redirect HTTP to HTTPS
        return 301 https://$server_name$request_uri;
    }

    server {
        listen 443 ssl http2;
        server_name yourdomain.com www.yourdomain.com;

        ssl_certificate /etc/nginx/ssl/cert.pem;
        ssl_certificate_key /etc/nginx/ssl/key.pem;
        ssl_protocols TLSv1.2 TLSv1.3;
        ssl_ciphers HIGH:!aNULL:!MD5;

        # API proxy
        location /api/ {
            proxy_pass http://backend/api/;
            proxy_http_version 1.1;
            proxy_set_header Upgrade $http_upgrade;
            proxy_set_header Connection keep-alive;
            proxy_set_header Host $host;
            proxy_set_header X-Real-IP $remote_addr;
            proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
            proxy_set_header X-Forwarded-Proto $scheme;
            proxy_buffering off;
        }

        # Frontend
        location / {
            proxy_pass http://frontend/;
            proxy_http_version 1.1;
            proxy_set_header Upgrade $http_upgrade;
            proxy_set_header Connection keep-alive;
            proxy_set_header Host $host;
            proxy_set_header X-Real-IP $remote_addr;
            proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
            proxy_set_header X-Forwarded-Proto $scheme;
        }
    }
}
EOF
```

### 5. Database Initialization

```bash
# For SQL Server
docker exec -it <backend-container> bash

# Inside container, run migrations
dotnet ef database update --project /app

# Or manually execute SQL scripts
sqlcmd -S your-sql-server -U sa -P "password" -i setup.sql
```

### 6. Monitoring & Logging

```bash
# View logs
docker-compose -f docker-compose.prod.yml logs -f backend
docker-compose -f docker-compose.prod.yml logs -f frontend

# Health check
curl -f http://localhost:5000/swagger || exit 1
```

---

## Cloud Deployment

### AWS Deployment

#### Using ECS (Elastic Container Service)

1. **Create ECR Repositories**
```bash
aws ecr create-repository --repository-name ecommerce-backend
aws ecr create-repository --repository-name ecommerce-frontend

# Get login token
aws ecr get-login-password --region us-east-1 | docker login --username AWS --password-stdin <account-id>.dkr.ecr.us-east-1.amazonaws.com

# Push images
docker tag ecommerce-backend:latest <account-id>.dkr.ecr.us-east-1.amazonaws.com/ecommerce-backend:latest
docker push <account-id>.dkr.ecr.us-east-1.amazonaws.com/ecommerce-backend:latest
```

2. **Create ECS Cluster**
```bash
aws ecs create-cluster --cluster-name ecommerce-prod
```

3. **Register Task Definition**
```bash
aws ecs register-task-definition --cli-input-json file://task-definition.json
```

4. **Create Service**
```bash
aws ecs create-service \
  --cluster ecommerce-prod \
  --service-name ecommerce-backend \
  --task-definition ecommerce-backend:1 \
  --desired-count 2 \
  --launch-type EC2
```

#### Using Elastic Beanstalk

```bash
# Install EB CLI
pip install awsebcli

# Initialize
eb init ecommerce-prod -p "Docker running on 64bit Amazon Linux 2" --region us-east-1

# Deploy
eb create ecommerce-prod-env
eb deploy
```

### Azure Deployment

#### Using Container Instances

```bash
# Create resource group
az group create --name ecommerce-rg --location eastus

# Deploy backend
az container create \
  --resource-group ecommerce-rg \
  --name ecommerce-backend \
  --image myregistry.azurecr.io/ecommerce-backend:latest \
  --ports 5000 \
  --environment-variables ASPNETCORE_ENVIRONMENT=Production
```

#### Using App Service

```bash
# Create App Service Plan
az appservice plan create \
  --name ecommerce-plan \
  --resource-group ecommerce-rg \
  --is-linux \
  --sku B2

# Create Web App
az webapp create \
  --resource-group ecommerce-rg \
  --plan ecommerce-plan \
  --name ecommerce-backend \
  --deployment-container-image-name-user myregistry \
  --deployment-container-image-name myregistry.azurecr.io/ecommerce-backend:latest
```

### Google Cloud Deployment

#### Using Cloud Run

```bash
# Build image
gcloud builds submit --tag gcr.io/PROJECT_ID/ecommerce-backend

# Deploy backend
gcloud run deploy ecommerce-backend \
  --image gcr.io/PROJECT_ID/ecommerce-backend:latest \
  --platform managed \
  --region us-central1 \
  --set-env-vars ASPNETCORE_ENVIRONMENT=Production

# Deploy frontend
gcloud run deploy ecommerce-frontend \
  --image gcr.io/PROJECT_ID/ecommerce-frontend:latest \
  --platform managed \
  --region us-central1
```

---

## Kubernetes Deployment

### Helm Chart

```bash
# Create namespace
kubectl create namespace ecommerce

# Install Helm chart
helm install ecommerce ./helm-chart \
  --namespace ecommerce \
  --values values.yaml
```

### Manual K8s Deployment

```yaml
apiVersion: v1
kind: Namespace
metadata:
  name: ecommerce

---
apiVersion: apps/v1
kind: Deployment
metadata:
  name: ecommerce-backend
  namespace: ecommerce
spec:
  replicas: 3
  selector:
    matchLabels:
      app: ecommerce-backend
  template:
    metadata:
      labels:
        app: ecommerce-backend
    spec:
      containers:
      - name: backend
        image: myregistry/ecommerce-backend:v1.0.0
        ports:
        - containerPort: 5000
        env:
        - name: ASPNETCORE_ENVIRONMENT
          value: "Production"
        resources:
          limits:
            memory: "512Mi"
            cpu: "500m"

---
apiVersion: v1
kind: Service
metadata:
  name: ecommerce-backend-svc
  namespace: ecommerce
spec:
  type: LoadBalancer
  ports:
  - port: 80
    targetPort: 5000
  selector:
    app: ecommerce-backend
```

```bash
# Apply configuration
kubectl apply -f deployment.yaml

# Check status
kubectl get pods -n ecommerce
kubectl logs -n ecommerce <pod-name>
```

---

## Troubleshooting

### Container Issues

```bash
# Check container logs
docker logs <container-id>

# Inspect container
docker inspect <container-id>

# Execute command in container
docker exec -it <container-id> bash
```

### Network Issues

```bash
# Check if services are running
docker ps

# Check network connectivity
docker network ls
docker network inspect <network-id>

# Test connectivity
docker exec <container-id> curl -f http://backend:5000/
```

### Database Issues

```bash
# Check database connectivity
docker exec <backend-container> dotnet run --health-check

# View database logs
docker logs <database-container>

# Reset database
docker volume rm ecommerce_postgres-data
docker-compose up -d
```

### Performance Issues

```bash
# Monitor resource usage
docker stats

# Check redis cache
redis-cli
> INFO
> KEYS *
> GET coupon:FLAT50
```

---

## Backup & Recovery

### Database Backup

```bash
# SQL Server backup
docker exec <sql-server-container> /opt/mssql-tools/bin/sqlcmd \
  -S localhost \
  -U sa \
  -P "password" \
  -Q "BACKUP DATABASE ECommerceDb TO DISK = '/var/opt/mssql/backup/ecommerce.bak'"

# Restore
RESTORE DATABASE ECommerceDb FROM DISK = '/backup/ecommerce.bak'
```

### Redis Backup

```bash
# Create backup
docker exec <redis-container> redis-cli BGSAVE

# View backup location
docker exec <redis-container> redis-cli CONFIG GET dir
```

### Volume Backup

```bash
# Backup volume
docker run --rm -v ecommerce_redis-data:/data -v $(pwd):/backup \
  alpine tar czf /backup/redis-backup.tar.gz -C /data .

# Restore volume
docker run --rm -v ecommerce_redis-data:/data -v $(pwd):/backup \
  alpine tar xzf /backup/redis-backup.tar.gz -C /data
```

---

## Security Checklist

- [ ] SSL/TLS certificates installed
- [ ] Environment variables not exposed
- [ ] Database credentials secured
- [ ] Rate limiting enabled
- [ ] Input validation enabled
- [ ] CORS configured properly
- [ ] Logging and monitoring active
- [ ] Regular backups scheduled
- [ ] Security patches applied
- [ ] API keys rotated

---

## Performance Optimization Checklist

- [ ] Caching enabled
- [ ] Database indexing optimized
- [ ] Connection pooling configured
- [ ] Gzip compression enabled
- [ ] CDN configured for static assets
- [ ] Load balancing setup
- [ ] Auto-scaling configured
- [ ] Monitoring and alerts active

---

**Version**: 1.0  
**Last Updated**: May 4, 2024
