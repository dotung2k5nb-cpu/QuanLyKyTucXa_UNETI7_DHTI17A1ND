// Họ và tên: Đỗ Văn Tùng
// Mã sinh viên: 23203100028
// Nội dung thực hiện: Tài khoản, đăng nhập, phân quyền, quản lý loại phòng

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using QuanLyKyTucXa_UNETI7_TI17A1ND.Data;
using QuanLyKyTucXa_UNETI7_TI17A1ND.Models;
namespace QuanLyKyTucXa_UNETI7_TI17A1ND.Filters;

public class QuyenAttribute : ActionFilterAttribute
{
    private readonly string _role;
    public QuyenAttribute(string role = null) { _role = role; }
    public override void OnActionExecuting(ActionExecutingContext c)
    {
        var session = c.HttpContext.Session;
        int? id = session.GetInt32("MaTaiKhoan");
        if (id == null) { c.Result = new RedirectToActionResult("Login", "TaiKhoan", null); return; }

        var db = c.HttpContext.RequestServices.GetRequiredService<AppDb>();
        var tk = db.TaiKhoans.AsNoTracking().Where(t => t.MaTaiKhoan == id.Value)
            .Select(t => new { t.VaiTro, t.TrangThai, t.HoTen }).FirstOrDefault();
        if (tk == null || tk.TrangThai == TT.Khoa)
        {
            session.Clear();
            c.Result = new RedirectToActionResult("Login", "TaiKhoan", new { lyDo = "khoa" });
            return;
        }
        session.SetString("VaiTro", tk.VaiTro);
        session.SetString("HoTen", tk.HoTen);
        
        // Fix lỗi 1: Lấy giới tính sinh viên và lưu vào session
        if (tk.VaiTro == TT.SinhVien)
        {
            var sv = db.SinhViens.AsNoTracking()
                .Where(s => s.MaTaiKhoan == id.Value)
                .Select(s => s.GioiTinh)
                .FirstOrDefault();
            if (sv != null)
                session.SetString("GioiTinh", sv);
        }
        if (_role != null && tk.VaiTro != _role) c.Result = new RedirectToActionResult("TuChoi", "TaiKhoan", null);
    }
}
