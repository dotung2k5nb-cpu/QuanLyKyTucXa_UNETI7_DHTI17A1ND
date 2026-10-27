# HƯỚNG DẪN CHẠY DỰ ÁN — QUẢN LÝ KÝ TÚC XÁ

**Dự án:** QuanLyKyTucXa_UNETI7_DHTI17A1ND
**Công nghệ:** ASP.NET Core 10 MVC + C# + Entity Framework Core 10 + SQL Server (LocalDB)
**URL:** http://localhost:5080

---

## 1. YÊU CẦU HỆ THỐNG

1. **.NET 10 SDK** — kiểm tra: `dotnet --version` (phải trả về `10.0.x`)
2. **SQL Server LocalDB** (dành cho dev) — thường có sẵn trong Visual Studio; kiểm tra:
   ```bash
   sqllocaldb info
   # Kết quả mong đợi: MSSQLLocalDB
   ```
   > Nếu không có LocalDB, cài đặt qua *Visual Studio Installer → Individual components → SQL Server Express LocalDB*
   > hoặc dùng SQL Server Express/Developer Edition và đổi connection string (mục 4).
3. **Visual Studio 2022** hoặc **Visual Studio Code** (cần .NET SDK + MSBuild)

`appsettings.json` đã được cấu hình sẵn dùng **SQL Server LocalDB**:
```json
"DatabaseProvider": "SqlServer",
"ConnectionStrings": {
  "KTX": "Server=(localdb)\\MSSQLLocalDB;Database=QuanLyKTX_UNETI7;Trusted_Connection=True;TrustServerCertificate=True"
}
```

---

## 2. CÁCH CHẠY NHANH

```bash
# Windows
dotnet run

# Hoặc dùng script có sẵn (Linux/macOS/WSL, tự ngắt cổng 5080 cũ)
./run.sh
```

Lần chạy đầu tiên, ứng dụng tự động:
1. Tạo database `QuanLyKTX_UNETI7` trong LocalDB
2. Áp dụng toàn bộ migration (Code First)
3. **Nạp dữ liệu mẫu** (chỉ khi database trống — xem mục 5)

Mở trình duyệt: **http://localhost:5080**

---

## 3. TÀI KHOẢN MẪU

### Admin
| Username | Password |
|----------|----------|
| `admin` | `admin123` |

### Sinh viên (30 tài khoản, SV001 → SV030)
**Quy tắc: Username = Password = Mã sinh viên**, ví dụ `SV001` / `SV001`.

- Mã lẻ → Nam (SV001, SV003, ...), mã chẵn → Nữ (SV002, SV004, ...)
- Khuyến nghị test: `SV001` (Nam, đang ở phòng P01) và `SV010` (Nữ, đăng ký đang chờ duyệt)

---

## 4. DỮ LIỆU MẪU (do Seeder tự nạp)

| Đối tượng | Số lượng | Chi tiết |
|-----------|----------|----------|
| Loại phòng | 4 | 2 người, 4 người, 6 người, 8 người |
| Phòng KTX | 20 | P01–P10: tòa A, Nam; P11–P20: tòa B, Nữ; P10 & P20: *Tạm ngừng* |
| Sinh viên | 30 | SV001–SV030, lớp DHTI17A1ND, khoa CNTT |
| Đăng ký KTX | 30 | 17 *Chờ duyệt*, 2 *Đã hủy*, 8 *Đã phân phòng* (đang ở), 1 *Đã trả phòng*, 1 *Đang ở + lịch sử chuyển phòng* |
| Phân phòng | ~10 | Phòng P01 đầy (2/2), P02 đầy (2/2), P05, P13, P14... còn chỗ |

**Trang "Đăng ký KTX chờ xử lý" (Admin) sẽ có sẵn 17 bản ghi ở trạng thái *Chờ duyệt*** để test duyệt/từ chối.

---

## 5. TẠO LẠI DỮ LIỆU MẪU

Trong `appsettings.json`:
```json
"DuLieuMau": {
  "TaoLaiKhiKhoiDong": true
}
```
- `true`: mỗi lần khởi động sẽ **xóa toàn bộ dữ liệu và nạp lại bộ mẫu** (đang bật sẵn).
- `false`: giữ nguyên dữ liệu (chuyển về `false` sau khi đã nạp xong, nếu không muốn mất dữ liệu nhập thật).

Sau đó chạy lại `dotnet run`.

> ⚠️ Thao tác này **xóa mọi thay đổi đã nhập** (tài khoản, sinh viên, đăng ký, phân phòng).

### Cách thay thế: dùng script SQL
- `reset_database.bat` (Windows): dừng app → xóa & tạo lại DB → chạy lại
- `ALL_DATABASE.sql`: toàn bộ script CSDL gộp lại (chạy trong **SSMS**, xem mục 6)

---

## 6. CHẠY BẰNG SSMS (SQL Server Management Studio)

File **`ALL_DATABASE.sql`** gộp toàn bộ script CSDL của dự án (tạo database + schema + các query kiểm tra):

1. Mở SSMS → kết nối server `(localdb)\MSSQLLocalDB`
2. **New Query** (Ctrl+Q) → mở file `ALL_DATABASE.sql` → **F5** (Execute)
3. Script sẽ: xóa DB cũ (nếu có) → tạo `QuanLyKTX_UNETI7` + toàn bộ bảng → chạy các query kiểm tra

**Lưu ý quan trọng:**
- Script **XÓA database cũ trước khi tạo lại** — sao lưu trước nếu cần
- Script **chỉ tạo cấu trúc, không có dữ liệu mẫu**. Dữ liệu mẫu được nạp bởi Seeder của ứng dụng ở lần `dotnet run` đầu tiên (hoặc bật cờ `TaoLaiKhiKhoiDong = true`)
- Nếu bị lỗi `Cannot drop database ... in use`: đóng app và mọi query đang mở trên DB đó trước, hoặc chạy thủ công:
  ```sql
  USE [master];
  ALTER DATABASE [QuanLyKTX_UNETI7] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
  DROP DATABASE [QuanLyKTX_UNETI7];
  ```

**Thứ tự khuyến nghị:**
```
1. Chạy ALL_DATABASE.sql trong SSMS  → DB trống + đủ bảng
2. dotnet run                        → Seeder nạp 30 SV, 20 phòng, 30 đăng ký...
3. Chạy lại các query kiểm tra       → thấy dữ liệu đầy đủ
```

---

## 7. TÌM KIẾM LỖI

| Lỗi | Nguyên nhân / Cách xử lý |
|-----|--------------------------|
| `Cannot open database "master" requested by the login` | Chưa có LocalDB — cài SQL Server Express LocalDB |
| `Cannot drop database "QuanLyKTX_UNETI7" because it is currently in use` | App vẫn đang chạy hoặc có kết nối SSMS cũ — dừng app, đóng query, chạy lại |
| Trang 405 (Method Not Allowed) khi bấm Duyệt/Từ chối | Bản code cũ — cập nhật lại source (bản mới có form xác nhận cho cả GET/POST) |
| Không thấy dữ liệu mẫu | Bật `DuLieuMau:TaoLaiKhiKhoiDong = true` rồi `dotnet run` lại |
| Quên mật khẩu Admin | Bật `TaiKhoanMacDinh:DatLaiAdminKhiKhoiDong = true` rồi chạy lại (đặt lại `admin/admin123`) |

---

## 8. TÓM TẮT CẤU TRÚC MODULE CHÍNH

| Module | Người phụ trách chính | Chức năng |
|--------|----------------------|-----------|
| Đăng nhập & phân quyền | — | Session, Admin/Sinh viên, Data Annotation + Validation |
| Quản lý sinh viên, đăng ký KTX | Phạm Quang Nhật (23203100085) | CRUD sinh viên, đăng ký ở, **duyệt/từ chối**, lịch sử |
| Phân phòng, chuyển phòng, trả phòng, Dashboard | Nguyễn Anh Tú (23203100027) | Assign/Chuyen/Tra phòng, thống kê, deadlock retry |
| Loại phòng, phòng KTX | — | CRUD loại phòng, phòng, lọc/tìm kiếm/phân trang |
| Dữ liệu mẫu, hằng số trạng thái | Đỗ Văn Tùng (23203100028), Hoàng Tiến Nam (23203100029) | Seeder, class `TT` |
