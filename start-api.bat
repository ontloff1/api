@echo off
echo ================================
echo   Запуск FIAS API в Docker
echo ================================

cd /d %~dp0

echo Старт docker compose...
docker compose up --build -d

if %errorlevel% neq 0 (
    echo Ошибка при запуске docker compose.
    pause
    exit /b
)

echo Ожидание запуска контейнера...
timeout /t 10 >nul

echo Открытие Swagger...
start http://localhost:5000/swagger/index.html

echo ================================
echo   FIAS API успешно запущено!
echo ================================
pause