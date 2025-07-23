@echo off
echo [INFO] Запуск на порту 8080...
docker-compose build
docker-compose up -d
start http://localhost:8080/swagger/index.html
pause