// Họ và tên: Phạm Quang Nhật
// Mã sinh viên: 23203100085
// Nội dung thực hiện: Quản lý sinh viên, đăng ký ở KTX, xem thông tin/lịch sử ở

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyKyTucXa_UNETI7_TI17A1ND.Data;
using QuanLyKyTucXa_UNETI7_TI17A1ND.Filters;
using QuanLyKyTucXa_UNETI7_TI17A1ND.Models;
using QuanLyKyTucXa_UNETI7_TI17A1ND.Services;
namespace QuanLyKyTucXa_UNETI7_TI17A1ND.Controllers;

[Quyen(TT.Admin)]
public class SinhVienController : Controller
{
    private readonly AppDb db;
    public SinhVienController(AppDb db) { this.db = db; }

    public IActionResult Index(string maSv, string hoTen, string lop, string khoa, string gioiTinh, string tt)
    {
        var l = db.SinhViens.AsQueryable();
        if (!string.IsNullOrWhiteSpace(maSv)) { maSv = maSv.Trim(); l = l.Where(s => s.MaSinhVien.Contains(maSv)); }
        if (!string.IsNullOrWhiteSpace(hoTen)) { hoTen = hoTen.Trim(); l = l.Where(s => s.HoTen.Contains(hoTen)); }
        if (!string.IsNullOrWhiteSpace(lop)) { lop = lop.Trim(); l = l.Where(s => s.Lop.Contains(lop)); }
        if (!string.IsNullOrWhiteSpace(khoa)) { khoa = khoa.Trim(); l = l.Where(s => s.Khoa.Contains(khoa)); }
        if (!string.IsNullOrWhiteSpace(gioiTinh)) l = l.Where(s => s.GioiTinh == gioiTinh);
        if (!string.IsNullOrWhiteSpace(tt)) l = l.Where(s => s.TrangThai == tt);
        return View(new SinhVienListVM
        {
            Items = l.OrderBy(s => s.MaSinhVien).ToList(),
            MaSinhVien = maSv, HoTen = hoTen, Lop = lop, Khoa = khoa, GioiTinh = gioiTinh, Tt = tt
        });
    }

    public IActionResult LichSu(string id)
    {
        var sv = db.SinhViens.Include(s => s.TaiKhoan).FirstOrDefault(s => s.MaSinhVien == id);
        if (sv == null) return NotFound();
        var dks = db.DangKys.Include(d => d.LoaiPhongNV).Where(d => d.MaSinhVien == id).OrderByDescending(d => d.NgayDangKy).ToList();
        var pps = db.PhanPhongs.Include(p => p.Phong).ThenInclude(p => p.LoaiPhong)
            .Where(p => p.DangKy.MaSinhVien == id).OrderBy(p => p.NgayVao).ThenBy(p => p.MaPhanPhong).ToList();
        return View(new SinhVienLichSuVM
        {
            SinhVien = sv, DangKys = dks, PhanPhongs = pps,
            SoLanChuyen = pps.Count(p => p.TrangThai == TT.DaChuyen),
            SoPhongDaO = pps.Select(p => p.MaPhong).Distinct().Count()
        });
    }

    public IActionResult Create() { ViewBag.Moi = true; return View("Form", new SinhVienFormVM()); }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Create([Bind("MaSinhVien,HoTen,NgaySinh,GioiTinh,Lop,Khoa,SoDienThoai,Email,DiaChi,MatKhau,XacNhan")] SinhVienFormVM m)
    {
        ModelState.Remove("TaiKhoan");
        m.TrangThai = TT.HoatDong; ModelState.Remove("TrangThai");
        if (db.SinhViens.Any(x => x.MaSinhVien == m.MaSinhVien) || db.TaiKhoans.Any(t => t.TenDangNhap == m.MaSinhVien)) ModelState.AddModelError("MaSinhVien", "Mã sinh viên đã tồn tại");
        if (m.NgaySinh >= DateTime.Today || m.NgaySinh < new DateTime(1980, 1, 1)) ModelState.AddModelError("NgaySinh", "Ngày sinh không hợp lệ");
        if (string.IsNullOrWhiteSpace(m.MatKhau)) ModelState.AddModelError("MatKhau", "Nhập mật khẩu đăng nhập cho sinh viên");
        if (m.MatKhau != m.XacNhan) ModelState.AddModelError("XacNhan", "Xác nhận mật khẩu không khớp");
        if (!ModelState.IsValid) { ViewBag.Moi = true; return View("Form", m); }
        m.TaiKhoan = new TaiKhoan { TenDangNhap = m.MaSinhVien, MatKhau = m.MatKhau, HoTen = m.HoTen, Email = m.Email, VaiTro = TT.SinhVien };
        db.SinhViens.Add(m); db.SaveChanges();
        TempData["Ok"] = $"Đã tạo sinh viên và tài khoản {m.MaSinhVien} (mật khẩu đăng nhập do bạn đặt).";
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Edit(string id)
    {
        var sv = db.SinhViens.Find(id);
        if (sv == null) return NotFound();
        ViewBag.Moi = false;
        return View("Form", new SinhVienFormVM
        {
            MaSinhVien = sv.MaSinhVien, HoTen = sv.HoTen, NgaySinh = sv.NgaySinh, GioiTinh = sv.GioiTinh,
            Lop = sv.Lop, Khoa = sv.Khoa, SoDienThoai = sv.SoDienThoai, Email = sv.Email, DiaChi = sv.DiaChi
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Edit(string id, SinhVienFormVM m)
    {
        if (id != m.MaSinhVien) return NotFound();
        ViewBag.Moi = false;
        ModelState.Remove("TaiKhoan");
        var cu = db.SinhViens.Find(id); if (cu == null) return NotFound();
        if (m.NgaySinh >= DateTime.Today) ModelState.AddModelError("NgaySinh", "Ngày sinh không hợp lệ");
        if (m.GioiTinh != cu.GioiTinh)
        {
            var dangO = db.PhanPhongs.Where(x => x.TrangThai == TT.Dang && x.DangKy.MaSinhVien == id)
                .Select(x => new { x.Phong.TenPhong, x.Phong.GioiTinhPhong }).FirstOrDefault();
            if (dangO != null && dangO.GioiTinhPhong != m.GioiTinh)
                ModelState.AddModelError("GioiTinh", $"Không thể đổi giới tính: sinh viên đang ở phòng {dangO.TenPhong} (dành cho {dangO.GioiTinhPhong}). Hãy chuyển phòng hoặc trả phòng trước.");
        }
        if (!string.IsNullOrWhiteSpace(m.MatKhau) && m.MatKhau != m.XacNhan) ModelState.AddModelError("XacNhan", "Xác nhận mật khẩu không khớp");
        if (!ModelState.IsValid) { ViewBag.Moi = false; return View("Form", m); }
        cu.HoTen = m.HoTen; cu.NgaySinh = m.NgaySinh; cu.GioiTinh = m.GioiTinh; cu.Lop = m.Lop; cu.Khoa = m.Khoa;
        cu.SoDienThoai = m.SoDienThoai; cu.Email = m.Email; cu.DiaChi = m.DiaChi;
        var thongBao = $"Đã cập nhật sinh viên {m.HoTen}.";
        var tk = db.TaiKhoans.Find(cu.MaTaiKhoan);
        if (tk != null)
        {
            tk.HoTen = cu.HoTen; tk.Email = cu.Email;
            if (!string.IsNullOrWhiteSpace(m.MatKhau))
            {
                tk.MatKhau = m.MatKhau;
                thongBao += " Đã đặt lại mật khẩu đăng nhập cho sinh viên.";
            }
        }
        db.SaveChanges();
        TempData["Ok"] = thongBao;
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult DatLaiMatKhau(string id)
    {
        var s = db.SinhViens.Find(id); if (s == null) return NotFound();
        var tk = db.TaiKhoans.Find(s.MaTaiKhoan); if (tk == null) return NotFound();
        var mkTam = MatKhauHelper.TaoMatKhauTam();
        tk.MatKhau = mkTam;
        db.SaveChanges();
        TempData["Ok"] = $"Đã đặt lại mật khẩu cho {s.MaSinhVien}. Mật khẩu tạm thời: {mkTam} (chỉ hiển thị một lần, hãy gửi cho sinh viên và yêu cầu đổi mật khẩu).";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Khoa(string id)
    {
        var s = db.SinhViens.Find(id); if (s == null) return NotFound();
        var tk = db.TaiKhoans.Find(s.MaTaiKhoan); if (tk == null) return NotFound();
        s.TrangThai = s.TrangThai == TT.HoatDong ? TT.Khoa : TT.HoatDong;
        tk.TrangThai = s.TrangThai;
        db.SaveChanges(); return RedirectToAction(nameof(Index));
    }
}
