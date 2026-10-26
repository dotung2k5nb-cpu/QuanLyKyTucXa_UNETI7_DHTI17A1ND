// Họ và tên: Đỗ Văn Tùng
// Mã sinh viên: 23203100028
// Nội dung thực hiện: Dữ liệu mẫu: tài khoản Admin, loại phòng
//
// Họ và tên: Hoàng Tiến Nam
// Mã sinh viên: 23203100029
// Nội dung thực hiện: Dữ liệu mẫu: phòng KTX (đầy, còn 1 chỗ, tạm ngừng)
//
// Họ và tên: Phạm Quang Nhật
// Mã sinh viên: 23203100085
// Nội dung thực hiện: Dữ liệu mẫu: sinh viên và tài khoản, đăng ký KTX
//
// Họ và tên: Nguyễn Anh Tú
// Mã sinh viên: 23203100027
// Nội dung thực hiện: Dữ liệu mẫu: phân phòng, chuyển phòng, trả phòng

using Microsoft.EntityFrameworkCore;
using QuanLyKyTucXa_UNETI7_TI17A1ND.Models;
using QuanLyKyTucXa_UNETI7_TI17A1ND.Services;
namespace QuanLyKyTucXa_UNETI7_TI17A1ND.Data;

public static class Seeder
{
    public const string AdminTenDangNhap = "admin", AdminMatKhauMacDinh = "admin123";

    public static bool DamBaoTaiKhoanAdmin(AppDb db)
    {
        var tk = db.TaiKhoans.FirstOrDefault(t => t.TenDangNhap == AdminTenDangNhap);
        if (tk == null)
            db.TaiKhoans.Add(new TaiKhoan { TenDangNhap = AdminTenDangNhap, MatKhau = MatKhauHelper.Bam(AdminMatKhauMacDinh), HoTen = "Quản trị viên", Email = "admin@uneti.edu.vn", VaiTro = TT.Admin });
        else if (MatKhauHelper.Kiem(AdminMatKhauMacDinh, tk.MatKhau) && tk.VaiTro == TT.Admin && tk.TrangThai == TT.HoatDong)
            return false;
        else
        { tk.MatKhau = MatKhauHelper.Bam(AdminMatKhauMacDinh); tk.VaiTro = TT.Admin; tk.TrangThai = TT.HoatDong; }
        db.SaveChanges();
        return true;
    }

    public static void Run(AppDb db)
    {
        if (db.TaiKhoans.Any()) return;
        SeedData(db);
    }

    public static void XoaVaTaoLai(AppDb db)
    {
        using var transaction = db.Database.BeginTransaction();
        db.PhanPhongs.RemoveRange(db.PhanPhongs);
        db.DangKys.RemoveRange(db.DangKys);
        db.SinhViens.RemoveRange(db.SinhViens);
        db.Phongs.RemoveRange(db.Phongs);
        db.LoaiPhongs.RemoveRange(db.LoaiPhongs);
        db.TaiKhoans.RemoveRange(db.TaiKhoans);
        db.SaveChanges();

        if (db.Database.IsSqlite())
            db.Database.ExecuteSqlRaw("DELETE FROM sqlite_sequence;");

        SeedData(db);
        transaction.Commit();
    }

    private static void SeedData(AppDb db)
    {
        db.TaiKhoans.Add(new TaiKhoan { TenDangNhap = AdminTenDangNhap, MatKhau = MatKhauHelper.Bam(AdminMatKhauMacDinh), HoTen = "Quản trị viên", Email = "admin@uneti.edu.vn", VaiTro = TT.Admin });
        var caps = new[] { 2, 4, 6, 8 };
        for (int i = 0; i < 4; i++)
            db.LoaiPhongs.Add(new LoaiPhong { TenLoaiPhong = $"Phòng {caps[i]} người", SoNguoiToiDa = caps[i], DonGiaThang = 300000 + 100000 * (3 - i), MoTa = $"Phòng tiêu chuẩn {caps[i]} chỗ" });
        db.SaveChanges();
        var loais = db.LoaiPhongs.OrderBy(l => l.SoNguoiToiDa).ToList();
        for (int n = 1; n <= 20; n++)
            db.Phongs.Add(new PhongKTX
            {
                TenPhong = $"P{n:00}", MaLoaiPhong = loais[(n - 1) % 4].MaLoaiPhong,
                ToaNha = n <= 10 ? "A" : "B", Tang = (n - 1) % 5 + 1,
                GioiTinhPhong = n <= 10 ? TT.Nam : TT.Nu,
                TrangThai = (n == 10 || n == 20) ? TT.PhongTam : TT.PhongDung, MoTa = "Phòng ký túc xá"
            });
        var sinhVienMau = new[]
        {
            (HoTen: "Nguyễn Văn An", GioiTinh: TT.Nam),
            (HoTen: "Trần Thu Hà", GioiTinh: TT.Nu),
            (HoTen: "Trần Quốc Bảo", GioiTinh: TT.Nam),
            (HoTen: "Nguyễn Ngọc Lan", GioiTinh: TT.Nu),
            (HoTen: "Lê Minh Cường", GioiTinh: TT.Nam),
            (HoTen: "Phạm Thị Mai", GioiTinh: TT.Nu),
            (HoTen: "Phạm Đức Dũng", GioiTinh: TT.Nam),
            (HoTen: "Lê Thanh Hương", GioiTinh: TT.Nu),
            (HoTen: "Hoàng Gia Huy", GioiTinh: TT.Nam),
            (HoTen: "Hoàng Khánh Linh", GioiTinh: TT.Nu),
            (HoTen: "Vũ Thành Long", GioiTinh: TT.Nam),
            (HoTen: "Vũ Thảo Nguyên", GioiTinh: TT.Nu),
            (HoTen: "Đặng Quang Minh", GioiTinh: TT.Nam),
            (HoTen: "Đặng Minh Châu", GioiTinh: TT.Nu),
            (HoTen: "Bùi Anh Tuấn", GioiTinh: TT.Nam),
            (HoTen: "Bùi Phương Anh", GioiTinh: TT.Nu),
            (HoTen: "Đỗ Nhật Nam", GioiTinh: TT.Nam),
            (HoTen: "Đỗ Quỳnh Trang", GioiTinh: TT.Nu),
            (HoTen: "Hồ Khắc Việt", GioiTinh: TT.Nam),
            (HoTen: "Hồ Bảo Ngọc", GioiTinh: TT.Nu),
            (HoTen: "Ngô Tiến Đạt", GioiTinh: TT.Nam),
            (HoTen: "Ngô Yến Nhi", GioiTinh: TT.Nu),
            (HoTen: "Dương Văn Khoa", GioiTinh: TT.Nam),
            (HoTen: "Dương Thu Trang", GioiTinh: TT.Nu),
            (HoTen: "Mai Đức Thịnh", GioiTinh: TT.Nam),
            (HoTen: "Mai Thùy Dương", GioiTinh: TT.Nu),
            (HoTen: "Đinh Công Thành", GioiTinh: TT.Nam),
            (HoTen: "Đinh Hà My", GioiTinh: TT.Nu),
            (HoTen: "Lý Hoàng Phúc", GioiTinh: TT.Nam),
            (HoTen: "Lý Gia Hân", GioiTinh: TT.Nu)
        };
        var _rng = new Random();
        var MaDangNhapDaDung = new HashSet<string>(new[] { "admin" });
        void MaSinhVienMau(int stt, out string maSinhVien, out string tenDangNhap)
        {
            // Tạo mã sinh viên theo format SV001, SV002, ..., SV020, SV021, ...
            maSinhVien = $"SV{stt:000}";
            tenDangNhap = maSinhVien;
        }
        string[] maSvMau = new string[31];
        for (int i = 1; i <= 30; i++)
        {
            var thongTin = sinhVienMau[i - 1];
            MaSinhVienMau(i, out var maSinhVien, out var tenDangNhap);
            maSvMau[i] = maSinhVien;
            var email = $"sv{i:000}@example.test";
            var tk = new TaiKhoan { TenDangNhap = tenDangNhap, MatKhau = MatKhauHelper.Bam(tenDangNhap), HoTen = thongTin.HoTen, Email = email, VaiTro = TT.SinhVien };
            db.TaiKhoans.Add(tk);
            db.SinhViens.Add(new SinhVien { MaSinhVien = maSinhVien, TaiKhoan = tk, HoTen = tk.HoTen, GioiTinh = thongTin.GioiTinh, Lop = "DHTI17A1ND", Khoa = "CNTT", SoDienThoai = $"09000000{i:00}", Email = tk.Email, DiaChi = "Hà Nội" });
        }
        db.SaveChanges();
        var ph = db.Phongs.ToDictionary(p => p.TenPhong, p => p.MaPhong);
        var now = DateTime.Today;
        void Dk(int sv, string tt, string phong = null, string ptt = null, string chuyenTu = null)
        {
            int tre = sv * 9;
            var d = new DangKyKTX { MaSinhVien = maSvMau[sv], NgayDangKy = now.AddDays(-30 - tre), NgayBatDau = now.AddDays(-20 - tre), NgayKetThucDuKien = now.AddMonths(6), TrangThai = tt, NguyenVongLoaiPhong = loais[sv % 4].MaLoaiPhong };
            db.DangKys.Add(d);
            if (chuyenTu != null) d.PhanPhongs.Add(new PhanPhong { MaPhong = ph[chuyenTu], NgayVao = now.AddDays(-20 - tre), NgayRa = now.AddDays(-5 - tre), TrangThai = TT.DaChuyen });
            if (phong != null) d.PhanPhongs.Add(new PhanPhong { MaPhong = ph[phong], NgayVao = now.AddDays((chuyenTu != null ? -5 : -20) - tre), NgayRa = ptt == TT.DaTra ? now.AddDays(-2 - tre) : null, TrangThai = ptt });
        }
        for (int i = 28; i >= 18; i--) Dk(i, TT.Cho);
        Dk(17, TT.DaHuy); Dk(16, TT.DaHuy);
        for (int i = 15; i >= 10; i--) Dk(i, TT.Cho);
        Dk(9, TT.DaPhan, "P03", TT.Dang, chuyenTu: "P02");
        Dk(8, TT.DaTra, "P14", TT.DaTra);
        Dk(7, TT.DaPhan, "P02", TT.Dang);
        Dk(6, TT.DaPhan, "P14", TT.Dang);
        Dk(5, TT.DaPhan, "P05", TT.Dang);
        Dk(4, TT.DaPhan, "P13", TT.Dang);
        Dk(3, TT.DaPhan, "P01", TT.Dang);
        Dk(2, TT.DaPhan, "P13", TT.Dang);
        Dk(1, TT.DaPhan, "P01", TT.Dang);
        db.SaveChanges();
    }
}
