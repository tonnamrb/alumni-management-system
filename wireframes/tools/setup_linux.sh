#!/bin/bash
# Agent Visual Workflow - Linux Setup Script  
# Bash script สำหรับติดตั้ง dependencies บน Ubuntu/Debian

echo "🚀 Agent Visual Workflow - Linux Setup"
echo "======================================"

# Detect Linux distribution
if [ -f /etc/os-release ]; then
    . /etc/os-release
    OS=$ID
    echo "✅ Detected: $PRETTY_NAME"
else
    echo "⚠️  Cannot detect Linux distribution. Assuming Ubuntu/Debian."
    OS="ubuntu"
fi

# Update package list
echo ""
echo "📦 Updating package list..."
if sudo apt update; then
    echo "✅ Package list updated"
else
    echo "❌ Failed to update package list"
    exit 1
fi

# Install Python and system dependencies
echo ""
echo "📦 Installing Python and system dependencies..."
if sudo apt install -y python3 python3-pip python3-dev; then
    echo "✅ Python installed"
else
    echo "❌ Failed to install Python"
    exit 1
fi

# Check Python version
python_version=$(python3 --version 2>&1 | cut -d' ' -f2 | cut -d'.' -f1-2)
echo "✅ Python version: $python_version"

if python3 -c "import sys; exit(0 if sys.version_info >= (3, 8) else 1)"; then
    echo "✅ Python version is compatible"
else
    echo "❌ Python version $python_version is too old. Need Python 3.8+"
    echo "   Consider upgrading your system or installing Python 3.8+ manually"
    exit 1
fi

# Install Python dependencies
echo ""
echo "📦 Installing Python dependencies..."
if pip3 install -r requirements.txt; then
    echo "✅ Python dependencies installed successfully"
else
    echo "❌ Failed to install Python dependencies"
    echo "   Trying with user install..."
    if pip3 install --user -r requirements.txt; then
        echo "✅ Python dependencies installed (user mode)"
    else
        echo "❌ Failed to install Python dependencies"
        exit 1
    fi
fi

# Check if user wants system dependencies for OCR
echo ""
echo "🔍 Optional: System dependencies for OCR and Computer Vision"
read -p "Do you want to install Tesseract OCR and OpenCV? (y/n) [n]: " install_deps
if [[ $install_deps == "y" || $install_deps == "Y" ]]; then
    echo "📦 Installing system dependencies..."
    
    # Install Tesseract OCR
    if sudo apt install -y tesseract-ocr tesseract-ocr-tha tesseract-ocr-eng; then
        echo "✅ Tesseract OCR installed"
    else
        echo "⚠️  Failed to install Tesseract OCR"
    fi
    
    # Install OpenCV dependencies
    if sudo apt install -y libopencv-dev python3-opencv; then
        echo "✅ OpenCV installed"
    else
        echo "⚠️  Failed to install OpenCV"
    fi
else
    echo "⏭️  Skipping system dependencies installation"
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
echo "Note: Use 'python3' instead of 'python' on Linux"
echo "If pip3 commands failed, try adding ~/.local/bin to your PATH:"
echo "export PATH=\"\$HOME/.local/bin:\$PATH\""