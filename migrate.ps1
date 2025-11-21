# Smart OTP - Database Migration Script
# This script helps manage Entity Framework Core migrations

param(
    [Parameter(Mandatory=$false)]
    [string]$Action,
    [Parameter(Mandatory=$false)]
    [string]$Name
)

$InfrastructureProject = "src\SmartOTP.Infrastructure"
$StartupProject = "src\SmartOTP.API"

function Show-Help {
    Write-Host "Smart OTP - Database Migration Helper" -ForegroundColor Cyan
    Write-Host "=====================================" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Usage:" -ForegroundColor Yellow
    Write-Host "  .\migrate.ps1 add <MigrationName>    # Add a new migration"
    Write-Host "  .\migrate.ps1 update                 # Apply migrations to database"
    Write-Host "  .\migrate.ps1 remove                 # Remove last migration"
    Write-Host "  .\migrate.ps1 list                   # List all migrations"
    Write-Host "  .\migrate.ps1 script                 # Generate SQL script"
    Write-Host ""
    Write-Host "Examples:" -ForegroundColor Cyan
    Write-Host "  .\migrate.ps1 add InitialCreate"
    Write-Host "  .\migrate.ps1 update"
    Write-Host "  .\migrate.ps1 remove"
}

function Add-Migration {
    param([string]$MigrationName)
    
    if ([string]::IsNullOrEmpty($MigrationName)) {
        Write-Host "Error: Migration name is required" -ForegroundColor Red
        Write-Host "Usage: .\migrate.ps1 add <MigrationName>" -ForegroundColor Yellow
        exit 1
    }
    
    Write-Host "Adding migration: $MigrationName" -ForegroundColor Green
    Set-Location $InfrastructureProject
    dotnet ef migrations add $MigrationName --startup-project ..\SmartOTP.API
    Set-Location ..\..
}

function Update-Database {
    Write-Host "Applying migrations to database..." -ForegroundColor Green
    Set-Location $InfrastructureProject
    dotnet ef database update --startup-project ..\SmartOTP.API
    Set-Location ..\..
    Write-Host "Database updated successfully!" -ForegroundColor Green
}

function Remove-LastMigration {
    Write-Host "Removing last migration..." -ForegroundColor Yellow
    Set-Location $InfrastructureProject
    dotnet ef migrations remove --startup-project ..\SmartOTP.API
    Set-Location ..\..
}

function List-Migrations {
    Write-Host "Listing migrations..." -ForegroundColor Cyan
    Set-Location $InfrastructureProject
    dotnet ef migrations list --startup-project ..\SmartOTP.API
    Set-Location ..\..
}

function Generate-Script {
    Write-Host "Generating SQL migration script..." -ForegroundColor Cyan
    $scriptPath = "migration-script.sql"
    Set-Location $InfrastructureProject
    dotnet ef migrations script --startup-project ..\SmartOTP.API --output ..\..\$scriptPath
    Set-Location ..\..
    Write-Host "SQL script generated: $scriptPath" -ForegroundColor Green
}

# Main script logic
switch ($Action.ToLower()) {
    "add" {
        Add-Migration -MigrationName $Name
    }
    "update" {
        Update-Database
    }
    "remove" {
        Remove-LastMigration
    }
    "list" {
        List-Migrations
    }
    "script" {
        Generate-Script
    }
    default {
        Show-Help
    }
}
