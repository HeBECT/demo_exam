@echo off
chcp 65001 >nul
color 0B
title Система бухгалтерского учёта - Запуск

cls
echo.
echo ╔═══════════════════════════════════════════════════════════════╗
echo ║                                                               ║
echo ║     🎓 СИСТЕМА БУХГАЛТЕРСКОГО УЧЁТА                          ║
echo ║                                                               ║
echo ║     Готовая база данных для демонстрационного экзамена       ║
echo ║     БОМ 26 - Бухгалтерский учёт                              ║
echo ║                                                               ║
echo ╚═══════════════════════════════════════════════════════════════╝
echo.
echo   📋 Учётные данные для входа:
echo   ┌─────────────────────────────────────────────────────────────┐
echo   │  👤 Администратор:  admin / admin123                        │
echo   │  👤 Пользователь:   user  / user123                         │
echo   └─────────────────────────────────────────────────────────────┘
echo.
echo ═══════════════════════════════════════════════════════════════
echo.

REM Переход в папку проекта
cd /d "%~dp0DatabaseApp"

REM Проверка существования проекта
if not exist "DatabaseApp.csproj" (
    color 0C
    echo ❌ ОШИБКА: Файл проекта не найден!
    echo Ожидаемый путь: %~dp0DatabaseApp\DatabaseApp.csproj
    echo.
    pause
    exit /b 1
)

REM Проверка .NET SDK
echo ⏳ [1/3] Проверка .NET SDK...
dotnet --version >nul 2>&1
if errorlevel 1 (
    color 0C
    echo ❌ .NET SDK не установлен!
    echo Скачайте с: https://dotnet.microsoft.com/download/dotnet/6.0
    echo.
    pause
    exit /b 1
)
echo    ✓ .NET SDK обнаружен
echo.

echo ⏳ [2/3] Подготовка к запуску...
echo    ✓ Проект готов
echo.

echo 🚀 [3/3] Запуск приложения...
echo.
echo ═══════════════════════════════════════════════════════════════
echo   ✅ ПРИЛОЖЕНИЕ ЗАПУСКАЕТСЯ...
echo ═══════════════════════════════════════════════════════════════
echo.
echo   💾 База данных: accounting.db
echo   📂 Расположение: bin\Debug\net6.0-windows\
echo.
echo   ℹ База данных создаётся автоматически при первом запуске
echo.
echo   📊 Таблицы в базе данных:
echo   • customers - Заказчики (6 записей)
echo   • products - Продукция и материалы (8 записей)
echo   • customer_orders - Заказы покупателей
echo   • order_items - Позиции заказов
echo   • production - Производство
echo   • production_materials - Материалы производства
echo   • specifications - Спецификации
echo   • specification_materials - Материалы спецификаций
echo   • cost_calculations - Расчет стоимости
echo.
echo ═══════════════════════════════════════════════════════════════
echo.

timeout /t 2 /nobreak >nul

REM Запуск через dotnet run (не требует Runtime)
dotnet run

if errorlevel 1 (
    echo.
    echo ❌ Ошибка при запуске приложения
    pause
)
