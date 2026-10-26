// Họ và tên: Đỗ Văn Tùng
// Mã sinh viên: 23203100028
// Nội dung thực hiện: DbSet/cấu hình quan hệ TaiKhoan, LoaiPhong
//
// Họ và tên: Hoàng Tiến Nam
// Mã sinh viên: 23203100029
// Nội dung thực hiện: DbSet/cấu hình quan hệ PhongKTX
//
// Họ và tên: Phạm Quang Nhật
// Mã sinh viên: 23203100085
// Nội dung thực hiện: DbSet/cấu hình quan hệ SinhVien, DangKyKTX
//
// Họ và tên: Nguyễn Anh Tú
// Mã sinh viên: 23203100027
// Nội dung thực hiện: DbSet/cấu hình quan hệ PhanPhong

using Microsoft.EntityFrameworkCore;
using QuanLyKyTucXa_UNETI7_TI17A1ND.Models;
namespace QuanLyKyTucXa_UNETI7_TI17A1ND.Data;

public class AppDb : DbContext
{
    public AppDb(DbContextOptions<AppDb> o) : base(o) { }
    public DbSet<TaiKhoan> TaiKhoans { get; set; }
    public DbSet<LoaiPhong> LoaiPhongs { get; set; }
    public DbSet<PhongKTX> Phongs { get; set; }
    public DbSet<SinhVien> SinhViens { get; set; }
    public DbSet<DangKyKTX> DangKys { get; set; }
    public DbSet<PhanPhong> PhanPhongs { get; set; }

    protected override void OnModelCreating(ModelBuilder m)
    {
        var sqlServerFilter = "[TrangThai] = N'{0}'";
        var sqliteFilter = "\"TrangThai\" = '{0}'";

        m.Entity<TaiKhoan>().HasIndex(x => x.TenDangNhap).IsUnique();
        m.Entity<LoaiPhong>().HasIndex(x => x.TenLoaiPhong).IsUnique();
        m.Entity<LoaiPhong>().Property(x => x.DonGiaThang).HasColumnType("decimal(18,0)");
        m.Entity<PhongKTX>().HasIndex(x => x.TenPhong).IsUnique();
        m.Entity<SinhVien>().HasOne(s => s.TaiKhoan).WithOne().HasForeignKey<SinhVien>(s => s.MaTaiKhoan).OnDelete(DeleteBehavior.Restrict);
        m.Entity<PhongKTX>().HasOne(p => p.LoaiPhong).WithMany(l => l.Phongs).HasForeignKey(p => p.MaLoaiPhong).OnDelete(DeleteBehavior.Restrict);
        m.Entity<DangKyKTX>().HasOne(d => d.SinhVien).WithMany(s => s.DangKys).HasForeignKey(d => d.MaSinhVien).OnDelete(DeleteBehavior.Restrict);
        m.Entity<DangKyKTX>().HasOne(d => d.LoaiPhongNV).WithMany().HasForeignKey(d => d.NguyenVongLoaiPhong).OnDelete(DeleteBehavior.Restrict);
        var dangKyChoFilter = Database.IsSqlite() ? sqliteFilter : sqlServerFilter;
        var dangKyDaPhanFilter = Database.IsSqlite() ? sqliteFilter : sqlServerFilter;
        var phanPhongDangFilter = Database.IsSqlite() ? sqliteFilter : sqlServerFilter;

        m.Entity<DangKyKTX>().HasIndex(d => d.MaSinhVien, "UX_DangKys_MotChoDuyetMoiSV").IsUnique().HasFilter(string.Format(dangKyChoFilter, "Chờ duyệt"));
        m.Entity<DangKyKTX>().HasIndex(d => d.MaSinhVien, "UX_DangKys_MotDaPhanPhongMoiSV").IsUnique().HasFilter(string.Format(dangKyDaPhanFilter, "Đã phân phòng"));
        m.Entity<PhanPhong>().HasIndex(x => x.MaDangKy, "UX_PhanPhongs_MotDangOMoiDangKy").IsUnique().HasFilter(string.Format(phanPhongDangFilter, "Đang ở"));
        m.Entity<PhanPhong>().HasOne(x => x.DangKy).WithMany(d => d.PhanPhongs).HasForeignKey(x => x.MaDangKy).OnDelete(DeleteBehavior.Restrict);
        m.Entity<PhanPhong>().HasOne(x => x.Phong).WithMany(p => p.PhanPhongs).HasForeignKey(x => x.MaPhong).OnDelete(DeleteBehavior.Restrict);
    }
}
