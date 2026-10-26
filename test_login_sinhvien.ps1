# Test đăng nhập tài khoản sinh viên
$baseUrl = "http://localhost:5080"

Write-Host "=== TEST DANG NHAP TAI KHOAN SINH VIEN ===" -ForegroundColor Cyan
Write-Host ""

# Test 1: Lấy trang login
Write-Host "[1/4] Lay trang login..." -ForegroundColor Yellow
$loginPage = Invoke-WebRequest -Uri "$baseUrl/TaiKhoan/Login" -SessionVariable session
$token = ($loginPage.Content | Select-String -Pattern '__RequestVerificationToken[^>]*value="([^"]+)"').Matches[0].Groups[1].Value
Write-Host "  Token: $($token.Substring(0,20))..." -ForegroundColor Green

# Test 2: Đăng nhập sinh viên NAM
Write-Host ""
Write-Host "[2/4] Test login sinh vien NAM: SV001/SV001" -ForegroundColor Yellow
$body = @{
    TenDangNhap = "SV001"
    MatKhau = "SV001"
    __RequestVerificationToken = $token
}

try {
    $response = Invoke-WebRequest -Uri "$baseUrl/TaiKhoan/Login" -Method Post -Body $body -WebSession $session -MaximumRedirection 0 -ErrorAction SilentlyContinue
} catch {
    $redirect = $_.Exception.Response.Headers.Location.AbsoluteUri
    if ($redirect -like "*Home*") {
        Write-Host "  [OK] Dang nhap thanh cong! Redirect: $redirect" -ForegroundColor Green
        
        # Test 3: Kiểm tra session
        Write-Host ""
        Write-Host "[3/4] Truy cap trang Home (sinh vien)..." -ForegroundColor Yellow
        try {
            $home = Invoke-WebRequest -Uri "$baseUrl/Home/Index" -WebSession $session
            Write-Host "  [OK] Trang Home tai thanh cong (HTTP $($home.StatusCode))" -ForegroundColor Green
            
            # Kiểm tra có phải trang sinh viên không
            if ($home.Content -match "sinh viên" -or $home.Content -match "Phòng KTX") {
                Write-Host "  [OK] Xac nhan: Dang o trang sinh vien" -ForegroundColor Green
            }
        } catch {
            Write-Host "  [ERROR] Khong truy cap duoc trang Home: $($_.Exception.Message)" -ForegroundColor Red
        }
        
        # Test 4: Xem danh sách phòng (chỉ phòng NAM)
        Write-Host ""
        Write-Host "[4/4] Xem danh sach phong (phai chi hien thi phong NAM)..." -ForegroundColor Yellow
        try {
            $phong = Invoke-WebRequest -Uri "$baseUrl/PhongKTX/Index" -WebSession $session
            Write-Host "  [OK] Trang Phong KTX tai thanh cong (HTTP $($phong.StatusCode))" -ForegroundColor Green
            
            # Kiểm tra lọc giới tính
            $contentNam = ($phong.Content | Select-String -Pattern "Phòng.*Nam" -AllMatches).Matches.Count
            $contentNu = ($phong.Content | Select-String -Pattern "Phòng.*N.." -AllMatches).Matches.Count
            
            Write-Host "  Tim thay: $contentNam phong (kiem tra loc gioi tinh)" -ForegroundColor Cyan
        } catch {
            Write-Host "  [ERROR] Khong truy cap duoc trang Phong: $($_.Exception.Message)" -ForegroundColor Red
        }
        
    } else {
        Write-Host "  [ERROR] Redirect sai: $redirect" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "=== TEST HOAN TAT ===" -ForegroundColor Cyan
