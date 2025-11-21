# Smart OTP - Generate Encryption Keys
# This script generates AES-256 encryption keys for the Smart OTP application

Write-Host "Smart OTP - Encryption Key Generator" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""

# Generate AES-256 key (32 bytes)
$aes = [System.Security.Cryptography.Aes]::Create()
$aes.KeySize = 256

$key = [System.Convert]::ToBase64String($aes.Key)
$iv = [System.Convert]::ToBase64String($aes.IV)

Write-Host "Generated AES-256 Encryption Keys:" -ForegroundColor Green
Write-Host ""
Write-Host "Encryption Key (32 bytes):" -ForegroundColor Yellow
Write-Host $key
Write-Host ""
Write-Host "Encryption IV (16 bytes):" -ForegroundColor Yellow
Write-Host $iv
Write-Host ""

Write-Host "Update your appsettings.json with these values:" -ForegroundColor Cyan
Write-Host ""
Write-Host '{' -ForegroundColor White
Write-Host '  "Encryption": {' -ForegroundColor White
Write-Host "    `"Key`": `"$key`"," -ForegroundColor White
Write-Host "    `"IV`": `"$iv`"" -ForegroundColor White
Write-Host '  }' -ForegroundColor White
Write-Host '}' -ForegroundColor White
Write-Host ""

# Generate JWT Secret (at least 32 characters)
$jwtSecretBytes = New-Object byte[] 64
$rng = [System.Security.Cryptography.RandomNumberGenerator]::Create()
$rng.GetBytes($jwtSecretBytes)
$jwtSecret = [System.Convert]::ToBase64String($jwtSecretBytes)

Write-Host "JWT Secret (64 bytes):" -ForegroundColor Yellow
Write-Host $jwtSecret
Write-Host ""

Write-Host "Update your JWT configuration:" -ForegroundColor Cyan
Write-Host ""
Write-Host '{' -ForegroundColor White
Write-Host '  "Jwt": {' -ForegroundColor White
Write-Host "    `"Secret`": `"$jwtSecret`"," -ForegroundColor White
Write-Host '    "Issuer": "SmartOTP",' -ForegroundColor White
Write-Host '    "Audience": "SmartOTP.API",' -ForegroundColor White
Write-Host '    "AccessTokenExpirationMinutes": "60"' -ForegroundColor White
Write-Host '  }' -ForegroundColor White
Write-Host '}' -ForegroundColor White
Write-Host ""

Write-Host "IMPORTANT: Keep these keys secret and secure!" -ForegroundColor Red
Write-Host "Do not commit them to version control!" -ForegroundColor Red
