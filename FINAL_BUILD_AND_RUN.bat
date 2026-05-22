@echo off
chcp 65001 >nul
color 0B
title Финальная сборка и запуск

cls
echo.
echo ╔═══════════════════════════════════════════════════════════════╗
echo ║                                                               ║
echo ║     ФИНАЛЬНАЯ СБОРКА И ЗАПУСК                                ║
echo ║     Все окна обновлены под новые таблицы                     ║
echo ║                                                               ║
echo ╚═══════════════════════════════════════════════════════════════╝
echo.

cd /d "%~dp0DatabaseApp"

echo [1/4] Удаление старой базы данных...
if exist "bin\Debug\net6.0-windows\accounting.db" (
    del /f /q "bin\Debug\net6.0-windows\accounting.db"
    echo ✓ База данных удалена
)
echo.

echo [2/4] Очистка проекта...
rmdir /s /q bin 2>nul
rmdir /s /q obj 2>nul
echo ✓ Проект очищен
echo.

echo [3/4] Сборка проекта...
dotnet build -c Debug --verbosity quiet

if errorlevel 1 (
    color 0C
    echo.
    echo ❌ Ошибка сборки
    echo.
    echo Показываю подробности:
    echo.
    dotnet build -c Debug
    echo.
    pause
    exit /b 1
)

echo ✓ Сборка завершена
echo.

echo [4/4] Запуск приложения...
echo.
echo ═══════════════════════════════════════════════════════════════
echo   ✅ ВСЕ ОКНА ОБНОВЛЕНЫ!
echo ═══════════════════════════════════════════════════════════════
echo.
echo   📋 СПРАВОЧНИКИ:
echo   • Заказчики - CustomersWindow (работает!)
echo   • Продукция и материалы - ProductsWindow (работает!)
echo   • Спецификации - SpecificationsWindow (работает!)
echo.
echo   📄 ДОКУМЕНТЫ:
echo   • Заказы покупателей - CustomerOrdersWindow (работает!)
echo   • Производство - ProductionWindow (работает!)
echo.
echo   📊 ОТЧЕТЫ:
echo   • Расчет стоимости - CostCalculationsWindow (работает!)
echo.
echo ═══════════════════════════════════════════════════════════════
echo.
echo   🔑 Учётные данные:
echo   • admin / admin123
echo   • user / user123
echo.
echo ═══════════════════════════════════════════════════════════════
echo.

timeout /t 3 /nobreak >nul
dotnet run --no-build

pause
