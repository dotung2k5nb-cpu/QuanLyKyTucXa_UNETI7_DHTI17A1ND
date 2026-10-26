@echo off
echo ========================================
echo XOA VA TAO LAI DATABASE
echo ========================================
echo.

echo [1/3] Dung ung dung...
taskkill /F /IM QuanLyKyTucXa_UNETI7_TI17A1ND.exe 2>nul
timeout /t 2 >nul

echo [2/3] Xoa database cu...
dotnet ef database drop --force

echo [3/3] Tao database moi...
dotnet ef database update

echo.
echo ========================================
echo HOAN THANH!
echo ========================================
echo.
echo Gio ban co the:
echo 1. Chay ung dung: dotnet run
echo 2. Truy cap: http://localhost:5080
echo 3. Login: admin / admin123
echo.
pause
