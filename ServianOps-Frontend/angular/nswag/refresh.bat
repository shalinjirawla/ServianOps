@echo off
echo Generating API Clients with NSwag...
cd %~dp0
call npx nswag run service.config.nswag
if %errorlevel% neq 0 (
    echo [ERROR] NSwag generation failed. Make sure the backend is running.
    pause
    exit /b %errorlevel%
)
echo [SUCCESS] API Clients generated successfully at src/app/core/api/service-proxies.ts!
