@echo off
REM Agent Visual Workflow - Windows Setup (Batch version)
REM Double-click to run setup

echo 🚀 Agent Visual Workflow - Windows Setup
echo =======================================
echo.

REM Check if Python is installed
echo 📦 Checking Python installation...
python --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ❌ Python not found. 
    echo    Please install Python 3.8+ from https://python.org
    echo    Or run: winget install Python.Python.3.11
    pause
    exit /b 1
)

for /f "tokens=2" %%i in ('python --version 2^>^&1') do set python_version=%%i
echo ✅ Found: Python %python_version%

REM Install Python dependencies
echo.
echo 📦 Installing Python dependencies...
pip install -r requirements.txt
if %errorlevel% neq 0 (
    echo ❌ Failed to install Python dependencies
    echo    Please check your internet connection and try again
    pause
    exit /b 1
)
echo ✅ Python dependencies installed successfully

REM Test installation
echo.
echo 🧪 Testing installation...
python agent_visual_workflow.py --help >nul 2>&1
if %errorlevel% neq 0 (
    echo ❌ agent_visual_workflow.py test failed
) else (
    echo ✅ agent_visual_workflow.py is working
)

python layout_helper.py --help >nul 2>&1
if %errorlevel% neq 0 (
    echo ❌ layout_helper.py test failed
) else (
    echo ✅ layout_helper.py is working
)

echo.
echo 🎉 Setup completed!
echo ==================
echo.
echo Next steps:
echo 1. Use: python agent_visual_workflow.py --request-image SC-XX
echo 2. Use: python layout_helper.py --generate-flutter-layout
echo 3. See README.md for complete usage guide
echo.
echo Press any key to continue...
pause >nul