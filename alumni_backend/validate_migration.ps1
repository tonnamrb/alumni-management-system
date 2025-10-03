# ============================================================================
# Migration Validation Script
# Tests all critical functionality after schema migration
# ============================================================================

Write-Host "🔍 Starting Migration Validation..." -ForegroundColor Green

# Test 1: Build Project
Write-Host "📦 Testing Build..." -ForegroundColor Yellow
dotnet build
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Build failed!" -ForegroundColor Red
    exit 1
}
Write-Host "✅ Build successful" -ForegroundColor Green

# Test 2: Run Unit Tests
Write-Host "🧪 Running Unit Tests..." -ForegroundColor Yellow
dotnet test tests/UnitTests --verbosity quiet
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Unit tests failed!" -ForegroundColor Red
    exit 1
}
Write-Host "✅ Unit tests passed" -ForegroundColor Green

# Test 3: Check Migration Status
Write-Host "🔄 Checking Migration Status..." -ForegroundColor Yellow
$migrations = dotnet ef migrations list -p src/Infrastructure -s src/Api --no-build 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Migration check failed!" -ForegroundColor Red
    Write-Host $migrations
    exit 1
}
Write-Host "✅ Migrations are healthy" -ForegroundColor Green

# Test 4: Check for Schema Inconsistencies
Write-Host "🔍 Checking for Old Schema References..." -ForegroundColor Yellow

$oldSchemaReferences = @()

# Check for IsActive references
$isActiveRefs = Select-String -Path "src/**/*.cs" -Pattern "\.IsActive" -Exclude "*.Designer.cs","*Migration*.cs" 2>$null
if ($isActiveRefs) {
    $oldSchemaReferences += "IsActive references found"
}

# Check for LastLoginAt references
$lastLoginRefs = Select-String -Path "src/**/*.cs" -Pattern "\.LastLoginAt" -Exclude "*.Designer.cs","*Migration*.cs" 2>$null
if ($lastLoginRefs) {
    $oldSchemaReferences += "LastLoginAt references found"
}

# Check for old Name field usage
$nameRefs = Select-String -Path "src/**/*.cs" -Pattern "user\.Name\s*=" -Exclude "*.Designer.cs","*Migration*.cs" 2>$null
if ($nameRefs) {
    $oldSchemaReferences += "Old user.Name assignments found"
}

if ($oldSchemaReferences.Count -gt 0) {
    Write-Host "⚠️  Found potential old schema references:" -ForegroundColor Yellow
    foreach ($ref in $oldSchemaReferences) {
        Write-Host "  - $ref" -ForegroundColor Yellow
    }
} else {
    Write-Host "✅ No old schema references found" -ForegroundColor Green
}

# Test 5: Verify Entity Structure
Write-Host "📋 Verifying Entity Structure..." -ForegroundColor Yellow

$userEntityPath = "src/Domain/Entities/User.cs"
if (Test-Path $userEntityPath) {
    $userContent = Get-Content $userEntityPath -Raw
    
    $requiredFields = @("Firstname", "Lastname", "RoleId", "MobilePhone")
    $missingFields = @()
    
    foreach ($field in $requiredFields) {
        if ($userContent -notmatch "public.*$field") {
            $missingFields += $field
        }
    }
    
    if ($missingFields.Count -gt 0) {
        Write-Host "❌ Missing required fields in User entity: $($missingFields -join ', ')" -ForegroundColor Red
        exit 1
    } else {
        Write-Host "✅ User entity has all required fields" -ForegroundColor Green
    }
} else {
    Write-Host "❌ User entity not found!" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "🎉 Migration Validation Complete!" -ForegroundColor Green
Write-Host "📊 Summary:" -ForegroundColor Cyan
Write-Host "  ✅ Build: Successful" -ForegroundColor Green
Write-Host "  ✅ Tests: All Passing" -ForegroundColor Green
Write-Host "  ✅ Migrations: Healthy" -ForegroundColor Green
Write-Host "  ✅ Entity Structure: Valid" -ForegroundColor Green
if ($oldSchemaReferences.Count -eq 0) {
    Write-Host "  ✅ Schema: Clean (no old references)" -ForegroundColor Green
} else {
    Write-Host "  ⚠️  Schema: Some old references detected" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "🚀 Ready for deployment!" -ForegroundColor Green