# Architecture Overview

## Clean Architecture Layers

```
┌─────────────────────────────────────────────────────────┐
│                    Presentation Layer                   │
│                    (SmartOTP.API)                       │
│  Controllers, Middleware, Authentication, Swagger       │
└─────────────────────────────────────────────────────────┘
                          ↓ depends on
┌─────────────────────────────────────────────────────────┐
│                  Application Layer                      │
│                 (SmartOTP.Application)                  │
│  Commands, Queries, Handlers, DTOs, Validators          │
└─────────────────────────────────────────────────────────┘
                          ↓ depends on
┌─────────────────────────────────────────────────────────┐
│                    Domain Layer                         │
│                   (SmartOTP.Domain)                     │
│  Entities, Value Objects, Domain Events, Enums          │
└─────────────────────────────────────────────────────────┘
                          ↑ implements
┌─────────────────────────────────────────────────────────┐
│                 Infrastructure Layer                    │
│                (SmartOTP.Infrastructure)                │
│  EF Core, PostgreSQL, Redis, Services, Repositories     │
└─────────────────────────────────────────────────────────┘
```

## Domain-Driven Design (DDD)

### Entities
- **User**: Aggregate root for user authentication and profile
- **OtpAccount**: Aggregate root for OTP account management
- **AuditLog**: Audit trail entity

### Value Objects
- **OtpCode**: Represents a generated OTP code with validation
- **SecretKey**: Encrypted OTP secret

### Domain Events
- UserRegisteredEvent
- UserLoggedInEvent
- OtpAccountCreatedEvent
- OtpAccountUpdatedEvent
- OtpAccountDeletedEvent
- OtpGeneratedEvent
- OtpVerifiedEvent

## CQRS Pattern

### Commands (Write Operations)
```
User Management:
  - RegisterCommand
  - LoginCommand
  - RefreshTokenCommand

OTP Account Management:
  - CreateOtpAccountCommand
  - DeleteOtpAccountCommand
  
OTP Operations:
  - VerifyOtpCommand
```

### Queries (Read Operations)
```
OTP Account:
  - GetUserOtpAccountsQuery
  
OTP Generation:
  - GenerateOtpQuery
```

## Repository Pattern

```csharp
IRepository<T>
  ├── GetByIdAsync(id)
  ├── GetAllAsync()
  ├── FindAsync(predicate)
  ├── FirstOrDefaultAsync(predicate)
  ├── AddAsync(entity)
  ├── Update(entity)
  ├── Remove(entity)
  └── AnyAsync(predicate)
```

## Unit of Work Pattern

```csharp
IUnitOfWork
  ├── SaveChangesAsync()
  ├── BeginTransactionAsync()
  ├── CommitTransactionAsync()
  └── RollbackTransactionAsync()
```

## Services

### Application Services (Interfaces)
- IEncryptionService - AES-256 encryption for OTP secrets
- IOtpService - TOTP/HOTP generation and verification
- ICacheService - Redis caching abstraction
- IJwtService - JWT token generation and validation
- IPasswordHasher - BCrypt password hashing
- IAuditService - Audit logging

### Infrastructure Services (Implementations)
All interfaces implemented in Infrastructure layer

## Data Flow

### User Registration Flow
```
1. Client → POST /api/auth/register
2. Controller → MediatR → RegisterCommandHandler
3. Handler → Validate → Hash Password → Create User Entity
4. Handler → Generate JWT Tokens
5. Handler → Repository → UnitOfWork → PostgreSQL
6. Handler → Audit Service → Log Event
7. Response ← AuthResponseDto (tokens + user)
```

### OTP Generation Flow
```
1. Client → GET /api/otp/generate/{accountId}
2. Controller → MediatR → GenerateOtpQueryHandler
3. Handler → Repository → Get OTP Account
4. Handler → Decrypt Secret → Generate OTP Code
5. Handler → Audit Service → Log Generation
6. Response ← OtpCodeDto (code + remaining seconds)
```

### OTP Verification Flow
```
1. Client → POST /api/otp/verify
2. Controller → MediatR → VerifyOtpCommandHandler
3. Handler → Cache Service → Check Rate Limit
4. Handler → Repository → Get OTP Account
5. Handler → Decrypt Secret → Verify Code
6. Handler → Cache Service → Update/Reset Limit
7. Handler → Audit Service → Log Result
8. Response ← { isValid: true/false }
```

## Security Architecture

### Authentication Flow
```
1. User logs in → Credentials validated
2. JWT Access Token generated (1 hour expiry)
3. Refresh Token generated (7 days expiry)
4. Refresh Token stored in database
5. Both tokens returned to client
```

### Token Refresh Flow
```
1. Client sends Refresh Token
2. Validate token exists and not expired
3. Generate new Access Token
4. Generate new Refresh Token
5. Update database with new Refresh Token
6. Return new token pair
```

### Encryption
- **Algorithm**: AES-256-CBC
- **Key Size**: 256 bits (32 bytes)
- **IV Size**: 128 bits (16 bytes)
- **Usage**: Encrypt OTP secrets before database storage

### Rate Limiting
- **Storage**: Redis
- **Key Format**: `otp_verify_attempts:{userId}:{accountId}`
- **Limit**: 5 attempts per 5 minutes
- **Reset**: On successful verification or expiration

## Database Schema

### Users
```sql
CREATE TABLE Users (
    Id UUID PRIMARY KEY,
    Email VARCHAR(256) UNIQUE NOT NULL,
    PasswordHash VARCHAR(512) NOT NULL,
    FirstName VARCHAR(100),
    LastName VARCHAR(100),
    IsEmailVerified BOOLEAN NOT NULL DEFAULT FALSE,
    LastLoginAt TIMESTAMP,
    RefreshToken VARCHAR(512),
    RefreshTokenExpiryTime TIMESTAMP,
    CreatedAt TIMESTAMP NOT NULL,
    UpdatedAt TIMESTAMP,
    IsDeleted BOOLEAN NOT NULL DEFAULT FALSE
);
```

### OtpAccounts
```sql
CREATE TABLE OtpAccounts (
    Id UUID PRIMARY KEY,
    UserId UUID NOT NULL REFERENCES Users(Id) ON DELETE CASCADE,
    Issuer VARCHAR(100) NOT NULL,
    AccountName VARCHAR(100) NOT NULL,
    Type VARCHAR(10) NOT NULL,
    Algorithm VARCHAR(10) NOT NULL,
    Digits INT NOT NULL,
    Period INT NOT NULL,
    Counter BIGINT NOT NULL,
    EncryptedSecret VARCHAR(1000) NOT NULL,
    SecretCreatedAt TIMESTAMP NOT NULL,
    IconUrl VARCHAR(500),
    SortOrder INT NOT NULL DEFAULT 0,
    CreatedAt TIMESTAMP NOT NULL,
    UpdatedAt TIMESTAMP,
    IsDeleted BOOLEAN NOT NULL DEFAULT FALSE
);
```

### AuditLogs
```sql
CREATE TABLE AuditLogs (
    Id UUID PRIMARY KEY,
    UserId UUID REFERENCES Users(Id) ON DELETE SET NULL,
    Action VARCHAR(50) NOT NULL,
    IpAddress VARCHAR(45),
    UserAgent VARCHAR(500),
    Details VARCHAR(2000),
    IsSuccess BOOLEAN NOT NULL,
    ErrorMessage VARCHAR(1000),
    CreatedAt TIMESTAMP NOT NULL,
    IsDeleted BOOLEAN NOT NULL DEFAULT FALSE
);
```

## Performance Considerations

### Caching Strategy
- Rate limit counters in Redis (5 min TTL)
- Future: Cache frequently accessed OTP accounts

### Database Indexes
```sql
-- Users
CREATE INDEX idx_users_email ON Users(Email);

-- OtpAccounts
CREATE INDEX idx_otpaccounts_userid_issuer_account 
  ON OtpAccounts(UserId, Issuer, AccountName);

-- AuditLogs
CREATE INDEX idx_auditlogs_userid ON AuditLogs(UserId);
CREATE INDEX idx_auditlogs_action ON AuditLogs(Action);
CREATE INDEX idx_auditlogs_createdat ON AuditLogs(CreatedAt);
```

### Query Optimization
- Soft delete with query filters
- Efficient eager/lazy loading
- Projection to DTOs for read operations

## Testing Strategy

### Unit Tests
- Domain entity behavior
- Value object validation
- Command/query handlers
- Service implementations

### Integration Tests
- API endpoints
- Database operations
- Authentication flow
- OTP generation/verification

### Test Coverage Areas
- User registration and authentication
- OTP account CRUD operations
- OTP generation (TOTP/HOTP)
- OTP verification with rate limiting
- Encryption/decryption
- Audit logging
