@echo off
REM Agent Visual Workflow - Windows Batch Script
REM Updated for cross-platform compatibility

setlocal enabledelayedexpansion
set SCRIPT_DIR=%~dp0

REM ตรวจสอบว่ามี Python หรือไม่
echo Checking Python installation...
python --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ❌ Python not found. Please run setup_windows.bat first
    echo    Or install Python from https://python.org
    pause
    exit /b 1
)

set TOOLS_DIR=%SCRIPT_DIR%python --version >nul 2>&1

set PROJECT_ROOT=%SCRIPT_DIR%..\..\if %ERRORLEVEL% neq 0 (

    echo ❌ Python ไม่พบในระบบ

REM Check Python    echo กรุณาติดตั้ง Python จาก https://python.org

python --version >nul 2>&1    pause

if errorlevel 1 (    exit /b 1

    echo ❌ Python not found. Install Python 3.8+)

    exit /b 1

)REM ตรวจสอบ dependencies

if not exist "wireframes\tools\requirements.txt" (

REM Check dependencies    echo ❌ ไม่พบไฟล์ requirements.txt

python -c "import easyocr, cv2, yaml" >nul 2>&1    pause

if errorlevel 1 (    exit /b 1

    echo 📦 Installing dependencies...)

    pip install -r "%TOOLS_DIR%requirements.txt"

    if errorlevel 1 (REM ติดตั้ง dependencies ถ้ายังไม่ได้ติดตั้ง

        echo ❌ Failed to install dependenciesecho 🔧 ตรวจสอบ Python dependencies...

        exit /b 1pip install -r wireframes\tools\requirements.txt >nul 2>&1

    )

)REM ตรวจสอบ arguments

if "%1"=="" (

REM Parse arguments    call :show_usage

set MANIFEST_PATH=    pause

set SCREEN_ID=    exit /b 0

set OUTPUT_PATH=)

set VERBOSE=

set CODE_ONLY=set COMMAND=%1

set TARGET=%2

:parse_argsset UC=%3

if "%~1"=="" goto run_analyzerset OUTPUT=%4

if "%~1"=="--manifest" (

    set MANIFEST_PATH=%~2REM กำหนด manifest path (default UC-01)

    shiftif "%UC%"=="" (

    shift    set UC=UC-01

    goto parse_args)

)set MANIFEST=wireframes\%UC%\wireframes-manifest.yml

if "%~1"=="--screen" (

    set SCREEN_ID=%~2REM ตรวจสอบว่ามี manifest หรือไม่

    shiftif not exist "%MANIFEST%" (

    shift    echo ❌ ไม่พบ manifest: %MANIFEST%

    goto parse_args    echo 💡 ลองใช้: analyze_wireframe.bat %COMMAND% %TARGET% UC-02

)    pause

if "%~1"=="--output" (    exit /b 1

    set OUTPUT_PATH=%~2)

    shift

    shiftREM รันคำสั่งตามประเภท

    goto parse_argsif "%COMMAND%"=="screen" (

)    if "%TARGET%"=="" (

if "%~1"=="--verbose" (        echo ❌ กรุณาระบุ Screen ID เช่น SC-01

    set VERBOSE=--verbose        pause

    shift        exit /b 1

    goto parse_args    )

)    

if "%~1"=="--code-only" (    echo 🔍 วิเคราะห์หน้าจอ: %TARGET% จาก %UC%

    set CODE_ONLY=--code-only    

    shift    if not "%OUTPUT%"=="" (

    goto parse_args        python wireframes\tools\wireframe_analyzer.py --manifest "%MANIFEST%" --screen "%TARGET%" --output "%OUTPUT%"

)        echo 💾 บันทึกผลลัพธ์: %OUTPUT%

if "%~1"=="-v" (    ) else (

    set VERBOSE=--verbose        python wireframes\tools\wireframe_analyzer.py --manifest "%MANIFEST%" --screen "%TARGET%"

    shift    )

    goto parse_args) else if "%COMMAND%"=="widget" (

)    if "%TARGET%"=="" (

shift        echo ❌ กรุณาระบุ Widget ID เช่น WG-01

goto parse_args        pause

        exit /b 1

:run_analyzer    )

if "%MANIFEST_PATH%"=="" (    

    echo Usage: analyze_wireframe.bat --manifest ^<path^> --screen ^<id^> [options]    echo 🧩 วิเคราะห์ widget: %TARGET% โดยอ้างจาก %UC%

    echo.    

    echo Options:    if not "%OUTPUT%"=="" (

    echo   --manifest ^<path^>   Manifest YAML file path        python wireframes\tools\wireframe_analyzer.py --manifest "%MANIFEST%" --widget "%TARGET%" --output "%OUTPUT%"

    echo   --screen ^<id^>       Screen ID to analyze        echo 💾 บันทึกผลลัพธ์: %OUTPUT%

    echo   --output ^<path^>     Output JSON file    ) else (

    echo   --code-only          Output only Flutter code        python wireframes\tools\wireframe_analyzer.py --manifest "%MANIFEST%" --widget "%TARGET%"

    echo   --verbose, -v        Verbose output    )

    echo.) else if "%COMMAND%"=="flow" (

    echo Examples:    echo 🌊 วิเคราะห์ทั้ง flow %UC%

    echo   analyze_wireframe.bat --manifest "../UC-01.1/wireframes-manifest.yml" --screen SC-09    

    echo   analyze_wireframe.bat --manifest "../UC-01.1/wireframes-manifest.yml" --screen SC-10 --output sc10_result.json    if not "%TARGET%"=="" (

    echo   analyze_wireframe.bat --manifest "../UC-01.1/wireframes-manifest.yml" --screen SC-09 --code-only        python wireframes\tools\wireframe_analyzer.py --manifest "%MANIFEST%" --flow --output "%TARGET%"

    exit /b 1        echo 💾 บันทึกผลลัพธ์: %TARGET%

)    ) else (

        python wireframes\tools\wireframe_analyzer.py --manifest "%MANIFEST%" --flow

if "%SCREEN_ID%"=="" (    )

    echo ❌ Screen ID required) else if "%COMMAND%"=="help" (

    exit /b 1    call :show_usage

)) else (

    echo ❌ คำสั่งไม่ถูกต้อง: %COMMAND%

REM Run analyzer    call :show_usage

set CMD_LINE=python "%TOOLS_DIR%wireframe_to_flutter.py" --manifest "%MANIFEST_PATH%" --screen "%SCREEN_ID%"    pause

    exit /b 1

if not "%OUTPUT_PATH%"=="" ()

    set CMD_LINE=!CMD_LINE! --output "%OUTPUT_PATH%"

)pause

if not "%VERBOSE%"=="" (exit /b 0

    set CMD_LINE=!CMD_LINE! %VERBOSE%

):show_usage

if not "%CODE_ONLY%"=="" (echo 🔍 Wireframe Analyzer Usage:

    set CMD_LINE=!CMD_LINE! %CODE_ONLY%echo.

)echo 📱 วิเคราะห์หน้าจอ:

echo   analyze_wireframe.bat screen SC-01 [UC-XX] [output.json]

echo 🚀 Running: !CMD_LINE!echo   analyze_wireframe.bat screen SC-01 UC-01

!CMD_LINE!echo   analyze_wireframe.bat screen SC-15 UC-02

echo.

if errorlevel 1 (echo 🧩 วิเคราะห์ widget:

    echo ❌ Analysis failedecho   analyze_wireframe.bat widget WG-01 [UC-XX] [output.json] 

    exit /b 1echo   analyze_wireframe.bat widget WG-13 UC-01

) else (echo.

    echo ✅ Analysis completedecho 🌊 วิเคราะห์ทั้ง flow:

)echo   analyze_wireframe.bat flow [output.json] [UC-XX]
echo   analyze_wireframe.bat flow UC-01
echo   analyze_wireframe.bat flow uc01_analysis.json UC-01
echo.
echo 💡 หมายเหตุ: ถ้าไม่ระบุ UC จะใช้ UC-01 เป็น default
echo.
goto :eof