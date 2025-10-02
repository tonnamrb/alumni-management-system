# Agent Visual Workflow - Windows Setup Script
# PowerShell script สำหรับติดตั้ง dependencies บน Windows

Write-Host "🚀 Agent Visual Workflow - Windows Setup" -ForegroundColor Green
Write-Host "=======================================" -ForegroundColor Green

# Check if Python is installed
Write-Host "`n📦 Checking Python installation..." -ForegroundColor Yellow
try {
    $pythonVersion = python --version 2>&1
    Write-Host "✅ Found: $pythonVersion" -ForegroundColor Green
} catch {
    Write-Host "❌ Python not found. Please install Python 3.8+ from https://python.org" -ForegroundColor Red
    Write-Host "   Or run: winget install Python.Python.3.11" -ForegroundColor Cyan
    exit 1
}

# Check Python version
$version = python -c "import sys; print(f'{sys.version_info.major}.{sys.version_info.minor}')" 2>$null
$versionFloat = [float]$version
if ($versionFloat -lt 3.8) {
    Write-Host "❌ Python version $version is too old. Please install Python 3.8+" -ForegroundColor Red
    exit 1
}
Write-Host "✅ Python version $version is compatible" -ForegroundColor Green

# Install Python dependencies
Write-Host "`n📦 Installing Python dependencies..." -ForegroundColor Yellow
try {
    pip install -r requirements.txt
    Write-Host "✅ Python dependencies installed successfully" -ForegroundColor Green
} catch {
    Write-Host "❌ Failed to install Python dependencies" -ForegroundColor Red
    Write-Host "   Please run manually: pip install -r requirements.txt" -ForegroundColor Cyan
    exit 1
}

# Check if Tesseract is needed/wanted
Write-Host "`n🔍 Optional: Tesseract OCR for text recognition" -ForegroundColor Yellow
$installTesseract = Read-Host "Do you want to install Tesseract OCR? (y/n) [n]"
if ($installTesseract -eq "y" -or $installTesseract -eq "Y") {
    Write-Host "📦 Installing Tesseract OCR..." -ForegroundColor Yellow
    try {
        # Check if Chocolatey is available
        choco --version 2>$null
        if ($LASTEXITCODE -eq 0) {
            choco install tesseract -y
            Write-Host "✅ Tesseract installed via Chocolatey" -ForegroundColor Green
        } else {
            Write-Host "⚠️  Chocolatey not found. Please install Tesseract manually:" -ForegroundColor Yellow
            Write-Host "   Download from: https://github.com/UB-Mannheim/tesseract/wiki" -ForegroundColor Cyan
        }
    } catch {
        Write-Host "⚠️  Please install Tesseract manually from:" -ForegroundColor Yellow
        Write-Host "   https://github.com/UB-Mannheim/tesseract/wiki" -ForegroundColor Cyan
    }
} else {
    Write-Host "⏭️  Skipping Tesseract OCR installation" -ForegroundColor Gray
}

# Verify installation
Write-Host "`n🧪 Verifying installation..." -ForegroundColor Yellow
try {
    python agent_visual_workflow.py --help > $null 2>&1
    Write-Host "✅ agent_visual_workflow.py is working" -ForegroundColor Green
} catch {
    Write-Host "❌ agent_visual_workflow.py test failed" -ForegroundColor Red
}

try {
    python layout_helper.py --help > $null 2>&1
    Write-Host "✅ layout_helper.py is working" -ForegroundColor Green
} catch {
    Write-Host "❌ layout_helper.py test failed" -ForegroundColor Red
}

Write-Host "`n🎉 Setup completed!" -ForegroundColor Green
Write-Host "=================`n" -ForegroundColor Green

Write-Host "Next steps:" -ForegroundColor Cyan
Write-Host "1. Use: python agent_visual_workflow.py --request-image SC-XX" -ForegroundColor White
Write-Host "2. Use: python layout_helper.py --generate-flutter-layout" -ForegroundColor White
Write-Host "3. See README.md for complete usage guide" -ForegroundColor White

Write-Host "`nPress any key to continue..." -ForegroundColor Gray
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")