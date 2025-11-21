# Smart OTP - Quick Start Script
# This script helps you get started with the Smart OTP backend

param(
    [switch]$Docker,
    [switch]$Local
)

Write-Host "Smart OTP - Quick Start" -ForegroundColor Cyan
Write-Host "=======================" -ForegroundColor Cyan
Write-Host ""

if ($Docker) {
    Write-Host "Starting Smart OTP with Docker Compose..." -ForegroundColor Green
    Write-Host ""
    
    # Check if docker-compose is available
    if (!(Get-Command docker-compose -ErrorAction SilentlyContinue)) {
        Write-Host "Error: docker-compose is not installed or not in PATH" -ForegroundColor Red
        exit 1
    }
    
    # Check if encryption keys are set
    Write-Host "Checking configuration..." -ForegroundColor Yellow
    $config = Get-Content "src\SmartOTP.API\appsettings.json" | ConvertFrom-Json
    
    if ($config.Encryption.Key -eq "CHANGE_THIS_TO_A_32_BYTE_BASE64_KEY") {
        Write-Host ""
        Write-Host "WARNING: Encryption keys not configured!" -ForegroundColor Red
        Write-Host "Please run: .\generate-keys.ps1" -ForegroundColor Yellow
        Write-Host "Then update src\SmartOTP.API\appsettings.json with the generated keys" -ForegroundColor Yellow
        Write-Host ""
        $continue = Read-Host "Continue anyway? (y/n)"
        if ($continue -ne "y") {
            exit 1
        }
    }
    
    Write-Host "Starting containers..." -ForegroundColor Green
    docker-compose up -d
    
    Write-Host ""
    Write-Host "Smart OTP is starting..." -ForegroundColor Green
    Write-Host "API: http://localhost:5000" -ForegroundColor Cyan
    Write-Host "Swagger: http://localhost:5000/swagger" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "PostgreSQL: localhost:5432" -ForegroundColor Cyan
    Write-Host "Redis: localhost:6379" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "To view logs: docker-compose logs -f" -ForegroundColor Yellow
    Write-Host "To stop: docker-compose down" -ForegroundColor Yellow
}
elseif ($Local) {
    Write-Host "Starting Smart OTP locally..." -ForegroundColor Green
    Write-Host ""
    
    # Check if .NET 9 is installed
    $dotnetVersion = dotnet --version
    if (!$dotnetVersion.StartsWith("9.")) {
        Write-Host "Error: .NET 9 SDK is required" -ForegroundColor Red
        Write-Host "Current version: $dotnetVersion" -ForegroundColor Yellow
        Write-Host "Download from: https://dotnet.microsoft.com/download/dotnet/9.0" -ForegroundColor Cyan
        exit 1
    }
    
    Write-Host "Restoring packages..." -ForegroundColor Yellow
    Set-Location src\SmartOTP.API
    dotnet restore
    
    Write-Host ""
    Write-Host "Building solution..." -ForegroundColor Yellow
    dotnet build
    
    Write-Host ""
    Write-Host "Applying database migrations..." -ForegroundColor Yellow
    Set-Location ..\SmartOTP.Infrastructure
    dotnet ef database update --startup-project ..\SmartOTP.API
    
    Write-Host ""
    Write-Host "Starting API..." -ForegroundColor Green
    Set-Location ..\SmartOTP.API
    dotnet run
}
else {
    Write-Host "Usage:" -ForegroundColor Yellow
    Write-Host "  .\quick-start.ps1 -Docker   # Start with Docker Compose"
    Write-Host "  .\quick-start.ps1 -Local    # Run locally"
    Write-Host ""
    Write-Host "Before running, make sure to:" -ForegroundColor Cyan
    Write-Host "  1. Run .\generate-keys.ps1 to generate encryption keys"
    Write-Host "  2. Update src\SmartOTP.API\appsettings.json with the keys"
    Write-Host "  3. Ensure PostgreSQL and Redis are running (if running locally)"
}
