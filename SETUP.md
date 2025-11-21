# Smart OTP Backend - Setup Guide

This guide will help you set up and run the Smart OTP backend system.

## Table of Contents
1. [Prerequisites](#prerequisites)
2. [Quick Start with Docker](#quick-start-with-docker)
3. [Manual Setup](#manual-setup)
4. [Configuration](#configuration)
5. [Database Migrations](#database-migrations)
6. [Running the Application](#running-the-application)
7. [Testing](#testing)
8. [Troubleshooting](#troubleshooting)

## Prerequisites

### Required
- **.NET 9 SDK** - [Download](https://dotnet.microsoft.com/download/dotnet/9.0)
- **PostgreSQL 16+** - [Download](https://www.postgresql.org/download/)
- **Redis 7+** - [Download](https://redis.io/download)

### Optional (for Docker)
- **Docker Desktop** - [Download](https://www.docker.com/products/docker-desktop)
- **Docker Compose** (included with Docker Desktop)

## Quick Start with Docker

This is the easiest way to get started!

### Step 1: Generate Encryption Keys

```powershell
.\generate-keys.ps1
```

Copy the output and save it. You'll need these keys in the next step.

### Step 2: Update Configuration

Open `src\SmartOTP.API\appsettings.json` and update:

```json
{
  "Encryption": {
    "Key": "PASTE_YOUR_GENERATED_KEY_HERE",
    "IV": "PASTE_YOUR_GENERATED_IV_HERE"
  },
  "Jwt": {
    "Secret": "PASTE_YOUR_GENERATED_JWT_SECRET_HERE"
  }
}
```

### Step 3: Start Everything

```powershell
.\quick-start.ps1 -Docker
```

This will:
- Start PostgreSQL container
- Start Redis container
- Build and start the API container
- Apply database migrations automatically

### Step 4: Access the API

- **API**: http://localhost:5000
- **Swagger UI**: http://localhost:5000/swagger

That's it! Your Smart OTP backend is now running.

## Manual Setup

If you prefer to run without Docker:

### Step 1: Install .NET 9 SDK

Verify installation:
```powershell
dotnet --version
# Should output: 9.x.x
```

### Step 2: Install PostgreSQL

1. Download and install PostgreSQL 16+
2. Create a database:
```sql
CREATE DATABASE smartotp;
```

### Step 3: Install Redis

**Windows (using Chocolatey):**
```powershell
choco install redis-64
```

**Or use Docker for just PostgreSQL and Redis:**
```powershell
# PostgreSQL
docker run --name smartotp-postgres `
  -e POSTGRES_PASSWORD=postgres `
  -e POSTGRES_DB=smartotp `
  -p 5432:5432 -d postgres:16-alpine

# Redis
docker run --name smartotp-redis `
  -p 6379:6379 -d redis:7-alpine
```

### Step 4: Clone and Restore Packages

```powershell
cd e:\Github\volcanion-smart-otp
dotnet restore
```

### Step 5: Generate Encryption Keys

```powershell
.\generate-keys.ps1
```

### Step 6: Update Configuration

Update `src\SmartOTP.API\appsettings.json` with:
- Generated encryption keys
- Database connection string (if different)
- Redis connection string (if different)

### Step 7: Apply Database Migrations

```powershell
.\migrate.ps1 add InitialCreate
.\migrate.ps1 update
```

Or manually:
```powershell
cd src\SmartOTP.Infrastructure
dotnet ef migrations add InitialCreate --startup-project ..\SmartOTP.API
dotnet ef database update --startup-project ..\SmartOTP.API
```

### Step 8: Run the API

```powershell
cd src\SmartOTP.API
dotnet run
```

## Configuration

### appsettings.json Structure

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Information"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=smartotp;Username=postgres;Password=postgres",
    "Redis": "localhost:6379"
  },
  "Jwt": {
    "Secret": "Your-JWT-Secret-At-Least-32-Characters-Long",
    "Issuer": "SmartOTP",
    "Audience": "SmartOTP.API",
    "AccessTokenExpirationMinutes": "60"
  },
  "Encryption": {
    "Key": "Base64-Encoded-32-Byte-AES-Key",
    "IV": "Base64-Encoded-16-Byte-IV"
  },
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:3000",
      "http://localhost:5173"
    ]
  }
}
```

### Environment Variables (Optional)

You can also use environment variables to override settings:

```powershell
$env:ConnectionStrings__DefaultConnection = "Host=localhost;..."
$env:Jwt__Secret = "your-secret"
$env:Encryption__Key = "your-key"
$env:Encryption__IV = "your-iv"
```

## Database Migrations

### Create a New Migration

```powershell
.\migrate.ps1 add MigrationName
```

### Apply Migrations

```powershell
.\migrate.ps1 update
```

### Remove Last Migration

```powershell
.\migrate.ps1 remove
```

### List All Migrations

```powershell
.\migrate.ps1 list
```

### Generate SQL Script

```powershell
.\migrate.ps1 script
```

## Running the Application

### Development Mode

```powershell
cd src\SmartOTP.API
dotnet run
```

Or with watch mode (auto-restart on file changes):
```powershell
dotnet watch run
```

### Production Mode

```powershell
dotnet run --configuration Release
```

### Using Docker Compose

```powershell
docker-compose up -d
```

View logs:
```powershell
docker-compose logs -f api
```

Stop services:
```powershell
docker-compose down
```

## Testing

### Run All Tests

```powershell
cd tests\SmartOTP.Tests
dotnet test
```

### Run with Coverage

```powershell
dotnet test /p:CollectCoverage=true
```

### Run Specific Test

```powershell
dotnet test --filter "FullyQualifiedName~UserTests"
```

## Testing the API

### Using Swagger UI

1. Navigate to http://localhost:5000/swagger
2. Click "Authorize" button
3. Register a user via `/api/auth/register`
4. Copy the `accessToken` from response
5. Enter `Bearer {accessToken}` in the authorization dialog
6. Now you can test protected endpoints

### Using cURL

See [API-EXAMPLES.md](API-EXAMPLES.md) for complete API examples.

**Quick Test:**

```bash
# Register
curl -X POST http://localhost:5000/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    "password": "SecurePass123!@#",
    "firstName": "Test",
    "lastName": "User"
  }'

# Create OTP Account (use token from register response)
curl -X POST http://localhost:5000/api/otpaccounts \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -d '{
    "issuer": "GitHub",
    "accountName": "test@example.com",
    "type": "TOTP",
    "algorithm": "SHA1",
    "digits": 6,
    "period": 30
  }'
```

## Troubleshooting

### Issue: "Connection refused" to PostgreSQL

**Solution:**
- Ensure PostgreSQL is running
- Check connection string in `appsettings.json`
- Verify PostgreSQL port (default: 5432)

```powershell
# Check if PostgreSQL is running
Get-Service -Name postgresql*

# Or using Docker
docker ps | Select-String postgres
```

### Issue: "Connection refused" to Redis

**Solution:**
- Ensure Redis is running
- Check connection string in `appsettings.json`
- Verify Redis port (default: 6379)

```powershell
# Using Docker
docker ps | Select-String redis

# Test Redis connection
docker exec -it smartotp-redis redis-cli ping
# Should return: PONG
```

### Issue: EF Core migrations fail

**Solution:**

```powershell
# Clean and rebuild
dotnet clean
dotnet build

# Remove and recreate migration
.\migrate.ps1 remove
.\migrate.ps1 add InitialCreate
.\migrate.ps1 update
```

### Issue: JWT validation fails

**Solution:**
- Ensure JWT secret is at least 32 characters
- Check that secret matches between configuration and token generation
- Verify token hasn't expired (default: 1 hour)

### Issue: OTP verification always fails

**Solution:**
- Check that encryption key and IV haven't changed
- Verify system time is correct (TOTP is time-based)
- Check rate limiting (5 attempts per 5 minutes)

### Issue: Docker build fails

**Solution:**

```powershell
# Clean Docker cache
docker system prune -a

# Rebuild without cache
docker-compose build --no-cache
docker-compose up
```

### Issue: Rate limiting triggers immediately

**Solution:**
- Clear Redis cache:
```powershell
docker exec -it smartotp-redis redis-cli FLUSHALL
```

### Enable Detailed Logging

Update `appsettings.Development.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore": "Debug",
      "Microsoft.EntityFrameworkCore": "Information"
    }
  }
}
```

## Security Checklist

Before deploying to production:

- [ ] Change all default passwords
- [ ] Generate new encryption keys (don't use development keys)
- [ ] Use environment variables for secrets (don't commit to Git)
- [ ] Enable HTTPS
- [ ] Configure CORS for specific origins only
- [ ] Set up proper firewall rules
- [ ] Enable rate limiting on API gateway
- [ ] Implement monitoring and alerting
- [ ] Regular database backups
- [ ] Keep dependencies updated

## Next Steps

1. **Read the [README.md](README.md)** for feature overview
2. **Check [API-EXAMPLES.md](API-EXAMPLES.md)** for API usage examples
3. **Review [ARCHITECTURE.md](ARCHITECTURE.md)** for architecture details
4. **Start building your frontend** application

## Getting Help

- Check the [README.md](README.md) for general information
- Review [API-EXAMPLES.md](API-EXAMPLES.md) for usage examples
- Read [ARCHITECTURE.md](ARCHITECTURE.md) for technical details
- Open an issue on GitHub for bugs or questions

## Useful Commands

```powershell
# Build solution
dotnet build

# Run tests
dotnet test

# Restore packages
dotnet restore

# Clean build artifacts
dotnet clean

# Format code
dotnet format

# Check for updates
dotnet list package --outdated

# Generate migration
.\migrate.ps1 add MigrationName

# Apply migrations
.\migrate.ps1 update

# Generate encryption keys
.\generate-keys.ps1

# Quick start with Docker
.\quick-start.ps1 -Docker

# Quick start locally
.\quick-start.ps1 -Local
```

---

Happy coding! 🚀
