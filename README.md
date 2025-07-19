# FIAS API (Полная версия)

API для загрузки и распаковки данных ФИАС по региону.

## 🚀 Запуск через Docker
```bash
docker compose up --build -d
```

API будет доступно на:
```
http://localhost:5000/swagger
```

## 📌 Эндпоинты
- **GET** `/fias/download?region=77` — скачать архив с ФИАС и извлечь файлы по региону
- **GET** `/fias/process?path=/app/storage/file.zip&region=77` — обработать локальный архив
