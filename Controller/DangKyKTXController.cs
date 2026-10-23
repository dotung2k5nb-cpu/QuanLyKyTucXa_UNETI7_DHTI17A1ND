// Họ và tên: Phạm Quang Nhật
// Mã sinh viên: 23203100085
// Nội dung thực hiện: Quản lý sinh viên, đăng ký ở KTX, xem thông tin/lịch sử ở

using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyKyTucXa_UNETI7_TI17A1ND.Data;
using QuanLyKyTucXa_UNETI7_TI17A1ND.Filters;
using QuanLyKyTucXa_UNETI7_TI17A1ND.Models;
namespace QuanLyKyTucXa_UNETI7_TI17A1ND.Controllers;

[Quyen]
public class DangKyKTXController : Controller
{
    private readonly AppDb db;
    public DangKyKTXController(AppDb db) { this.db = db; }

    private SinhVien SvHienTai()
    {
        int? id = HttpContext.Session.GetInt32("MaTaiKhoan");
        return id == null ? null : db.SinhViens.FirstOrDefault(s => s.MaTaiKhoan == id);
    }
    
    [Quyen(TT.Admin)]
    public IActionResult Index(string maSv, string hoTen, string tt, int page = 1)
    {
        var l = db.DangKys.Include(d => d.SinhVien).Include(d => d.LoaiPhongNV).AsQueryable();
        if (!string.IsNullOrWhiteSpace(maSv)) { maSv = maSv.Trim(); l = l.Where(d => d.MaSinhVien.Contains(maSv)); }
        if (!string.IsNullOrWhiteSpace(hoTen)) { hoTen = hoTen.Trim(); l = l.Where(d => d.SinhVien.HoTen.Contains(hoTen)); }
        if (!string.IsNullOrEmpty(tt)) l = l.Where(d => d.TrangThai == tt);
        var pager = new PagerVM(l.Count(), page, ("maSv", maSv), ("hoTen", hoTen), ("tt", tt));
        var items = l.OrderByDescending(d => d.NgayDangKy).ThenByDescending(d => d.MaDangKy)
            .Skip(pager.Skip).Take(PagerVM.Size).ToList();
        return View(new DangKyListVM { Items = items, Pager = pager, MaSinhVien = maSv, HoTen = hoTen, Tt = tt });
    }

    [Quyen(TT.Admin)]
    public IActionResult Details(int id)
    {
        var d = db.DangKys.Include(x => x.SinhVien).Include(x => x.LoaiPhongNV)
            .Include(x => x.PhanPhongs).ThenInclude(p => p.Phong)
            .FirstOrDefault(x => x.MaDangKy == id);
        return d == null ? NotFound() : View(d);
    }

    [Quyen(TT.SinhVien)]
    public IActionResult Create()
    {
        ViewBag.Loais = new SelectList(db.LoaiPhongs.Where(l => l.TrangThai == TT.HoatDong), "MaLoaiPhong", "TenLoaiPhong");
        return View(new DangKyKTX());
    }

    private IActionResult FormLai(DangKyKTX m)
    {
        ViewBag.Loais = new SelectList(db.LoaiPhongs.Where(l => l.TrangThai == TT.HoatDong), "MaLoaiPhong", "TenLoaiPhong", m.NguyenVongLoaiPhong);
        return View(m);
    }

    private void KiemTraDangKyHienCo(string maSinhVien)
    {
        if (db.DangKys.Any(d => d.MaSinhVien == maSinhVien && d.TrangThai == TT.DaPhan)) ModelState.AddModelError("", "Bạn đang có đăng ký đã phân phòng.");
        if (db.DangKys.Any(d => d.MaSinhVien == maSinhVien && d.TrangThai == TT.Cho)) ModelState.AddModelError("", "Bạn đã có đăng ký đang chờ duyệt.");
    }

    [Quyen(TT.SinhVien), HttpPost, ValidateAntiForgeryToken]
    public IActionResult Create([Bind("NgayBatDau,NgayKetThucDuKien,NguyenVongLoaiPhong,GhiChu")] DangKyKTX m)
    {
        var sv = SvHienTai();
        ModelState.Remove("MaSinhVien"); ModelState.Remove("SinhVien"); ModelState.Remove("LoaiPhongNV");
        if (sv == null || sv.TrangThai != TT.HoatDong) ModelState.AddModelError("", "Sinh viên không tồn tại hoặc không hoạt động.");
        if (m.NgayBatDau.Date < DateTime.Today) ModelState.AddModelError("NgayBatDau", "Ngày bắt đầu không được ở quá khứ");
        if (m.NguyenVongLoaiPhong <= 0 || !db.LoaiPhongs.Any(l => l.MaLoaiPhong == m.NguyenVongLoaiPhong && l.TrangThai == TT.HoatDong))
            ModelState.AddModelError("NguyenVongLoaiPhong", "Loại phòng nguyện vọng không hợp lệ");
        if (m.NgayKetThucDuKien.Date <= m.NgayBatDau.Date) ModelState.AddModelError("NgayKetThucDuKien", "Ngày kết thúc phải sau ngày bắt đầu");
        if (sv != null) KiemTraDangKyHienCo(sv.MaSinhVien);
        if (!ModelState.IsValid) return FormLai(m);

        try
        {
            using var tx = db.Database.BeginTransaction(IsolationLevel.Serializable);
            KiemTraDangKyHienCo(sv.MaSinhVien);
            if (!ModelState.IsValid) return FormLai(m);
            m.MaSinhVien = sv.MaSinhVien; m.NgayDangKy = DateTime.Now; m.TrangThai = TT.Cho; m.MaDangKy = 0;
            db.DangKys.Add(m); db.SaveChanges();
            tx.Commit();
        }
        catch (Exception ex) when (LoiCsdl.LaTrungKhoa(ex))
        {
            ModelState.AddModelError("", "Bạn vừa gửi một đăng ký khác đang chờ duyệt. Vui lòng kiểm tra trong mục Thông tin của tôi.");
            return FormLai(m);
        }
        catch (Exception ex) when (LoiCsdl.LaDeadlock(ex))
        {
            ModelState.AddModelError("", "Hệ thống đang bận xử lý, vui lòng bấm Gửi đăng ký lại.");
            return FormLai(m);
        }
        return RedirectToAction(nameof(Me));
    }

    [Quyen(TT.SinhVien)]
    public IActionResult Me()
    {
        var sv = SvHienTai(); if (sv == null) return NotFound();
        var m = db.SinhViens.Include(s => s.TaiKhoan)
            .Include(s => s.DangKys).ThenInclude(d => d.PhanPhongs).ThenInclude(p => p.Phong).ThenInclude(p => p.LoaiPhong)
            .First(s => s.MaSinhVien == sv.MaSinhVien);
        return View(m);
    }

    [Quyen(TT.SinhVien), HttpPost, ValidateAntiForgeryToken]
    public IActionResult Huy(int id)
    {
        var sv = SvHienTai(); if (sv == null) return NotFound();
        if (!db.DangKys.Any(x => x.MaDangKy == id && x.MaSinhVien == sv.MaSinhVien)) return NotFound();
        int n = db.DangKys.Where(x => x.MaDangKy == id && x.MaSinhVien == sv.MaSinhVien && x.TrangThai == TT.Cho)
            .ExecuteUpdate(s => s.SetProperty(x => x.TrangThai, TT.DaHuy));
        if (n == 0) TempData["Loi"] = "Chỉ được hủy đăng ký đang chờ duyệt.";
        return RedirectToAction(nameof(Me));
    }
}
