# Script test ứng dụng
$baseUrl = "http://localhost:5080"

Write-Host "=== TEST 1: Truy cập trang chủ ===" -ForegroundColor Green
try {
    $response = Invoke-WebRequest -Uri $baseUrl -MaximumRedirection 0 -ErrorAction SilentlyContinue
    Write-Host "Status: $($response.StatusCode) - Redirect to: $($response.Headers.Location)"
} catch {
    Write-Host "Redirect to: $($_.Exception.Response.Headers.Location)"
}

Write-Host "`n=== TEST 2: Trang đăng nhập ===" -ForegroundColor Green
$loginPage = Invoke-WebRequest -Uri "$baseUrl/TaiKhoan/Login" -SessionVariable session
Write-Host "Status: $($loginPage.StatusCode)"
Write-Host "Content length: $($loginPage.Content.Length) bytes"

Write-Host "`n=== TEST 3: Đăng nhập Admin ===" -ForegroundColor Green
$loginData = @{
    TenDangNhap = "admin"
    MatKhau = "admin123"
}

# Get antiforgery token
$token = ($loginPage.Content | Select-String -Pattern '__RequestVerificationToken[^>]*value="([^"]+)"').Matches[0].Groups[1].Value

$body = @{
    TenDangNhap = "admin"
    MatKhau = "admin123"
    __RequestVerificationToken = $token
}

try {
    $loginResponse = Invoke-WebRequest -Uri "$baseUrl/TaiKhoan/Login" -Method Post -Body $body -WebSession $session -MaximumRedirection 0 -ErrorAction SilentlyContinue
} catch {
    Write-Host "Login redirect to: $($_.Exception.Response.Headers.Location)"
}

Write-Host "`n=== TEST 4: Truy cập Dashboard (cần đăng nhập) ===" -ForegroundColor Green
try {
    $dashboard = Invoke-WebRequest -Uri "$baseUrl/ThongKe/Index" -WebSession $session
    Write-Host "Status: $($dashboard.StatusCode)"
    Write-Host "Dashboard loaded successfully!"
} catch {
    Write-Host "Error: $($_.Exception.Message)"
}

Write-Host "`n=== TEST 5: Kiểm tra dữ liệu ===" -ForegroundColor Green
Write-Host "Checking database..."
