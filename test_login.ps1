$baseUrl = "http://localhost:5080"

Write-Host "TEST DANG NHAP SINH VIEN SV001/SV001" -ForegroundColor Cyan

# Lấy token
$loginPage = Invoke-WebRequest -Uri "$baseUrl/TaiKhoan/Login" -SessionVariable session
$token = ($loginPage.Content | Select-String -Pattern '__RequestVerificationToken[^>]*value="([^"]+)"').Matches[0].Groups[1].Value

# Đăng nhập
$body = @{
    TenDangNhap = "SV001"
    MatKhau = "SV001"
    __RequestVerificationToken = $token
}

try {
    $response = Invoke-WebRequest -Uri "$baseUrl/TaiKhoan/Login" -Method Post -Body $body -WebSession $session -MaximumRedirection 0 -ErrorAction SilentlyContinue
    $redirect = $response.Headers.Location.AbsoluteUri
    if ($redirect -like "*Home*") {
        Write-Host "✅ Dang nhap thanh cong! Redirect: $redirect" -ForegroundColor Green
    }
} catch {
    Write-Host "❌ Loi: $($_.Exception.Message)" -ForegroundColor Red
}
