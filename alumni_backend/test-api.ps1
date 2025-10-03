# Alumni API Testing Script
# สำหรับทดสอบ API endpoints โดยไม่ทำให้ API shutdown

param(
    [string]$BaseUrl = "http://localhost:5000",
    [string]$TestType = "basic"
)

Write-Host "=== Alumni API Testing ===" -ForegroundColor Cyan
Write-Host "Base URL: $BaseUrl" -ForegroundColor Yellow
Write-Host "Test Type: $TestType" -ForegroundColor Yellow

# Function to test endpoint with error handling
function Test-Endpoint {
    param(
        [string]$Url,
        [string]$Method = "GET",
        [string]$Body = $null,
        [hashtable]$Headers = @{}
    )
    
    try {
        Write-Host "Testing: $Method $Url" -ForegroundColor Cyan
        
        $params = @{
            Uri = $Url
            Method = $Method
            UseBasicParsing = $true
            TimeoutSec = 10
        }
        
        if ($Headers.Count -gt 0) {
            $params.Headers = $Headers
        }
        
        if ($Body) {
            $params.Body = $Body
            $params.ContentType = "application/json"
        }
        
        $response = Invoke-WebRequest @params
        Write-Host "✅ Status: $($response.StatusCode)" -ForegroundColor Green
        
        if ($response.Content) {
            $contentPreview = if ($response.Content.Length -gt 200) { 
                $response.Content.Substring(0, 200) + "..." 
            } else { 
                $response.Content 
            }
            Write-Host "Response: $contentPreview" -ForegroundColor Gray
        }
        
        return $true
        
    } catch {
        Write-Host "❌ Error: $($_.Exception.Message)" -ForegroundColor Red
        if ($_.Exception.Response) {
            Write-Host "Status Code: $($_.Exception.Response.StatusCode)" -ForegroundColor Yellow
        }
        return $false
    }
    
    Write-Host ""
}

# Check if API is running first
Write-Host "Checking API health..." -ForegroundColor Yellow
$healthCheck = Test-Endpoint "$BaseUrl/health"

if (-not $healthCheck) {
    Write-Host "❌ API is not running or not responding!" -ForegroundColor Red
    Write-Host "Please start the API first using: .\start-api.ps1 -Mode background" -ForegroundColor Cyan
    exit 1
}

Write-Host "✅ API is running! Starting tests..." -ForegroundColor Green
Write-Host ""

switch ($TestType) {
    "basic" {
        # Basic endpoint tests
        Write-Host "=== Basic API Tests ===" -ForegroundColor Cyan
        
        Test-Endpoint "$BaseUrl/health"
        Test-Endpoint "$BaseUrl/health/ready"
        Test-Endpoint "$BaseUrl/swagger/v1/swagger.json"
        
        # Test endpoints (no auth required)
        Test-Endpoint "$BaseUrl/api/v1/test"
        Test-Endpoint "$BaseUrl/api/v1/test/status"
        Test-Endpoint "$BaseUrl/api/v1/test/echo" "POST" '{"test":"data","message":"hello world"}'
        
        # Posts endpoint (allows anonymous) - should return sample data
        Test-Endpoint "$BaseUrl/api/v1/posts"
        
        # Auth endpoints
        Test-Endpoint "$BaseUrl/api/v1/auth/status"
        Test-Endpoint "$BaseUrl/api/v1/auth/login" "POST" '{"email":"test@example.com","password":"password123"}'
    }
    
    "auth" {
        # Authentication tests
        Write-Host "=== Authentication Tests ===" -ForegroundColor Cyan
        
        # Test registration
        $registerBody = @{
            firstName = "Test"
            lastName = "User"
            email = "test@alumni.com"
            password = "Password123!"
            graduationYear = 2020
        } | ConvertTo-Json
        
        Test-Endpoint "$BaseUrl/api/v1/auth/register" "POST" $registerBody
        
        # Test login
        $loginBody = @{
            email = "test@alumni.com"
            password = "Password123!"
        } | ConvertTo-Json
        
        Test-Endpoint "$BaseUrl/api/v1/auth/login" "POST" $loginBody
    }
    
    "full" {
        # Full test suite
        Write-Host "=== Full API Test Suite ===" -ForegroundColor Cyan
        
        # Run basic tests first
        & $MyInvocation.MyCommand.Path -BaseUrl $BaseUrl -TestType "basic"
        
        Write-Host ""
        
        # Run auth tests
        & $MyInvocation.MyCommand.Path -BaseUrl $BaseUrl -TestType "auth"
    }
}

Write-Host ""
Write-Host "=== Test Complete ===" -ForegroundColor Green