// Họ và tên: Đỗ Văn Tùng
// Mã sinh viên: 23203100028
// Nội dung thực hiện: Cấu hình Session và đăng nhập
//
// Họ và tên: Hoàng Tiến Nam
// Mã sinh viên: 23203100029
// Nội dung thực hiện: Đăng ký DbContext/Controller cho module phòng
//
// Họ và tên: Phạm Quang Nhật
// Mã sinh viên: 23203100085
// Nội dung thực hiện: Cấu hình chung của Project
//
// Họ và tên: Nguyễn Anh Tú
// Mã sinh viên: 23203100027
// Nội dung thực hiện: Cấu hình chung của Project

using Microsoft.EntityFrameworkCore;
using QuanLyKyTucXa_UNETI7_TI17A1ND.Data;

var b = WebApplication.CreateBuilder(args);
var databaseProvider = b.Configuration.GetValue("DatabaseProvider", "Sqlite");
var connectionString = b.Configuration.GetConnectionString("KTX");

b.Services.AddControllersWithViews(o =>
{
    var mp = o.ModelBindingMessageProvider;
    mp.SetValueMustNotBeNullAccessor(_ => "Vui lòng nhập giá trị.");
    mp.SetMissingKeyOrValueAccessor(() => "Vui lòng nhập giá trị.");
    mp.SetMissingBindRequiredValueAccessor(f => $"Thiếu giá trị cho {f}.");
    mp.SetAttemptedValueIsInvalidAccessor((v, f) => $"Giá trị '{v}' không hợp lệ cho {f}.");
    mp.SetUnknownValueIsInvalidAccessor(f => $"Giá trị không hợp lệ cho {f}.");
    mp.SetValueIsInvalidAccessor(v => $"Giá trị '{v}' không hợp lệ.");
    mp.SetValueMustBeANumberAccessor(f => $"{f} phải là số.");
});

if (string.Equals(databaseProvider, "SqlServer", StringComparison.OrdinalIgnoreCase))
{
    b.Services.AddDbContext<AppDb>(o => o.UseSqlServer(connectionString));
}
else
{
    b.Services.AddDbContext<AppDb>(o => o.UseSqlite(connectionString));
}

b.Services.AddDistributedMemoryCache();
b.Services.AddSession(o => o.IdleTimeout = TimeSpan.FromMinutes(30));
var app = b.Build();
using (var s = app.Services.CreateScope())
{
    var db = s.ServiceProvider.GetRequiredService<AppDb>();
    if (string.Equals(databaseProvider, "SqlServer", StringComparison.OrdinalIgnoreCase))
    {
        db.Database.Migrate();
    }
    else
    {
        db.Database.EnsureCreated();
    }

    if (b.Configuration.GetValue("DuLieuMau:TaoLaiKhiKhoiDong", false))
        Seeder.XoaVaTaoLai(db);
    else
        Seeder.Run(db);
    if (b.Configuration.GetValue("TaiKhoanMacDinh:DatLaiAdminKhiKhoiDong", false)) Seeder.DamBaoTaiKhoanAdmin(db);
}
// app.UseStatusCodePages("text/html; charset=utf-8", "<p>Không tìm thấy trang hoặc dữ liệu yêu cầu (mã {0}).</p><p><a href='/'>Về trang chủ</a></p>");
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
app.Run();
