#!/bin/bash
# Agent Visual Workflow - macOS Setup Script
# Bash script สำหรับติดตั้ง dependencies บน macOS

echo "🚀 Agent Visual Workflow - macOS Setup"
echo "======================================"

# Check if Homebrew is installed
echo ""
echo "📦 Checking Homebrew installation..."
if ! command -v brew &> /dev/null; then
    echo "❌ Homebrew not found. Installing Homebrew..."
    /bin/bash -c "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)"
    
    # Add Homebrew to PATH for Apple Silicon Macs
    if [[ $(uname -m) == "arm64" ]]; then
        echo 'eval "$(/opt/homebrew/bin/brew shellenv)"' >> ~/.zprofile
        eval "$(/opt/homebrew/bin/brew shellenv)"
    fi
else
    echo "✅ Homebrew is installed"
fi

# Check if Python is installed
echo ""
echo "📦 Checking Python installation..."
if ! command -v python3 &> /dev/null; then
    echo "❌ Python not found. Installing Python..."
    brew install python@3.11
else
    python_version=$(python3 --version 2>&1 | cut -d' ' -f2 | cut -d'.' -f1-2)
    echo "✅ Found: Python $python_version"
    
    # Check Python version
    if python3 -c "import sys; exit(0 if sys.version_info >= (3, 8) else 1)"; then
        echo "✅ Python version is compatible"
    else
        echo "❌ Python version $python_version is too old. Installing Python 3.11..."
        brew install python@3.11
    fi
fi

# Install Python dependencies
echo ""
echo "📦 Installing Python dependencies..."
if pip3 install -r requirements.txt; then
    echo "✅ Python dependencies installed successfully"
else
    echo "❌ Failed to install Python dependencies"
    echo "   Please run manually: pip3 install -r requirements.txt"
    exit 1
fi

# Check if user wants Tesseract OCR
echo ""
echo "🔍 Optional: Tesseract OCR for text recognition"
read -p "Do you want to install Tesseract OCR? (y/n) [n]: " install_tesseract
if [[ $install_tesseract == "y" || $install_tesseract == "Y" ]]; then
    echo "📦 Installing Tesseract OCR and OpenCV..."
    brew install tesseract
    brew install opencv
    echo "✅ Tesseract and OpenCV installed"
else
    echo "⏭️  Skipping Tesseract OCR installation"
fi

# Verify installation
echo ""
echo "🧪 Verifying installation..."
if python3 agent_visual_workflow.py --help > /dev/null 2>&1; then
    echo "✅ agent_visual_workflow.py is working"
else
    echo "❌ agent_visual_workflow.py test failed"
fi

if python3 layout_helper.py --help > /dev/null 2>&1; then
    echo "✅ layout_helper.py is working"
else
    echo "❌ layout_helper.py test failed"
fi

echo ""
echo "🎉 Setup completed!"
echo "=================="
echo ""
echo "Next steps:"
echo "1. Use: python3 agent_visual_workflow.py --request-image SC-XX"
echo "2. Use: python3 layout_helper.py --generate-flutter-layout"
echo "3. See README.md for complete usage guide"
echo ""
echo "Note: Use 'python3' instead of 'python' on macOS"