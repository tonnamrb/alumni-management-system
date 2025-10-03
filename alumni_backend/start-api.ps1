# Alumni Backend API Startup Script
# สำหรับเริ่ม API ในพื้นหลังแยกจากการทดสอบ

param(
    [string]$Mode = "development"
)

Write-Host "=== Alumni Backend API Startup ===" -ForegroundColor Cyan
Write-Host "Mode: $Mode" -ForegroundColor Yellow

# Navigate to API directory
$apiPath = "d:\fusionware\use_project\alumni_v1\alumni_backend\src\Api"
Set-Location $apiPath

# Stop any existing dotnet processes
Write-Host "Stopping existing dotnet processes..." -ForegroundColor Yellow
Get-Process -Name "dotnet" -ErrorAction SilentlyContinue | Stop-Process -Force

# Wait a moment for processes to stop
Start-Sleep -Seconds 2

# Start API in background
Write-Host "Starting Alumni API..." -ForegroundColor Green

if ($Mode -eq "background") {
    # Start API in background job
    $job = Start-Job -ScriptBlock {
        Set-Location "d:\fusionware\use_project\alumni_v1\alumni_backend\src\Api"
        dotnet run
    }
    
    Write-Host "API started in background job (ID: $($job.Id))" -ForegroundColor Green
    Write-Host "Use 'Get-Job' to check status" -ForegroundColor Cyan
    Write-Host "Use 'Stop-Job -Id $($job.Id)' to stop" -ForegroundColor Cyan
    
    # Wait for API to start
    Start-Sleep -Seconds 10
    
    # Test if API is running
    try {
        $response = Invoke-WebRequest -Uri "http://localhost:5000/health" -Method GET -TimeoutSec 5 -UseBasicParsing
        Write-Host "✅ API Health Check: $($response.StatusCode)" -ForegroundColor Green
    } catch {
        Write-Host "❌ API not responding yet, please wait..." -ForegroundColor Red
    }
    
} else {
    # Start API in foreground (development mode)
    Write-Host "Starting API in development mode..." -ForegroundColor Green
    Write-Host "Press Ctrl+C to stop" -ForegroundColor Yellow
    dotnet run
}