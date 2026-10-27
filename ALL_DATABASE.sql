/* ============================================================================
   TOAN BO SCRIPT CO SO DU LIEU - QUAN LY KY TUC XA - UNETI7 - DHTI17A1ND
   (Gop tu 7 file .sql trong du an)

   Thu muc:
   1. database.sql        - Tao lai CSDL + schema (tu migration EF Core)
   2. verify_data.sql     - Kiem tra du lieu mau
   3. check_db.sql        - Kiem tra database + tai khoan
   4. check_accounts.sql  - Kiem tra tai khoan Admin/Sinh vien
   5. verify_accounts.sql - Xac thuc tai khoan
   6. get_accounts.sql    - Lay danh sach tai khoan sinh vien
   7. query_accounts.sql  - Query tai khoan sinh vien + thong ke
   ============================================================================ */

/* ------------------------------------------------------------------------------
   NGUON: database.sql
   ------------------------------------------------------------------------------ */

/* ============================================================================
   QUAN LY KY TUC XA - UNETI7 - DHTI17A1ND
   Script tao lai CSDL QuanLyKTX_UNETI7 tu dau (schema cuoi cung cua EF Core)

   - Tuong duong 6 migration, tu 20261004040702 den 20261022030102.
   - AN TOAN KHI CHAY LAI: script se XOA database QuanLyKTX_UNETI7 neu da ton tai
     roi tao moi, nen khong con loi "There is already an object named ...".
   - Script chi tao cau truc bang + bang __EFMigrationsHistory. Du lieu mau
     (admin/admin123, phong, sinh vien...) do ung dung tu nap (Seeder) o lan chay
     dau tien bang "dotnet run".
   - KHONG chay script nay tren database ma ung dung da tung Migrate().
   ============================================================================ */

USE [master];
GO

IF DB_ID(N'QuanLyKTX_UNETI7') IS NOT NULL
BEGIN
    ALTER DATABASE [QuanLyKTX_UNETI7] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [QuanLyKTX_UNETI7];
END;
GO

CREATE DATABASE [QuanLyKTX_UNETI7];
GO

USE [QuanLyKTX_UNETI7];
GO

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET XACT_ABORT ON;
GO

BEGIN TRANSACTION;

CREATE TABLE [__EFMigrationsHistory] (
    [MigrationId] nvarchar(150) NOT NULL,
    [ProductVersion] nvarchar(32) NOT NULL,
    CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
);

CREATE TABLE [LoaiPhongs] (
    [MaLoaiPhong] int NOT NULL IDENTITY,
    [TenLoaiPhong] nvarchar(100) NOT NULL,
    [SoNguoiToiDa] int NOT NULL,
    [DonGiaThang] decimal(18,0) NOT NULL,
    [MoTa] nvarchar(500) NULL,
    [TrangThai] nvarchar(20) NOT NULL,
    CONSTRAINT [PK_LoaiPhongs] PRIMARY KEY ([MaLoaiPhong])
);

CREATE TABLE [TaiKhoans] (
    [MaTaiKhoan] int NOT NULL IDENTITY,
    [TenDangNhap] nvarchar(50) NOT NULL,
    [MatKhau] nvarchar(100) NOT NULL,
    [HoTen] nvarchar(100) NOT NULL,
    [Email] nvarchar(100) NULL,
    [VaiTro] nvarchar(20) NOT NULL,
    [TrangThai] nvarchar(20) NOT NULL,
    CONSTRAINT [PK_TaiKhoans] PRIMARY KEY ([MaTaiKhoan])
);

CREATE TABLE [Phongs] (
    [MaPhong] int NOT NULL IDENTITY,
    [TenPhong] nvarchar(50) NOT NULL,
    [MaLoaiPhong] int NOT NULL,
    [ToaNha] nvarchar(50) NOT NULL,
    [Tang] int NOT NULL,
    [GioiTinhPhong] nvarchar(10) NOT NULL,
    [TrangThai] nvarchar(20) NOT NULL,
    [MoTa] nvarchar(500) NULL,
    CONSTRAINT [PK_Phongs] PRIMARY KEY ([MaPhong]),
    CONSTRAINT [FK_Phongs_LoaiPhongs_MaLoaiPhong] FOREIGN KEY ([MaLoaiPhong]) REFERENCES [LoaiPhongs] ([MaLoaiPhong]) ON DELETE NO ACTION
);

CREATE TABLE [SinhViens] (
    [MaSinhVien] nvarchar(20) NOT NULL,
    [MaTaiKhoan] int NOT NULL,
    [HoTen] nvarchar(100) NOT NULL,
    [NgaySinh] datetime2 NOT NULL,
    [GioiTinh] nvarchar(10) NOT NULL,
    [Lop] nvarchar(50) NULL,
    [Khoa] nvarchar(100) NULL,
    [SoDienThoai] nvarchar(15) NULL,
    [Email] nvarchar(100) NULL,
    [DiaChi] nvarchar(200) NULL,
    [TrangThai] nvarchar(20) NOT NULL,
    CONSTRAINT [PK_SinhViens] PRIMARY KEY ([MaSinhVien]),
    CONSTRAINT [FK_SinhViens_TaiKhoans_MaTaiKhoan] FOREIGN KEY ([MaTaiKhoan]) REFERENCES [TaiKhoans] ([MaTaiKhoan]) ON DELETE NO ACTION
);

CREATE TABLE [DangKys] (
    [MaDangKy] int NOT NULL IDENTITY,
    [MaSinhVien] nvarchar(20) NOT NULL,
    [NgayDangKy] datetime2 NOT NULL,
    [NgayBatDau] datetime2 NOT NULL,
    [NgayKetThucDuKien] datetime2 NOT NULL,
    [NguyenVongLoaiPhong] int NOT NULL,
    [GhiChu] nvarchar(500) NULL,
    [TrangThai] nvarchar(20) NOT NULL,
    CONSTRAINT [PK_DangKys] PRIMARY KEY ([MaDangKy]),
    CONSTRAINT [FK_DangKys_LoaiPhongs_NguyenVongLoaiPhong] FOREIGN KEY ([NguyenVongLoaiPhong]) REFERENCES [LoaiPhongs] ([MaLoaiPhong]) ON DELETE NO ACTION,
    CONSTRAINT [FK_DangKys_SinhViens_MaSinhVien] FOREIGN KEY ([MaSinhVien]) REFERENCES [SinhViens] ([MaSinhVien]) ON DELETE NO ACTION
);

CREATE TABLE [PhanPhongs] (
    [MaPhanPhong] int NOT NULL IDENTITY,
    [MaDangKy] int NOT NULL,
    [MaPhong] int NOT NULL,
    [NgayVao] datetime2 NOT NULL,
    [NgayRa] datetime2 NULL,
    [TrangThai] nvarchar(20) NOT NULL,
    CONSTRAINT [PK_PhanPhongs] PRIMARY KEY ([MaPhanPhong]),
    CONSTRAINT [FK_PhanPhongs_DangKys_MaDangKy] FOREIGN KEY ([MaDangKy]) REFERENCES [DangKys] ([MaDangKy]) ON DELETE NO ACTION,
    CONSTRAINT [FK_PhanPhongs_Phongs_MaPhong] FOREIGN KEY ([MaPhong]) REFERENCES [Phongs] ([MaPhong]) ON DELETE NO ACTION
);

CREATE INDEX [IX_DangKys_NguyenVongLoaiPhong] ON [DangKys] ([NguyenVongLoaiPhong]);
CREATE UNIQUE INDEX [UX_DangKys_MotChoDuyetMoiSV] ON [DangKys] ([MaSinhVien]) WHERE [TrangThai] = N'Chờ duyệt';
CREATE UNIQUE INDEX [UX_DangKys_MotDaPhanPhongMoiSV] ON [DangKys] ([MaSinhVien]) WHERE [TrangThai] = N'Đã phân phòng';

CREATE UNIQUE INDEX [IX_LoaiPhongs_TenLoaiPhong] ON [LoaiPhongs] ([TenLoaiPhong]);

CREATE INDEX [IX_PhanPhongs_MaPhong] ON [PhanPhongs] ([MaPhong]);
CREATE UNIQUE INDEX [UX_PhanPhongs_MotDangOMoiDangKy] ON [PhanPhongs] ([MaDangKy]) WHERE [TrangThai] = N'Đang ở';

CREATE INDEX [IX_Phongs_MaLoaiPhong] ON [Phongs] ([MaLoaiPhong]);
CREATE UNIQUE INDEX [IX_Phongs_TenPhong] ON [Phongs] ([TenPhong]);

CREATE UNIQUE INDEX [IX_SinhViens_MaTaiKhoan] ON [SinhViens] ([MaTaiKhoan]);

CREATE UNIQUE INDEX [IX_TaiKhoans_TenDangNhap] ON [TaiKhoans] ([TenDangNhap]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES
    (N'20261004040702_KhoiTaoCoSoDuLieu', N'10.0.0'),
    (N'20261004120000_RangBuocCotVaThongNhatTrangThai', N'10.0.0'),
    (N'20261004130000_RangBuocDuyNhatChoDuyetVaDangO', N'10.0.0'),
    (N'20261006120000_GhiNhanChiMucVaoModel', N'10.0.0'),
    (N'20261007111345_DongBoChiMucTheoModel', N'10.0.0'),
    (N'20261022030102_RemoveUnusedValidationOrSync', N'10.0.0');

COMMIT;
GO

SELECT [MigrationId] FROM [__EFMigrationsHistory] ORDER BY [MigrationId];
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES ORDER BY TABLE_NAME;
GO

/* ------------------------------------------------------------------------------
   NGUON: verify_data.sql
   ------------------------------------------------------------------------------ */

-- Script kiểm tra dữ liệu mẫu trong database
-- Chạy script này trong SQL Server để verify dữ liệu

USE QuanLyKTX_UNETI7;
GO

PRINT '=== 1. KIỂM TRA TÀI KHOẢN ADMIN ===';
SELECT COUNT(*) AS 'Số tài khoản Admin', 
       STRING_AGG(TenDangNhap, ', ') AS 'Username'
FROM TaiKhoans 
WHERE VaiTro = 'Admin';

PRINT CHAR(13) + '=== 2. KIỂM TRA LOẠI PHÒNG (Cần >= 3) ===';
SELECT MaLoaiPhong, TenLoaiPhong, SoNguoiToiDa, DonGiaThang, TrangThai
FROM LoaiPhongs
ORDER BY SoNguoiToiDa;

PRINT CHAR(13) + '=== 3. KIỂM TRA PHÒNG KTX (Cần >= 10) ===';
SELECT 
    COUNT(*) AS 'Tổng số phòng',
    SUM(CASE WHEN TrangThai = N'Đang sử dụng' THEN 1 ELSE 0 END) AS 'Đang sử dụng',
    SUM(CASE WHEN TrangThai = N'Tạm ngừng' THEN 1 ELSE 0 END) AS 'Tạm ngừng',
    SUM(CASE WHEN GioiTinhPhong = N'Nam' THEN 1 ELSE 0 END) AS 'Phòng Nam',
    SUM(CASE WHEN GioiTinhPhong = N'Nữ' THEN 1 ELSE 0 END) AS 'Phòng Nữ'
FROM Phongs;

PRINT CHAR(13) + '=== 4. CHI TIẾT PHÒNG ===';
SELECT 
    p.TenPhong,
    p.ToaNha,
    p.Tang,
    lp.TenLoaiPhong,
    lp.SoNguoiToiDa AS 'Sức chứa',
    COUNT(pp.MaPhanPhong) AS 'Đang ở',
    lp.SoNguoiToiDa - COUNT(pp.MaPhanPhong) AS 'Còn chỗ',
    p.GioiTinhPhong AS 'Giới tính',
    p.TrangThai
FROM Phongs p
LEFT JOIN LoaiPhongs lp ON p.MaLoaiPhong = lp.MaLoaiPhong
LEFT JOIN PhanPhongs pp ON p.MaPhong = pp.MaPhong AND pp.TrangThai = N'Đang ở'
GROUP BY p.TenPhong, p.ToaNha, p.Tang, lp.TenLoaiPhong, lp.SoNguoiToiDa, p.GioiTinhPhong, p.TrangThai
ORDER BY p.ToaNha, p.Tang, p.TenPhong;

PRINT CHAR(13) + '=== 5. KIỂM TRA SINH VIÊN (Cần >= 20) ===';
SELECT 
    COUNT(*) AS 'Tổng sinh viên',
    SUM(CASE WHEN GioiTinh = N'Nam' THEN 1 ELSE 0 END) AS 'Nam',
    SUM(CASE WHEN GioiTinh = N'Nữ' THEN 1 ELSE 0 END) AS 'Nữ',
    SUM(CASE WHEN TrangThai = N'Hoạt động' THEN 1 ELSE 0 END) AS 'Hoạt động',
    SUM(CASE WHEN TrangThai = N'Khóa' THEN 1 ELSE 0 END) AS 'Khóa'
FROM SinhViens;

PRINT CHAR(13) + '=== 6. KIỂM TRA ĐĂNG KÝ (Cần >= 10) ===';
SELECT 
    COUNT(*) AS 'Tổng đăng ký',
    SUM(CASE WHEN TrangThai = N'Chờ duyệt' THEN 1 ELSE 0 END) AS 'Chờ duyệt',
    SUM(CASE WHEN TrangThai = N'Đã phân phòng' THEN 1 ELSE 0 END) AS 'Đã phân phòng',
    SUM(CASE WHEN TrangThai = N'Đã trả phòng' THEN 1 ELSE 0 END) AS 'Đã trả phòng',
    SUM(CASE WHEN TrangThai = N'Đã hủy' THEN 1 ELSE 0 END) AS 'Đã hủy'
FROM DangKys;

PRINT CHAR(13) + '=== 7. KIỂM TRA PHÂN PHÒNG (Cần >= 5) ===';
SELECT 
    COUNT(*) AS 'Tổng phân phòng',
    SUM(CASE WHEN TrangThai = N'Đang ở' THEN 1 ELSE 0 END) AS 'Đang ở',
    SUM(CASE WHEN TrangThai = N'Đã chuyển' THEN 1 ELSE 0 END) AS 'Đã chuyển'
FROM PhanPhongs;

PRINT CHAR(13) + '=== 8. CHI TIẾT SINH VIÊN ĐANG Ở ===';
SELECT 
    sv.MaSinhVien,
    sv.HoTen,
    sv.GioiTinh,
    sv.Lop,
    p.TenPhong,
    p.ToaNha,
    lp.TenLoaiPhong,
    pp.NgayVao,
    pp.TrangThai
FROM PhanPhongs pp
INNER JOIN DangKys dk ON pp.MaDangKy = dk.MaDangKy
INNER JOIN SinhViens sv ON dk.MaSinhVien = sv.MaSinhVien
INNER JOIN Phongs p ON pp.MaPhong = p.MaPhong
INNER JOIN LoaiPhongs lp ON p.MaLoaiPhong = lp.MaLoaiPhong
WHERE pp.TrangThai = N'Đang ở'
ORDER BY p.TenPhong, sv.MaSinhVien;

PRINT CHAR(13) + '=== 9. KIỂM TRA PHÒNG ĐẦY/CÒN CHỖ ===';
SELECT 
    'Phòng đầy' AS 'Loại',
    COUNT(*) AS 'Số lượng'
FROM (
    SELECT p.MaPhong
    FROM Phongs p
    LEFT JOIN LoaiPhongs lp ON p.MaLoaiPhong = lp.MaLoaiPhong
    LEFT JOIN PhanPhongs pp ON p.MaPhong = pp.MaPhong AND pp.TrangThai = N'Đang ở'
    WHERE p.TrangThai = N'Đang sử dụng'
    GROUP BY p.MaPhong, lp.SoNguoiToiDa
    HAVING COUNT(pp.MaPhanPhong) >= lp.SoNguoiToiDa
) AS PhongDay
UNION ALL
SELECT 
    'Phòng còn chỗ' AS 'Loại',
    COUNT(*) AS 'Số lượng'
FROM (
    SELECT p.MaPhong
    FROM Phongs p
    LEFT JOIN LoaiPhongs lp ON p.MaLoaiPhong = lp.MaLoaiPhong
    LEFT JOIN PhanPhongs pp ON p.MaPhong = pp.MaPhong AND pp.TrangThai = N'Đang ở'
    WHERE p.TrangThai = N'Đang sử dụng'
    GROUP BY p.MaPhong, lp.SoNguoiToiDa
    HAVING COUNT(pp.MaPhanPhong) < lp.SoNguoiToiDa
) AS PhongConCho;

PRINT CHAR(13) + '=== 10. TÀI KHOẢN SINH VIÊN MẪU (5 tài khoản đầu) ===';
SELECT TOP 5
    sv.MaSinhVien AS 'Username/Password mặc định',
    sv.HoTen,
    sv.GioiTinh,
    sv.Lop,
    tk.VaiTro,
    tk.TrangThai
FROM SinhViens sv
INNER JOIN TaiKhoans tk ON sv.MaTaiKhoan = tk.MaTaiKhoan
ORDER BY sv.MaSinhVien;

PRINT CHAR(13) + '=== TỔNG KẾT ===';
SELECT 
    'Tài khoản' AS 'Entity',
    CAST(COUNT(*) AS VARCHAR) AS 'Số lượng'
FROM TaiKhoans
UNION ALL
SELECT 'Loại phòng', CAST(COUNT(*) AS VARCHAR) FROM LoaiPhongs
UNION ALL
SELECT 'Phòng KTX', CAST(COUNT(*) AS VARCHAR) FROM Phongs
UNION ALL
SELECT 'Sinh viên', CAST(COUNT(*) AS VARCHAR) FROM SinhViens
UNION ALL
SELECT 'Đăng ký', CAST(COUNT(*) AS VARCHAR) FROM DangKys
UNION ALL
SELECT 'Phân phòng', CAST(COUNT(*) AS VARCHAR) FROM PhanPhongs;

GO

/* ------------------------------------------------------------------------------
   NGUON: check_db.sql
   ------------------------------------------------------------------------------ */

USE QuanLyKTX_UNETI7;

-- Kiểm tra có database không
SELECT 'Database exists' AS Status;

-- Đếm số lượng tài khoản
SELECT 'TaiKhoans' AS Table_Name, COUNT(*) AS Count FROM TaiKhoans;

-- Kiểm tra Admin
SELECT 'Admin Account' AS Type, TenDangNhap, VaiTro, TrangThai 
FROM TaiKhoans 
WHERE VaiTro = 'Admin';

-- Kiểm tra sinh viên
SELECT 'SinhVien Accounts' AS Type, COUNT(*) AS Count 
FROM TaiKhoans 
WHERE VaiTro = N'Sinh viên';

-- Xem 5 tài khoản đầu tiên
SELECT TOP 5 TenDangNhap, VaiTro, TrangThai 
FROM TaiKhoans 
ORDER BY MaTaiKhoan;

-- Kiểm tra SV001 có tồn tại không
SELECT 'Check SV001' AS Type, 
       CASE WHEN EXISTS(SELECT 1 FROM TaiKhoans WHERE TenDangNhap = 'SV001') 
            THEN 'EXISTS' 
            ELSE 'NOT EXISTS' 
       END AS Status;

/* ------------------------------------------------------------------------------
   NGUON: check_accounts.sql
   ------------------------------------------------------------------------------ */

USE QuanLyKTX_UNETI7;
GO

-- Kiểm tra tài khoản Admin
SELECT 'ADMIN' AS 'Loai', TenDangNhap, HoTen, VaiTro, TrangThai FROM TaiKhoans WHERE VaiTro = 'Admin';

-- Kiểm tra tài khoản Sinh viên
SELECT 'SINH VIEN' AS 'Loai', COUNT(*) AS 'So luong' FROM TaiKhoans WHERE VaiTro = N'Sinh viên';

-- Xem 5 tài khoản sinh viên đầu tiên
SELECT TOP 5
    tk.TenDangNhap AS 'Username',
    sv.HoTen,
    sv.GioiTinh,
    tk.VaiTro,
    tk.TrangThai
FROM TaiKhoans tk
LEFT JOIN SinhViens sv ON tk.MaTaiKhoan = sv.MaTaiKhoan
WHERE tk.VaiTro = N'Sinh viên'
ORDER BY tk.TenDangNhap;

-- Kiểm tra tổng số bản ghi
SELECT 
    'TaiKhoans' AS 'Bang',
    COUNT(*) AS 'So luong'
FROM TaiKhoans
UNION ALL
SELECT 'SinhViens', COUNT(*) FROM SinhViens
UNION ALL
SELECT 'LoaiPhongs', COUNT(*) FROM LoaiPhongs
UNION ALL
SELECT 'Phongs', COUNT(*) FROM Phongs;
GO

/* ------------------------------------------------------------------------------
   NGUON: verify_accounts.sql
   ------------------------------------------------------------------------------ */

USE QuanLyKTX_UNETI7;

-- Kiểm tra tổng số tài khoản
SELECT COUNT(*) AS 'Tong tai khoan' FROM TaiKhoans;

-- Kiểm tra Admin
SELECT 'Admin' AS 'Loai', TenDangNhap FROM TaiKhoans WHERE VaiTro = 'Admin';

-- Kiểm tra sinh viên
SELECT 'SinhVien' AS 'Loai', COUNT(*) AS 'So luong' FROM TaiKhoans WHERE VaiTro = N'Sinh viên';

-- Xem 10 tài khoản sinh viên đầu
SELECT TOP 10 TenDangNhap, VaiTro FROM TaiKhoans WHERE VaiTro = N'Sinh viên' ORDER BY TenDangNhap;

-- Kiểm tra SV001 và SV011
SELECT TenDangNhap, VaiTro FROM TaiKhoans WHERE TenDangNhap IN ('SV001', 'SV011');

/* ------------------------------------------------------------------------------
   NGUON: get_accounts.sql
   ------------------------------------------------------------------------------ */

-- Lấy danh sách tài khoản sinh viên
USE QuanLyKTX_UNETI7;
GO

SELECT TOP 10
    sv.MaSinhVien AS 'Username',
    sv.MaSinhVien AS 'Password (mặc định)',
    sv.HoTen AS 'Họ tên',
    sv.GioiTinh AS 'Giới tính',
    sv.Lop AS 'Lớp',
    tk.TrangThai AS 'Trạng thái'
FROM SinhViens sv
INNER JOIN TaiKhoans tk ON sv.MaTaiKhoan = tk.MaTaiKhoan
WHERE tk.VaiTro = N'Sinh viên'
ORDER BY sv.MaSinhVien;
GO

/* ------------------------------------------------------------------------------
   NGUON: query_accounts.sql
   ------------------------------------------------------------------------------ */

-- Query lấy danh sách tài khoản sinh viên
USE QuanLyKTX_UNETI7;
GO

PRINT '=== TAI KHOAN SINH VIEN ===';
PRINT '';

SELECT
    sv.MaSinhVien AS 'Username',
    sv.MaSinhVien AS 'Password',
    sv.HoTen AS 'Ho ten',
    sv.GioiTinh AS 'Gioi tinh',
    sv.Lop AS 'Lop',
    tk.TrangThai AS 'Trang thai'
FROM SinhViens sv
INNER JOIN TaiKhoans tk ON sv.MaTaiKhoan = tk.MaTaiKhoan
WHERE tk.VaiTro = N'Sinh viên'
ORDER BY sv.MaSinhVien;

PRINT '';
PRINT '=== THONG KE ===';
SELECT COUNT(*) AS 'Tong so sinh vien' FROM SinhViens;
GO
