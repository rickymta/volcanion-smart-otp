# SmartOTP - Two-Factor Authentication System

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16+-336791)](https://www.postgresql.org/)
[![Redis](https://img.shields.io/badge/Redis-7+-DC382D)](https://redis.io/)

Hệ thống xác thực hai yếu tố (2FA) hoàn chỉnh được xây dựng trên nền tảng .NET 9, áp dụng Clean Architecture, Domain-Driven Design (DDD), và CQRS pattern.

## 🌟 Tính Năng Nổi Bật

### Xác Thực & Bảo Mật
- ✅ **JWT Authentication** - Access token + Refresh token
- ✅ **BCrypt Password Hashing** - Mã hóa mật khẩu an toàn
- ✅ **AES-256 Encryption** - Mã hóa OTP secrets
- ✅ **Rate Limiting** - Giới hạn 5 lần thử trong 5 phút (Redis)
- ✅ **Audit Logging** - Ghi log tất cả hoạt động quan trọng

### Quản Lý OTP
- ✅ **TOTP** (Time-based OTP) - RFC 6238
- ✅ **HOTP** (Counter-based OTP) - RFC 4226
- ✅ **Nhiều thuật toán** - SHA1, SHA256, SHA512
- ✅ **Tùy chỉnh** - 6/8 chữ số, thời gian tùy chỉnh (mặc định 30s)
- ✅ **Multi-accounts** - Nhiều OTP account cho mỗi user

### Kiến Trúc
- ✅ **Clean Architecture** - Tách biệt rõ ràng các layer
- ✅ **Domain-Driven Design** - Rich domain models
- ✅ **CQRS Pattern** - Tách biệt Command/Query
- ✅ **MediatR** - Request/response pipeline
- ✅ **Repository Pattern** - Abstraction layer cho data access
- ✅ **Unit of Work** - Quản lý transactions

## 🏗️ Cấu Trúc Dự Án

```
SmartOTP/
├── src/
│   ├── SmartOTP.Domain/          # Entities, Value Objects, Domain Events
│   ├── SmartOTP.Application/     # Commands, Queries, DTOs, Validators
│   ├── SmartOTP.Infrastructure/  # EF Core, Services, Repositories
│   └── SmartOTP.API/             # Controllers, Middleware, Configuration
├── tests/
│   ├── SmartOTP.Domain.Tests/
│   ├── SmartOTP.Application.Tests/
│   ├── SmartOTP.Infrastructure.Tests/
│   └── SmartOTP.API.Tests/
├── docker-compose.yml
└── SmartOTP.sln
```

## 🛠️ Công Nghệ Sử Dụng

| Công nghệ | Phiên bản | Mục đích |
|-----------|-----------|----------|
| .NET | 9.0 | Framework chính |
| ASP.NET Core | 9.0 | Web API |
| Entity Framework Core | 9.0 | ORM |
| PostgreSQL | 16+ | Database |
| Redis | 7+ | Cache & Rate Limiting |
| MediatR | 12+ | CQRS Implementation |
| FluentValidation | 11+ | Request Validation |
| BCrypt.Net | 0.1+ | Password Hashing |
| xUnit | 2.6+ | Unit Testing |

## 📋 Yêu Cầu Hệ Thống

- **.NET 9 SDK** - [Download](https://dotnet.microsoft.com/download/dotnet/9.0)
- **PostgreSQL 16+** - [Download](https://www.postgresql.org/download/)
- **Redis 7+** - [Download](https://redis.io/download/)
- **Docker & Docker Compose** (Optional) - [Download](https://www.docker.com/products/docker-desktop)

## 🚀 Bắt Đầu Nhanh

### Option 1: Sử dụng Docker (Khuyến nghị)

```bash
# Clone repository
git clone https://github.com/yourusername/volcanion-smart-otp.git
cd volcanion-smart-otp

# Khởi động services
docker-compose up -d

# API sẽ chạy tại http://localhost:5000
# Swagger UI: http://localhost:5000/swagger
```

### Option 2: Cài đặt thủ công

#### 1. Cài đặt Dependencies

```bash
# Restore NuGet packages
dotnet restore SmartOTP.sln
```

#### 2. Cấu hình Database

```bash
# Sửa connection string trong appsettings.json
cd src/SmartOTP.API
notepad appsettings.json

# Connection string mẫu:
# "DefaultConnection": "Host=localhost;Port=5432;Database=smartotp;Username=postgres;Password=your_password"
```

#### 3. Tạo Database

```bash
# Generate encryption keys
.\generate-keys.ps1

# Chạy migrations
.\migrate.ps1
```

#### 4. Khởi động ứng dụng

```bash
# Chạy API
cd src/SmartOTP.API
dotnet run

# Hoặc sử dụng script
.\quick-start.ps1
```

API sẽ khởi động tại: **http://localhost:5000**  
Swagger UI: **http://localhost:5000/swagger**

## 📚 API Documentation

### Authentication Endpoints

#### Đăng ký
```http
POST /api/auth/register
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "SecurePass123!",
  "firstName": "John",
  "lastName": "Doe"
}
```

#### Đăng nhập
```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "SecurePass123!"
}
```

#### Refresh Token
```http
POST /api/auth/refresh-token
Content-Type: application/json

{
  "refreshToken": "your_refresh_token"
}
```

### OTP Account Management

#### Tạo OTP Account
```http
POST /api/otpaccounts
Authorization: Bearer {token}
Content-Type: application/json

{
  "issuer": "MyApp",
  "accountName": "user@example.com",
  "type": "TOTP",
  "algorithm": "SHA1",
  "digits": 6,
  "period": 30
}
```

#### Lấy danh sách OTP Accounts
```http
GET /api/otpaccounts
Authorization: Bearer {token}
```

#### Xóa OTP Account
```http
DELETE /api/otpaccounts/{accountId}
Authorization: Bearer {token}
```

### OTP Operations

#### Generate OTP Code
```http
GET /api/otp/generate/{accountId}
Authorization: Bearer {token}
```

#### Verify OTP Code
```http
POST /api/otp/verify
Authorization: Bearer {token}
Content-Type: application/json

{
  "accountId": "account-guid",
  "code": "123456"
}
```

## 🔐 Cấu Hình Bảo Mật

### JWT Configuration

```json
{
  "Jwt": {
    "Secret": "YourSecretKeyAtLeast32CharactersLong",
    "Issuer": "SmartOTP",
    "Audience": "SmartOTP.API",
    "AccessTokenExpirationMinutes": "60"
  }
}
```

### Encryption Configuration

```json
{
  "Encryption": {
    "Key": "base64_encoded_32_bytes_key",
    "IV": "base64_encoded_16_bytes_iv"
  }
}
```

**Lưu ý:** Sử dụng script `generate-keys.ps1` để tạo keys an toàn.

## 🧪 Testing

```bash
# Chạy tất cả tests
dotnet test

# Chạy tests với coverage
dotnet test /p:CollectCoverage=true /p:CoverageReportFormat=opencover

# Chạy tests cho specific project
dotnet test tests/SmartOTP.Application.Tests/SmartOTP.Application.Tests.csproj
```

## 📊 Database Schema

### Users Table
```sql
- Id (UUID, PK)
- Email (VARCHAR, Unique)
- PasswordHash (VARCHAR)
- FirstName (VARCHAR)
- LastName (VARCHAR)
- IsEmailVerified (BOOLEAN)
- RefreshToken (VARCHAR, Nullable)
- RefreshTokenExpiresAt (TIMESTAMP, Nullable)
- CreatedAt (TIMESTAMP)
- UpdatedAt (TIMESTAMP)
- IsDeleted (BOOLEAN)
```

### OtpAccounts Table
```sql
- Id (UUID, PK)
- UserId (UUID, FK)
- Issuer (VARCHAR)
- AccountName (VARCHAR)
- SecretEncryptedValue (TEXT)
- Type (VARCHAR) -- TOTP/HOTP
- Algorithm (VARCHAR) -- SHA1/SHA256/SHA512
- Digits (INT) -- 6 or 8
- Period (INT) -- For TOTP
- Counter (BIGINT) -- For HOTP
- SortOrder (INT)
- CreatedAt (TIMESTAMP)
- UpdatedAt (TIMESTAMP)
- IsDeleted (BOOLEAN)
```

### AuditLogs Table
```sql
- Id (UUID, PK)
- UserId (UUID, FK)
- Action (VARCHAR)
- Status (VARCHAR) -- Success/Failure
- ErrorMessage (VARCHAR, Nullable)
- Details (VARCHAR, Nullable)
- CreatedAt (TIMESTAMP)
```

## 🔄 Luồng Hoạt Động

### 1. Đăng ký và Kích hoạt 2FA

```
User → Register → Login → Create OTP Account → Scan QR Code → Verify OTP → 2FA Enabled
```

### 2. Đăng nhập với 2FA

```
User → Login → Enter OTP → Verify OTP → Success
```

### 3. Xác thực giao dịch

```
User → Perform Action → Request OTP → Verify OTP → Execute Action
```

## 🌐 Tích Hợp với Ứng Dụng Khác

SmartOTP được thiết kế để dễ dàng tích hợp với bất kỳ hệ thống nào. Xem hướng dẫn chi tiết tại [2FA-INTEGRATION-GUIDE.md](2FA-INTEGRATION-GUIDE.md)

### Ứng dụng Authenticator tương thích:
- ✅ Google Authenticator
- ✅ Microsoft Authenticator
- ✅ Authy
- ✅ 1Password
- ✅ LastPass Authenticator
- ✅ FreeOTP

## 📱 QR Code Format

```
otpauth://totp/Issuer:AccountName?secret=BASE32SECRET&issuer=Issuer&algorithm=SHA1&digits=6&period=30
```

## 🔧 Scripts Hỗ Trợ

| Script | Mô tả |
|--------|-------|
| `generate-keys.ps1` | Tạo JWT secret và Encryption keys |
| `migrate.ps1` | Chạy EF Core migrations |
| `quick-start.ps1` | Setup và khởi động ứng dụng nhanh |

## 🐳 Docker Commands

```bash
# Build images
docker-compose build

# Khởi động services
docker-compose up -d

# Xem logs
docker-compose logs -f smartotp-api

# Dừng services
docker-compose down

# Dừng và xóa volumes
docker-compose down -v
```

## 📈 Performance & Scalability

- **Rate Limiting**: Redis-based, 5 attempts/5 minutes
- **Caching**: Redis cache cho frequently accessed data
- **Database Indexing**: Optimized indexes trên các bảng quan trọng
- **Async/Await**: Non-blocking I/O operations
- **Connection Pooling**: EF Core connection pooling

## 🔒 Security Features

| Feature | Implementation |
|---------|----------------|
| Password Hashing | BCrypt với work factor 12 |
| OTP Secret Encryption | AES-256-CBC |
| JWT Signing | HMAC-SHA256 |
| HTTPS | Khuyến nghị cho production |
| CORS | Configurable allowed origins |
| Rate Limiting | Redis-based với sliding window |

## 🤝 Contributing

Contributions are welcome! Please follow these steps:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 👥 Authors

- **Your Name** - *Initial work* - [YourGitHub](https://github.com/yourusername)

## 🙏 Acknowledgments

- RFC 6238 - TOTP Specification
- RFC 4226 - HOTP Specification
- Clean Architecture by Robert C. Martin
- Domain-Driven Design by Eric Evans

## 📧 Support

Nếu bạn gặp vấn đề hoặc có câu hỏi:

- 🐛 [Report Bug](https://github.com/yourusername/volcanion-smart-otp/issues)
- 💡 [Request Feature](https://github.com/yourusername/volcanion-smart-otp/issues)
- 📧 Email: support@example.com

## 🗺️ Roadmap

- [ ] Email verification
- [ ] SMS-based OTP
- [ ] Backup codes generation
- [ ] Account recovery mechanism
- [ ] Admin dashboard
- [ ] Multi-language support
- [ ] Mobile SDK (iOS/Android)
- [ ] WebAuthn/FIDO2 support

---

**Made with ❤️ using .NET 9 and Clean Architecture**
