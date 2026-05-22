@echo off
chcp 65001 >nul
echo ═══════════════════════════════════════════════════════════════
echo   Система бухгалтерского учета - Запуск
echo ═══════════════════════════════════════════════════════════════
echo.
echo Проверка .NET SDK...
dotnet --version >nul 2>&1
if errorlevel 1 (
    echo ❌ .NET SDK не найден!
    echo.
    echo Установите .NET 6.0 SDK с:
    echo https://dotnet.microsoft.com/download
    echo.
    pause
    exit /b 1
)

echo ✓ .NET SDK найден
echo.
echo Восстановление пакетов...
dotnet restore

echo.
echo Сборка приложения...
dotnet build

echo.
echo Запуск приложения...
echo.
echo Учетные данные:
echo   Администратор: admin / admin123
echo   Пользователь: user / user123
echo.
echo ═══════════════════════════════════════════════════════════════
echo.

dotnet run

if errorlevel 1 (
    echo.
    echo ❌ Ошибка при запуске приложения
    pause
)
