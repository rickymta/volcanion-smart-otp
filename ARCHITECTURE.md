# SmartOTP - Architecture Documentation

## 📐 Tổng Quan Kiến Trúc

SmartOTP được xây dựng dựa trên **Clean Architecture** kết hợp với **Domain-Driven Design (DDD)** và **CQRS pattern**, đảm bảo tính separation of concerns, testability, và maintainability.

## 🏛️ Clean Architecture Layers

```
┌────────────────────────────────────────────────────────────────┐
│                      Presentation Layer                        │
│                       (SmartOTP.API)                           │
│  ┌──────────────┬──────────────────┬────────────────────┐      │
│  │ Controllers  │   Middleware     │   Configuration    │      │
│  │              │                  │                    │      │
│  │ - Auth       │ - Exception      │ - JWT Setup        │      │
│  │ - OTP        │   Handling       │ - CORS             │      │
│  │ - Accounts   │ - Logging        │ - Swagger          │      │
│  └──────────────┴──────────────────┴────────────────────┘      │
└────────────────────────────────────────────────────────────────┘
                            ↓ depends on
┌────────────────────────────────────────────────────────────────┐
│                     Application Layer                          │
│                   (SmartOTP.Application)                       │
│  ┌──────────────┬──────────────────┬────────────────────┐      │
│  │  Commands    │     Queries      │       DTOs         │      │
│  │              │                  │                    │      │
│  │ - Register   │ - GetAccounts    │ - AuthResponse     │      │
│  │ - Login      │ - GenerateOTP    │ - OtpAccount       │      │
│  │ - CreateOTP  │                  │ - OtpCode          │      │
│  │ - VerifyOTP  │                  │ - User             │      │
│  └──────────────┴──────────────────┴────────────────────┘      │
│  ┌──────────────┬──────────────────┬────────────────────┐      │
│  │   Handlers   │    Validators    │    Interfaces      │      │
│  │  (MediatR)   │ (FluentValid.)   │                    │      │
│  └──────────────┴──────────────────┴────────────────────┘      │
└────────────────────────────────────────────────────────────────┘
                            ↓ depends on
┌────────────────────────────────────────────────────────────────┐
│                       Domain Layer                             │
│                     (SmartOTP.Domain)                          │
│  ┌──────────────┬──────────────────┬────────────────────┐      │
│  │  Entities    │  Value Objects   │  Domain Events     │      │
│  │              │                  │                    │      │
│  │ - User       │ - OtpCode        │ - UserRegistered   │      │
│  │ - OtpAccount │ - SecretKey      │ - OtpGenerated     │      │
│  │ - AuditLog   │                  │ - OtpVerified      │      │
│  └──────────────┴──────────────────┴────────────────────┘      │
│  ┌─────────────────────────────────────────────────────┐       │
│  │              Enums & Common                         │       │
│  │  - OtpType, OtpAlgorithm, AuditActionType           │       │
│  │  - BaseEntity, IDomainEvent, ValueObject            │       │
│  └─────────────────────────────────────────────────────┘       │
└────────────────────────────────────────────────────────────────┘
                            ↑ implements
┌────────────────────────────────────────────────────────────────┐
│                   Infrastructure Layer                         │
│                  (SmartOTP.Infrastructure)                     │
│  ┌──────────────┬──────────────────┬────────────────────┐      │
│  │ Persistence  │     Services     │   External         │      │
│  │              │                  │                    │      │
│  │ - DbContext  │ - OtpService     │ - PostgreSQL       │      │
│  │ - Repository │ - Encryption     │ - Redis            │      │
│  │ - UnitOfWork │ - JwtService     │                    │      │
│  │ - Migrations │ - CacheService   │                    │      │
│  └──────────────┴──────────────────┴────────────────────┘      │
└────────────────────────────────────────────────────────────────┘
```

## 🎯 Dependency Rule

**Quy tắc quan trọng:** Dependencies chỉ được trỏ vào trong (inward), không được trỏ ra ngoài (outward).

```
Infrastructure → Application → Domain
     ↑                              ↑
Presentation ───────────────────────┘
```

## 🔷 Domain Layer (Core)

### Entities

#### User Entity
```csharp
public class User : BaseEntity
{
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public bool IsEmailVerified { get; private set; }
    public string? RefreshToken { get; private set; }
    public DateTime? RefreshTokenExpiresAt { get; private set; }
    
    // Navigation
    public ICollection<OtpAccount> OtpAccounts { get; private set; }
    public ICollection<AuditLog> AuditLogs { get; private set; }
    
    // Business logic methods
    public void SetRefreshToken(string token, DateTime expiresAt);
    public void ClearRefreshToken();
    public bool IsRefreshTokenValid(string token);
}
```

#### OtpAccount Entity
```csharp
public class OtpAccount : BaseEntity
{
    public Guid UserId { get; private set; }
    public string Issuer { get; private set; }
    public string AccountName { get; private set; }
    public SecretKey Secret { get; private set; } // Value Object
    public OtpType Type { get; private set; }
    public OtpAlgorithm Algorithm { get; private set; }
    public int Digits { get; private set; }
    public int Period { get; private set; }
    public long Counter { get; private set; }
    public int SortOrder { get; private set; }
    
    // Navigation
    public User User { get; private set; }
    
    // Business logic
    public void IncrementCounter();
    public void UpdateSortOrder(int newOrder);
}
```

#### AuditLog Entity
```csharp
public class AuditLog : BaseEntity
{
    public Guid UserId { get; private set; }
    public AuditActionType Action { get; private set; }
    public string Status { get; private set; }
    public string? ErrorMessage { get; private set; }
    public string? Details { get; private set; }
    
    // Navigation
    public User User { get; private set; }
    
    // Factory methods
    public static AuditLog CreateSuccess(...);
    public static AuditLog CreateFailure(...);
}
```

### Value Objects

#### SecretKey (Value Object)
```csharp
public class SecretKey : ValueObject
{
    public string EncryptedValue { get; private set; }
    
    private SecretKey(string encryptedValue)
    {
        EncryptedValue = encryptedValue;
    }
    
    public static SecretKey Create(string encryptedValue);
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return EncryptedValue;
    }
}
```

#### OtpCode (Value Object)
```csharp
public class OtpCode : ValueObject
{
    public string Code { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    
    public static OtpCode Create(string code, int validitySeconds);
    public bool IsExpired() => DateTime.UtcNow > ExpiresAt;
    public bool IsValid(string inputCode) => Code == inputCode && !IsExpired();
}
```

### Domain Events

```csharp
// User Events
public record UserRegisteredEvent(Guid UserId, string Email) : IDomainEvent;
public record UserLoggedInEvent(Guid UserId) : IDomainEvent;

// OTP Account Events
public record OtpAccountCreatedEvent(Guid AccountId, Guid UserId) : IDomainEvent;
public record OtpAccountUpdatedEvent(Guid AccountId) : IDomainEvent;
public record OtpAccountDeletedEvent(Guid AccountId, Guid UserId) : IDomainEvent;

// OTP Events
public record OtpGeneratedEvent(Guid AccountId, Guid UserId) : IDomainEvent;
public record OtpVerifiedEvent(Guid AccountId, Guid UserId, bool IsValid) : IDomainEvent;
```

### Enums

```csharp
public enum OtpType { TOTP, HOTP }

public enum OtpAlgorithm { SHA1, SHA256, SHA512 }

public enum AuditActionType
{
    UserRegistered,
    UserLoggedIn,
    OtpAccountCreated,
    OtpAccountDeleted,
    OtpGenerated,
    OtpVerified,
    OtpVerificationFailed
}
```

## 🔶 Application Layer

### CQRS Pattern

#### Commands (Write Operations)

```
Auth Commands:
├── RegisterCommand
│   └── RegisterCommandHandler
├── LoginCommand
│   └── LoginCommandHandler
└── RefreshTokenCommand
    └── RefreshTokenCommandHandler

OTP Account Commands:
├── CreateOtpAccountCommand
│   └── CreateOtpAccountCommandHandler
└── DeleteOtpAccountCommand
    └── DeleteOtpAccountCommandHandler

OTP Commands:
└── VerifyOtpCommand
    └── VerifyOtpCommandHandler
```

#### Queries (Read Operations)

```
OTP Account Queries:
└── GetUserOtpAccountsQuery
    └── GetUserOtpAccountsQueryHandler

OTP Queries:
└── GenerateOtpQuery
    └── GenerateOtpQueryHandler
```

### MediatR Pipeline

```
Request → Validation → Handler → Response
          (FluentValidation)
```

### DTOs (Data Transfer Objects)

```csharp
public record AuthResponseDto
{
    public string AccessToken { get; init; }
    public string RefreshToken { get; init; }
    public DateTime AccessTokenExpiresAt { get; init; }
    public DateTime RefreshTokenExpiresAt { get; init; }
    public UserDto User { get; init; }
}

public record OtpAccountDto
{
    public Guid Id { get; init; }
    public string Issuer { get; init; }
    public string AccountName { get; init; }
    public OtpType Type { get; init; }
    public OtpAlgorithm Algorithm { get; init; }
    public int Digits { get; init; }
    public int Period { get; init; }
    public long Counter { get; init; }
    public int SortOrder { get; init; }
    public DateTime CreatedAt { get; init; }
}

public record OtpCodeDto
{
    public string Code { get; init; }
    public int RemainingSeconds { get; init; }
    public DateTime GeneratedAt { get; init; }
}
```

### Interfaces (Abstraction)

```csharp
// Repository Pattern
public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    void Update(T entity);
    void Remove(T entity);
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
}

// Unit of Work Pattern
public interface IUnitOfWork : IDisposable
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}

// Services
public interface IOtpService
{
    string GenerateTOTP(string secret, int digits, int period, OtpAlgorithm algorithm);
    string GenerateHOTP(string secret, long counter, int digits, OtpAlgorithm algorithm);
    bool VerifyTOTP(string secret, string code, int digits, int period, OtpAlgorithm algorithm, int window = 1);
    bool VerifyHOTP(string secret, string code, long counter, int digits, OtpAlgorithm algorithm);
    int GetRemainingSeconds(int period);
}

public interface IEncryptionService
{
    string Encrypt(string plainText);
    string Decrypt(string cipherText);
}

public interface IJwtService
{
    string GenerateAccessToken(Guid userId, string email);
    string GenerateRefreshToken();
    bool ValidateToken(string token, out Guid userId);
}

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default);
    Task<long> IncrementAsync(string key, long value, TimeSpan? expiration = null, CancellationToken cancellationToken = default);
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
}

public interface IPasswordHasher
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string hash);
}

public interface IAuditService
{
    Task LogAsync(AuditLog auditLog, CancellationToken cancellationToken = default);
}
```

## 🔸 Infrastructure Layer

### Persistence (EF Core)

#### ApplicationDbContext
```csharp
public class ApplicationDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<OtpAccount> OtpAccounts { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        
        // Global query filter for soft delete
        modelBuilder.Entity<User>().HasQueryFilter(u => !u.IsDeleted);
        modelBuilder.Entity<OtpAccount>().HasQueryFilter(o => !o.IsDeleted);
    }
}
```

#### Entity Configurations
```csharp
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
        builder.HasIndex(u => u.Email).IsUnique();
        builder.Property(u => u.Email).IsRequired().HasMaxLength(256);
        builder.Property(u => u.PasswordHash).IsRequired();
        
        builder.HasMany(u => u.OtpAccounts)
               .WithOne(o => o.User)
               .HasForeignKey(o => o.UserId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
```

### Services Implementation

#### OtpService
```csharp
public class OtpService : IOtpService
{
    // TOTP: Time-based OTP (RFC 6238)
    public string GenerateTOTP(string secret, int digits, int period, OtpAlgorithm algorithm)
    {
        var counter = DateTimeOffset.UtcNow.ToUnixTimeSeconds() / period;
        return GenerateOtp(secret, counter, digits, algorithm);
    }
    
    // HOTP: Counter-based OTP (RFC 4226)
    public string GenerateHOTP(string secret, long counter, int digits, OtpAlgorithm algorithm)
    {
        return GenerateOtp(secret, counter, digits, algorithm);
    }
    
    private string GenerateOtp(string secret, long counter, int digits, OtpAlgorithm algorithm)
    {
        // 1. Decode Base32 secret
        // 2. Convert counter to bytes (big-endian)
        // 3. Compute HMAC (SHA1/256/512)
        // 4. Dynamic truncation
        // 5. Generate digits
    }
}
```

#### EncryptionService (AES-256)
```csharp
public class EncryptionService : IEncryptionService
{
    private readonly byte[] _key;
    private readonly byte[] _iv;
    
    public string Encrypt(string plainText)
    {
        using var aes = Aes.Create();
        aes.Key = _key;
        aes.IV = _iv;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        
        // Encrypt and return Base64
    }
    
    public string Decrypt(string cipherText)
    {
        // Reverse of encrypt
    }
}
```

## 🔺 Presentation Layer (API)

### Controllers

```
AuthController
├── POST /api/auth/register
├── POST /api/auth/login
└── POST /api/auth/refresh-token

OtpAccountsController
├── GET    /api/otpaccounts
├── POST   /api/otpaccounts
└── DELETE /api/otpaccounts/{id}

OtpController
├── GET  /api/otp/generate/{accountId}
└── POST /api/otp/verify
```

### Middleware Pipeline

```
Request
  ↓
ExceptionHandlingMiddleware
  ↓
CORS Middleware
  ↓
Authentication Middleware (JWT)
  ↓
Authorization Middleware
  ↓
Routing
  ↓
Controller Action
  ↓
Response
```

## 📊 Data Flow Examples

### Example 1: User Registration Flow

```
1. Client → POST /api/auth/register
             { email, password, firstName, lastName }

2. API Controller → MediatR.Send(RegisterCommand)

3. RegisterCommandHandler
   ├── Validate input (FluentValidation)
   ├── Check if email exists (Repository)
   ├── Hash password (IPasswordHasher)
   ├── Create User entity (Domain)
   ├── Save to database (UnitOfWork)
   ├── Generate JWT tokens (IJwtService)
   ├── Log event (IAuditService)
   └── Return AuthResponseDto

4. Response ← { accessToken, refreshToken, user }
```

### Example 2: OTP Generation Flow

```
1. Client → GET /api/otp/generate/{accountId}
            Authorization: Bearer {token}

2. API Controller → Extract userId from JWT
                  → MediatR.Send(GenerateOtpQuery)

3. GenerateOtpQueryHandler
   ├── Get OTP account (Repository)
   ├── Verify ownership (userId match)
   ├── Decrypt secret (IEncryptionService)
   ├── Generate OTP code (IOtpService)
   │   └── TOTP: time-based counter
   │   └── HOTP: increment counter
   ├── Save counter if HOTP (UnitOfWork)
   ├── Log generation (IAuditService)
   └── Return OtpCodeDto

4. Response ← { code: "123456", remainingSeconds: 25 }
```

### Example 3: OTP Verification Flow

```
1. Client → POST /api/otp/verify
            { accountId, code }
            Authorization: Bearer {token}

2. API Controller → MediatR.Send(VerifyOtpCommand)

3. VerifyOtpCommandHandler
   ├── Check rate limit (ICacheService/Redis)
   │   └── Max 5 attempts per 5 minutes
   ├── Get OTP account (Repository)
   ├── Decrypt secret (IEncryptionService)
   ├── Verify code (IOtpService)
   │   └── TOTP: time window ±30s
   │   └── HOTP: exact counter match
   ├── Log result (IAuditService)
   │   └── Success: Reset rate limit
   │   └── Failure: Increment attempts
   └── Return bool (isValid)

4. Response ← { isValid: true/false }
```

## 🔐 Security Architecture

### Authentication Flow

```
┌─────────┐                  ┌─────────┐
│ Client  │                  │   API   │
└────┬────┘                  └────┬────┘
     │                            │
     │  1. Login (email/pass)     │
     │───────────────────────────>│
     │                            │
     │  2. Validate credentials   │
     │                            │
     │  3. Generate JWT tokens    │
     │    - Access token (1h)     │
     │    - Refresh token (7d)    │
     │                            │
     │  4. Return tokens          │
     │<───────────────────────────│
     │                            │
     │  5. API request + Bearer   │
     │───────────────────────────>│
     │                            │
     │  6. Validate JWT           │
     │                            │
     │  7. Extract userId         │
     │                            │
     │  8. Process request        │
     │                            │
     │  9. Return response        │
     │<───────────────────────────│
```

### Encryption Strategy

| Data Type | Encryption Method | Key Storage |
|-----------|------------------|-------------|
| Passwords | BCrypt (one-way) | N/A |
| OTP Secrets | AES-256-CBC | Configuration/Secrets |
| JWT Tokens | HMAC-SHA256 | Configuration/Secrets |

### Rate Limiting (Redis)

```
Key: "otp_verify_attempts:{userId}:{accountId}"
Value: Counter (incremented on each attempt)
TTL: 5 minutes (sliding window)
Limit: 5 attempts

If exceeded → HTTP 400 "Too many attempts"
```

## 📈 Scalability Considerations

### Horizontal Scaling

```
┌────────┐     ┌────────────────┐     ┌──────────────┐
│ Client │────>│ Load Balancer  │────>│ API Instance │
└────────┘     └────────────────┘     └──────────────┘
                      │               ┌──────────────┐
                      └──────────────>│ API Instance │
                                      └──────────────┘
                                      ┌──────────────┐
                                      │ API Instance │
                                      └──────────────┘
                                             ↓
                      ┌──────────────────────┴─────────────┐
                      ↓                                    ↓
              ┌──────────────┐                    ┌──────────────┐
              │  PostgreSQL  │                    │    Redis     │
              │   (Master)   │                    │   Cluster    │
              └──────────────┘                    └──────────────┘
```

### Caching Strategy

- **Redis**: Session data, rate limiting, temporary OTP codes
- **Database**: Persistent data với proper indexing
- **In-Memory**: Configuration, lookup tables

### Database Optimization

- Indexes trên: `Users.Email`, `OtpAccounts.UserId`, `AuditLogs.UserId`
- Soft delete với query filters
- Connection pooling (mặc định EF Core)
- Async operations

## 🧪 Testing Strategy

```
Unit Tests
├── Domain.Tests
│   ├── Entities Tests
│   ├── Value Objects Tests
│   └── Domain Events Tests
│
├── Application.Tests
│   ├── Commands Handlers Tests
│   ├── Queries Handlers Tests
│   └── Validators Tests
│
└── Infrastructure.Tests
    ├── Services Tests
    └── Repository Tests

Integration Tests
└── API.Tests
    ├── Auth Endpoints Tests
    ├── OTP Endpoints Tests
    └── OtpAccounts Endpoints Tests
```

## 📚 Design Patterns Used

| Pattern | Usage | Location |
|---------|-------|----------|
| CQRS | Command/Query separation | Application Layer |
| MediatR | Request/Response pipeline | Application Layer |
| Repository | Data access abstraction | Infrastructure Layer |
| Unit of Work | Transaction management | Infrastructure Layer |
| Factory | Domain object creation | Domain Layer |
| Value Object | Immutable domain concepts | Domain Layer |
| Dependency Injection | IoC Container | All Layers |
| Strategy | Algorithm selection (OTP) | Infrastructure Layer |

## 🔄 Future Enhancements

- [ ] **Event Sourcing**: Lưu trữ domain events
- [ ] **GRPC Support**: Alternative API protocol
- [ ] **GraphQL**: Query flexibility
- [ ] **Microservices**: Split into smaller services
- [ ] **Message Queue**: Async processing (RabbitMQ/Kafka)
- [ ] **Multi-tenancy**: Support multiple organizations
- [ ] **Read/Write DB Separation**: CQRS với separate databases

---

**Last Updated:** November 21, 2025  
**Version:** 1.0  
**Architecture Style:** Clean Architecture + DDD + CQRS
