@echo off
setlocal
cd /d "%~dp0"

"%~dp0PamelloV7.Server.exe" %*
set "code=%ERRORLEVEL%"

echo.
if "%code%"=="0" (
    echo PamelloV7.Server exited normally ^(code 0^).
) else (
    echo PamelloV7.Server exited with code %code%.
    pause
)
