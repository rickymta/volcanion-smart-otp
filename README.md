# Smart OTP Backend

A complete Smart OTP (One-Time Password) backend system similar to Microsoft Authenticator and Google Authenticator, built using .NET 9, Clean Architecture, Domain-Driven Design (DDD), CQRS, and MediatR patterns.

## 🚀 Features

### Authentication & Authorization
- User registration and login with JWT tokens
- Access token + Refresh token implementation
- Password hashing with BCrypt
- Email-based user accounts

### OTP Management
- **TOTP (Time-based OTP)** - RFC 6238 compliant
- **HOTP (Counter-based OTP)** - RFC 4226 compliant
- Support for SHA1, SHA256, and SHA512 algorithms
- 6 or 8 digit codes
- Customizable time periods (default 30 seconds)
- Multiple OTP accounts per user

### Security
- AES-256 encryption for OTP secrets at rest
- JWT authentication middleware
- Rate limiting (5 attempts per 5 minutes) using Redis
- Audit logging for all critical operations
- Soft delete pattern for data integrity

### Architecture
- **Clean Architecture** - Separation of concerns across layers
- **Domain-Driven Design** - Rich domain models with business logic
- **CQRS** - Command/Query separation
- **MediatR** - Request/response pattern
- **Repository Pattern** - Data access abstraction
- **Unit of Work** - Transaction management

## 🏗️ Architecture

```
SmartOTP/
├── src/
│   ├── SmartOTP.Domain/          # Domain entities, value objects, events
│   ├── SmartOTP.Application/     # Use cases, DTOs, interfaces
│   ├── SmartOTP.Infrastructure/  # EF Core, Redis, services
│   └── SmartOTP.API/             # Controllers, middleware, startup
└── tests/
    └── SmartOTP.Tests/           # Unit and integration tests
```

### Layers

1. **Domain Layer**
   - Entities: `User`, `OtpAccount`, `AuditLog`
   - Value Objects: `OtpCode`, `SecretKey`
   - Domain Events
   - Enums

2. **Application Layer**
   - Commands & Queries (CQRS)
   - MediatR handlers
   - DTOs
   - FluentValidation validators
   - Interface definitions

3. **Infrastructure Layer**
   - EF Core DbContext & Configurations
   - Repository implementations
   - PostgreSQL database
   - Redis caching
   - Encryption service
   - OTP generation service
   - JWT service

4. **Presentation Layer (API)**
   - Controllers
   - Middleware
   - JWT authentication
   - Swagger documentation

## 🛠️ Technology Stack

- **.NET 9** - Latest .NET framework
- **ASP.NET Core Web API** - RESTful API
- **Entity Framework Core 9** - ORM
- **PostgreSQL** - Primary database
- **Redis** - Caching and rate limiting
- **MediatR** - CQRS implementation
- **FluentValidation** - Request validation
- **BCrypt.Net** - Password hashing
- **JWT** - Authentication tokens
- **Swagger/OpenAPI** - API documentation
- **xUnit** - Unit testing

## 📋 Prerequisites

- .NET 9 SDK
- PostgreSQL 16+
- Redis 7+
- Docker & Docker Compose (optional)

## 🚀 Getting Started

### Option 1: Using Docker Compose (Recommended)

1. Generate encryption keys:
```powershell
# Generate 32-byte AES key
$key = [System.Security.Cryptography.Aes]::Create().Key
[System.Convert]::ToBase64String($key)

# Generate 16-byte IV
$iv = [System.Security.Cryptography.Aes]::Create().IV
[System.Convert]::ToBase64String($iv)
```

2. Update `appsettings.json` with generated keys:
```json
{
  "Encryption": {
    "Key": "YOUR_GENERATED_BASE64_KEY",
    "IV": "YOUR_GENERATED_BASE64_IV"
  }
}
```

3. Start all services:
```bash
docker-compose up -d
```

4. API will be available at: `http://localhost:5000`
5. Swagger UI: `http://localhost:5000/swagger`

### Option 2: Manual Setup

1. **Install Dependencies**
```bash
cd src/SmartOTP.API
dotnet restore
```

2. **Start PostgreSQL**
```bash
docker run --name smartotp-postgres -e POSTGRES_PASSWORD=postgres -e POSTGRES_DB=smartotp -p 5432:5432 -d postgres:16-alpine
```

3. **Start Redis**
```bash
docker run --name smartotp-redis -p 6379:6379 -d redis:7-alpine
```

4. **Update Connection Strings** in `appsettings.json`

5. **Apply Migrations**
```bash
cd src/SmartOTP.Infrastructure
dotnet ef database update --startup-project ../SmartOTP.API
```

6. **Run the API**
```bash
cd ../SmartOTP.API
dotnet run
```

## 📚 API Endpoints

### Authentication
- `POST /api/auth/register` - Register new user
- `POST /api/auth/login` - Login user
- `POST /api/auth/refresh-token` - Refresh access token

### OTP Accounts
- `GET /api/otpaccounts` - Get user's OTP accounts (requires auth)
- `POST /api/otpaccounts` - Create new OTP account (requires auth)
- `DELETE /api/otpaccounts/{id}` - Delete OTP account (requires auth)

### OTP Operations
- `GET /api/otp/generate/{accountId}` - Generate OTP code (requires auth)
- `POST /api/otp/verify` - Verify OTP code (requires auth)

## 🔐 Security Features

### Encryption
- OTP secrets encrypted using AES-256 before storage
- Unique encryption key and IV per environment

### Rate Limiting
- Maximum 5 OTP verification attempts per 5 minutes per user
- Implemented using Redis counters

### JWT Authentication
- Access tokens expire in 1 hour
- Refresh tokens expire in 7 days
- Secure token validation

### Audit Logging
- All user actions logged
- Success/failure tracking
- IP address and user agent capture

## 📊 Database Schema

### Users Table
- Id, Email, PasswordHash, FirstName, LastName
- IsEmailVerified, LastLoginAt
- RefreshToken, RefreshTokenExpiryTime
- CreatedAt, UpdatedAt, IsDeleted

### OtpAccounts Table
- Id, UserId, Issuer, AccountName
- Type (TOTP/HOTP), Algorithm, Digits, Period, Counter
- EncryptedSecret, SecretCreatedAt
- IconUrl, SortOrder
- CreatedAt, UpdatedAt, IsDeleted

### AuditLogs Table
- Id, UserId, Action, IpAddress, UserAgent
- Details, IsSuccess, ErrorMessage
- CreatedAt

## 🧪 Testing

Run unit tests:
```bash
cd tests/SmartOTP.Tests
dotnet test
```

## 📖 Example Usage

### 1. Register User
```bash
curl -X POST http://localhost:5000/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "user@example.com",
    "password": "SecurePass123!",
    "firstName": "John",
    "lastName": "Doe"
  }'
```

### 2. Create OTP Account
```bash
curl -X POST http://localhost:5000/api/otpaccounts \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_ACCESS_TOKEN" \
  -d '{
    "issuer": "GitHub",
    "accountName": "john@example.com",
    "type": "TOTP",
    "algorithm": "SHA1",
    "digits": 6,
    "period": 30
  }'
```

### 3. Generate OTP Code
```bash
curl -X GET http://localhost:5000/api/otp/generate/{accountId} \
  -H "Authorization: Bearer YOUR_ACCESS_TOKEN"
```

### 4. Verify OTP Code
```bash
curl -X POST http://localhost:5000/api/otp/verify \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_ACCESS_TOKEN" \
  -d '{
    "accountId": "ACCOUNT_GUID",
    "code": "123456"
  }'
```

## 🔧 Configuration

### appsettings.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=smartotp;Username=postgres;Password=postgres",
    "Redis": "localhost:6379"
  },
  "Jwt": {
    "Secret": "Your-Secret-Key-At-Least-32-Characters",
    "Issuer": "SmartOTP",
    "Audience": "SmartOTP.API",
    "AccessTokenExpirationMinutes": "60"
  },
  "Encryption": {
    "Key": "BASE64_ENCODED_32_BYTE_KEY",
    "IV": "BASE64_ENCODED_16_BYTE_IV"
  }
}
```

## 📝 TOTP & HOTP Algorithms

### TOTP (Time-based OTP)
- Based on current time
- Default 30-second window
- Supports time drift (±1 window)

### HOTP (Counter-based OTP)
- Based on incrementing counter
- Counter stored and incremented after each generation
- More suitable for hardware tokens

Both support:
- SHA1, SHA256, SHA512 algorithms
- 6 or 8 digit codes
- Base32 encoded secrets

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## 📄 License

This project is licensed under the MIT License.

## 🔗 References

- [RFC 6238 - TOTP](https://datatracker.ietf.org/doc/html/rfc6238)
- [RFC 4226 - HOTP](https://datatracker.ietf.org/doc/html/rfc4226)
- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Domain-Driven Design](https://martinfowler.com/bliki/DomainDrivenDesign.html)

## 📞 Support

For issues and questions, please open an issue on GitHub.

---

Built with ❤️ using .NET 9 and Clean Architecture principles.
