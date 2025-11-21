# 🚀 Getting Started Checklist

Follow this checklist to get your Smart OTP Backend up and running!

## ✅ Pre-Flight Checklist

### 1. Install Prerequisites

- [ ] .NET 9 SDK installed
  - Run: `dotnet --version` (should show 9.x.x)
  - Download: https://dotnet.microsoft.com/download/dotnet/9.0

- [ ] Docker Desktop installed (for Docker route)
  - Download: https://www.docker.com/products/docker-desktop

**OR** (for manual setup):

- [ ] PostgreSQL 16+ installed
  - Download: https://www.postgresql.org/download/

- [ ] Redis 7+ installed or Docker
  - Windows: `choco install redis-64`
  - Or Docker: See SETUP.md

## ✅ Configuration Steps

### 2. Generate Encryption Keys

- [ ] Run: `.\generate-keys.ps1`
- [ ] Copy the generated keys (you'll need them next)

### 3. Update Configuration

- [ ] Open `src\SmartOTP.API\appsettings.json`
- [ ] Replace `CHANGE_THIS_TO_A_32_BYTE_BASE64_KEY` with your generated Key
- [ ] Replace `CHANGE_THIS_TO_A_16_BYTE_BASE64_IV` with your generated IV
- [ ] Replace JWT Secret with your generated JWT secret

**Your appsettings.json should look like:**
```json
{
  "Encryption": {
    "Key": "YOUR_GENERATED_BASE64_KEY_HERE",
    "IV": "YOUR_GENERATED_BASE64_IV_HERE"
  },
  "Jwt": {
    "Secret": "YOUR_GENERATED_JWT_SECRET_HERE",
    "Issuer": "SmartOTP",
    "Audience": "SmartOTP.API",
    "AccessTokenExpirationMinutes": "60"
  }
}
```

## ✅ Choose Your Path

### Option A: Docker (Recommended for Quick Start)

- [ ] Run: `.\quick-start.ps1 -Docker`
- [ ] Wait for containers to start
- [ ] Access Swagger: http://localhost:5000/swagger
- [ ] **You're done!** Skip to "Testing" section below

### Option B: Local Development

#### 4. Start Database Services

- [ ] Start PostgreSQL (or run: `docker run --name smartotp-postgres -e POSTGRES_PASSWORD=postgres -e POSTGRES_DB=smartotp -p 5432:5432 -d postgres:16-alpine`)
- [ ] Start Redis (or run: `docker run --name smartotp-redis -p 6379:6379 -d redis:7-alpine`)

#### 5. Restore Packages

- [ ] Run: `dotnet restore`

#### 6. Create Database

- [ ] Run: `.\migrate.ps1 add InitialCreate`
- [ ] Run: `.\migrate.ps1 update`

#### 7. Run the API

- [ ] Run: `cd src\SmartOTP.API`
- [ ] Run: `dotnet run`
- [ ] Access Swagger: http://localhost:5000/swagger

## ✅ Testing

### 8. Test the API

Using Swagger UI (http://localhost:5000/swagger):

- [ ] **Register a user**
  - Click on `POST /api/auth/register`
  - Click "Try it out"
  - Enter test data:
    ```json
    {
      "email": "test@example.com",
      "password": "SecurePass123!@#",
      "firstName": "Test",
      "lastName": "User"
    }
    ```
  - Click "Execute"
  - Copy the `accessToken` from the response

- [ ] **Authorize in Swagger**
  - Click the "Authorize" button at the top
  - Enter: `Bearer YOUR_ACCESS_TOKEN_HERE`
  - Click "Authorize"

- [ ] **Create an OTP Account**
  - Click on `POST /api/otpaccounts`
  - Click "Try it out"
  - Enter:
    ```json
    {
      "issuer": "GitHub",
      "accountName": "test@example.com",
      "type": "TOTP",
      "algorithm": "SHA1",
      "digits": 6,
      "period": 30
    }
    ```
  - Click "Execute"
  - Copy the account `id` from response

- [ ] **Generate an OTP Code**
  - Click on `GET /api/otp/generate/{accountId}`
  - Click "Try it out"
  - Paste your account ID
  - Click "Execute"
  - You should see a 6-digit code!

- [ ] **Verify the OTP Code**
  - Click on `POST /api/otp/verify`
  - Click "Try it out"
  - Enter:
    ```json
    {
      "accountId": "YOUR_ACCOUNT_ID",
      "code": "THE_CODE_FROM_PREVIOUS_STEP"
    }
    ```
  - Click "Execute"
  - Should return: `{ "isValid": true }`

## ✅ Verification

### 9. Verify Everything Works

- [ ] API responds at http://localhost:5000
- [ ] Swagger UI loads at http://localhost:5000/swagger
- [ ] User registration works
- [ ] Login works
- [ ] JWT tokens are generated
- [ ] OTP account creation works
- [ ] OTP code generation works
- [ ] OTP verification works
- [ ] Rate limiting works (try verifying wrong code 6 times)

## ✅ Next Steps

### 10. Development

- [ ] Read [README.md](README.md) for feature overview
- [ ] Check [API-EXAMPLES.md](API-EXAMPLES.md) for more examples
- [ ] Review [ARCHITECTURE.md](ARCHITECTURE.md) for architecture details
- [ ] Run tests: `cd tests\SmartOTP.Tests; dotnet test`

### 11. Build Your Frontend

Now you can build a frontend application that uses this API!

**Suggested frontend frameworks:**
- React / Next.js
- Vue.js / Nuxt.js
- Angular
- Blazor
- Mobile: React Native, Flutter, .NET MAUI

**Key endpoints to integrate:**
1. `/api/auth/register` - User registration
2. `/api/auth/login` - User login
3. `/api/otpaccounts` - Manage OTP accounts
4. `/api/otp/generate/{id}` - Get OTP codes
5. `/api/otp/verify` - Verify codes

## 🎉 Success Criteria

You're all set when:
- ✅ API is running
- ✅ Database is created and migrated
- ✅ You can register a user
- ✅ You can create OTP accounts
- ✅ You can generate OTP codes
- ✅ You can verify OTP codes

## 🆘 Troubleshooting

If something doesn't work:

1. **Check logs** in the console where API is running
2. **Verify configuration** in appsettings.json
3. **Check services** - PostgreSQL and Redis must be running
4. **Read SETUP.md** for detailed troubleshooting
5. **Check ports** - Ensure 5000, 5432, 6379 are available

### Quick Diagnostics

```powershell
# Check .NET version
dotnet --version

# Check if PostgreSQL is running (Docker)
docker ps | Select-String postgres

# Check if Redis is running (Docker)
docker ps | Select-String redis

# Test PostgreSQL connection
docker exec -it smartotp-postgres psql -U postgres -d smartotp

# Test Redis connection
docker exec -it smartotp-redis redis-cli ping

# View API logs
docker logs smartotp-api
```

## 📚 Documentation

- **README.md** - Project overview and features
- **SETUP.md** - Detailed setup instructions
- **ARCHITECTURE.md** - Architecture and design patterns
- **API-EXAMPLES.md** - API endpoint examples
- **PROJECT-SUMMARY.md** - Complete project summary

## 🔐 Security Reminders

Before going to production:
- [ ] Change all default passwords
- [ ] Use environment variables for secrets
- [ ] Enable HTTPS
- [ ] Configure proper CORS
- [ ] Set up monitoring
- [ ] Implement backups
- [ ] Review security checklist in SETUP.md

---

**Have fun building with Smart OTP!** 🚀

Need help? Check the documentation or open an issue!
