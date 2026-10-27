// Họ và tên: Đỗ Văn Tùng
// Mã sinh viên: 23203100028
// Nội dung thực hiện: Tài khoản, đăng nhập, phân quyền, quản lý loại phòng

using Microsoft.AspNetCore.Mvc;
using QuanLyKyTucXa_UNETI7_TI17A1ND.Data;
using QuanLyKyTucXa_UNETI7_TI17A1ND.Filters;
using QuanLyKyTucXa_UNETI7_TI17A1ND.Models;
using QuanLyKyTucXa_UNETI7_TI17A1ND.Services;
namespace QuanLyKyTucXa_UNETI7_TI17A1ND.Controllers;

public class TaiKhoanController : Controller
{
    private readonly AppDb db;
    public TaiKhoanController(AppDb db) { this.db = db; }

    public IActionResult Login(string lyDo)
    {
        if (lyDo == "khoa") ModelState.AddModelError("", "Tài khoản đã bị khóa hoặc không còn tồn tại. Vui lòng liên hệ quản trị viên.");
        return View(new LoginVM());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Login(LoginVM m)
    {
        if (!ModelState.IsValid) return View(m);
        m.TenDangNhap = m.TenDangNhap.Trim();
        var tk = db.TaiKhoans.FirstOrDefault(x => x.TenDangNhap == m.TenDangNhap);
        bool dung = tk?.MatKhau == m.MatKhau;
        bool chuyenTuHash = !dung && tk != null && MatKhauHelper.Kiem(m.MatKhau, tk.MatKhau);
        if (chuyenTuHash) { tk.MatKhau = m.MatKhau; db.SaveChanges(); }
        if (tk == null) { ModelState.AddModelError(nameof(m.TenDangNhap), "Tên đăng nhập không tồn tại."); return View(m); }
        if (tk.TrangThai == TT.Khoa) { ModelState.AddModelError("", "Tài khoản đã bị khóa. Vui lòng liên hệ quản trị viên."); return View(m); }
        if (!dung && !chuyenTuHash) { ModelState.AddModelError(nameof(m.MatKhau), "Mật khẩu không đúng."); return View(m); }
        HttpContext.Session.Clear();
        HttpContext.Session.SetInt32("MaTaiKhoan", tk.MaTaiKhoan);
        HttpContext.Session.SetString("HoTen", tk.HoTen);
        HttpContext.Session.SetString("VaiTro", tk.VaiTro);
        
        // Fix lỗi 1: Lưu giới tính vào session cho sinh viên
        if (tk.VaiTro == TT.SinhVien)
        {
            var sv = db.SinhViens.FirstOrDefault(s => s.MaTaiKhoan == tk.MaTaiKhoan);
            if (sv != null)
                HttpContext.Session.SetString("GioiTinh", sv.GioiTinh);
        }
        
        TempData["LoginTransition"] = true;
        return RedirectToAction("Index", "Home");
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Logout() { HttpContext.Session.Clear(); return RedirectToAction("Login"); }
    public IActionResult TuChoi() => View();

    [Quyen]
    public IActionResult DoiMatKhau() => View(new DoiMatKhauVM());

    [Quyen, HttpPost, ValidateAntiForgeryToken]
    public IActionResult DoiMatKhau(DoiMatKhauVM m)
    {
        if (!ModelState.IsValid) return View(m);
        int? id = HttpContext.Session.GetInt32("MaTaiKhoan");
        var tk = id == null ? null : db.TaiKhoans.Find(id.Value);
        if (tk == null) return RedirectToAction(nameof(Login));
        if (tk.MatKhau != m.MatKhauCu && !MatKhauHelper.Kiem(m.MatKhauCu, tk.MatKhau)) { ModelState.AddModelError(nameof(m.MatKhauCu), "Mật khẩu hiện tại không đúng."); return View(m); }
        if (m.MatKhauMoi == m.MatKhauCu) { ModelState.AddModelError(nameof(m.MatKhauMoi), "Mật khẩu mới phải khác mật khẩu hiện tại."); return View(m); }
        tk.MatKhau = m.MatKhauMoi;
        db.SaveChanges();
        TempData["Ok"] = "Đã đổi mật khẩu.";
        return RedirectToAction("Index", "Home");
    }
}
