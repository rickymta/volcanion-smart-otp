# Smart OTP API Examples

This file contains example API requests for testing the Smart OTP backend.

## 1. Register User

```http
POST http://localhost:5000/api/auth/register
Content-Type: application/json

{
  "email": "john.doe@example.com",
  "password": "SecurePass123!@#",
  "firstName": "John",
  "lastName": "Doe"
}
```

**Response:**
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIs...",
  "refreshToken": "base64_encoded_token",
  "accessTokenExpiresAt": "2024-01-01T13:00:00Z",
  "refreshTokenExpiresAt": "2024-01-08T12:00:00Z",
  "user": {
    "id": "guid",
    "email": "john.doe@example.com",
    "firstName": "John",
    "lastName": "Doe",
    "isEmailVerified": false,
    "createdAt": "2024-01-01T12:00:00Z"
  }
}
```

## 2. Login

```http
POST http://localhost:5000/api/auth/login
Content-Type: application/json

{
  "email": "john.doe@example.com",
  "password": "SecurePass123!@#"
}
```

## 3. Refresh Token

```http
POST http://localhost:5000/api/auth/refresh-token
Content-Type: application/json

{
  "refreshToken": "your_refresh_token_here"
}
```

## 4. Create OTP Account (TOTP)

```http
POST http://localhost:5000/api/otpaccounts
Content-Type: application/json
Authorization: Bearer YOUR_ACCESS_TOKEN

{
  "issuer": "GitHub",
  "accountName": "john.doe@example.com",
  "type": "TOTP",
  "algorithm": "SHA1",
  "digits": 6,
  "period": 30
}
```

**Response:**
```json
{
  "id": "guid",
  "issuer": "GitHub",
  "accountName": "john.doe@example.com",
  "type": "TOTP",
  "algorithm": "SHA1",
  "digits": 6,
  "period": 30,
  "counter": 0,
  "sortOrder": 0,
  "createdAt": "2024-01-01T12:00:00Z"
}
```

## 5. Create OTP Account (HOTP)

```http
POST http://localhost:5000/api/otpaccounts
Content-Type: application/json
Authorization: Bearer YOUR_ACCESS_TOKEN

{
  "issuer": "Google",
  "accountName": "john.doe@gmail.com",
  "type": "HOTP",
  "algorithm": "SHA1",
  "digits": 6,
  "counter": 0
}
```

## 6. Get User's OTP Accounts

```http
GET http://localhost:5000/api/otpaccounts
Authorization: Bearer YOUR_ACCESS_TOKEN
```

**Response:**
```json
[
  {
    "id": "guid1",
    "issuer": "GitHub",
    "accountName": "john.doe@example.com",
    "type": "TOTP",
    "algorithm": "SHA1",
    "digits": 6,
    "period": 30,
    "counter": 0,
    "sortOrder": 0,
    "createdAt": "2024-01-01T12:00:00Z"
  },
  {
    "id": "guid2",
    "issuer": "Google",
    "accountName": "john.doe@gmail.com",
    "type": "HOTP",
    "algorithm": "SHA1",
    "digits": 6,
    "counter": 5,
    "sortOrder": 1,
    "createdAt": "2024-01-01T12:05:00Z"
  }
]
```

## 7. Generate OTP Code

```http
GET http://localhost:5000/api/otp/generate/{accountId}
Authorization: Bearer YOUR_ACCESS_TOKEN
```

**Response:**
```json
{
  "code": "123456",
  "remainingSeconds": 25,
  "generatedAt": "2024-01-01T12:00:00Z"
}
```

## 8. Verify OTP Code

```http
POST http://localhost:5000/api/otp/verify
Content-Type: application/json
Authorization: Bearer YOUR_ACCESS_TOKEN

{
  "accountId": "your_account_guid",
  "code": "123456"
}
```

**Response:**
```json
{
  "isValid": true
}
```

## 9. Delete OTP Account

```http
DELETE http://localhost:5000/api/otpaccounts/{accountId}
Authorization: Bearer YOUR_ACCESS_TOKEN
```

**Response:** 204 No Content

---

## Testing with cURL

### Register
```bash
curl -X POST http://localhost:5000/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    "password": "SecurePass123!@#",
    "firstName": "Test",
    "lastName": "User"
  }'
```

### Login
```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    "password": "SecurePass123!@#"
  }'
```

### Create OTP Account
```bash
curl -X POST http://localhost:5000/api/otpaccounts \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN_HERE" \
  -d '{
    "issuer": "GitHub",
    "accountName": "test@example.com",
    "type": "TOTP",
    "algorithm": "SHA1",
    "digits": 6,
    "period": 30
  }'
```

### Generate OTP
```bash
curl -X GET "http://localhost:5000/api/otp/generate/ACCOUNT_ID_HERE" \
  -H "Authorization: Bearer YOUR_TOKEN_HERE"
```

### Verify OTP
```bash
curl -X POST http://localhost:5000/api/otp/verify \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN_HERE" \
  -d '{
    "accountId": "ACCOUNT_ID_HERE",
    "code": "123456"
  }'
```

---

## Rate Limiting

The OTP verification endpoint is rate-limited to **5 attempts per 5 minutes** per user per account.

If you exceed the limit, you'll receive:
```json
{
  "statusCode": 400,
  "message": "Too many verification attempts. Please try again later."
}
```

---

## Error Responses

### 400 Bad Request
```json
{
  "statusCode": 400,
  "message": "Error description"
}
```

### 401 Unauthorized
```json
{
  "statusCode": 401,
  "message": "Invalid email or password"
}
```

### 404 Not Found
```json
{
  "statusCode": 404,
  "message": "Resource not found"
}
```

### 500 Internal Server Error
```json
{
  "statusCode": 500,
  "message": "An internal server error occurred"
}
```
