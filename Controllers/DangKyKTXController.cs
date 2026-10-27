using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using QuanLyKyTucXa_UNETI7_DHTI17A1ND.Data;
using QuanLyKyTucXa_UNETI7_DHTI17A1ND.Filters;
using QuanLyKyTucXa_UNETI7_DHTI17A1ND.Models;

namespace QuanLyKyTucXa_UNETI7_DHTI17A1ND.Controllers;

[Quyen]
public class DangKyKTXController : Controller
{
    private readonly AppDb db;
    public DangKyKTXController(AppDb db) { this.db = db; }

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

    [Quyen(TT.SinhVien)]
    public IActionResult Me()
    {
        var maSv = MaSinhVienCuaToi();
        if (maSv == null) return RedirectToAction("TuChoi", "TaiKhoan");
        var sv = db.SinhViens.Include(s => s.TaiKhoan)
            .Include(s => s.DangKys).ThenInclude(d => d.PhanPhongs).ThenInclude(p => p.Phong).ThenInclude(p => p.LoaiPhong)
            .FirstOrDefault(s => s.MaSinhVien == maSv);
        return sv == null ? NotFound() : View(sv);
    }

    [Quyen(TT.SinhVien)]
    public IActionResult Create()
    {
        var maSv = MaSinhVienCuaToi();
        if (maSv == null) return RedirectToAction("TuChoi", "TaiKhoan");
        var sv = db.SinhViens.Find(maSv);
        if (sv == null) return NotFound();
        if (sv.TrangThai != TT.HoatDong)
        {
            TempData["Loi"] = "Tài khoản của bạn đang bị khóa, không thể đăng ký ở ký túc xá. Vui lòng liên hệ ban quản lý.";
            return RedirectToAction(nameof(Me));
        }
        if (db.DangKys.Any(d => d.MaSinhVien == maSv && d.TrangThai == TT.Cho))
        {
            TempData["Loi"] = "Bạn đã có một đăng ký đang chờ duyệt, vui lòng xem tại Thông tin của tôi.";
            return RedirectToAction(nameof(Me));
        }
        if (db.DangKys.Any(d => d.MaSinhVien == maSv && d.TrangThai == TT.DaPhan))
        {
            TempData["Loi"] = "Bạn đang ở ký túc xá (đăng ký đã được phân phòng), không thể tạo thêm đăng ký. Hãy trả phòng trước nếu muốn đăng ký lại.";
            return RedirectToAction(nameof(Me));
        }
        var m = new DangKyKTX { MaSinhVien = maSv };
        ViewBag.Loais = new SelectList(db.LoaiPhongs.Where(l => l.TrangThai == TT.HoatDong).ToList(), "MaLoaiPhong", "TenLoaiPhong");
        return View(m);
    }

    [Quyen(TT.SinhVien), HttpPost, ValidateAntiForgeryToken]
    public IActionResult Create([Bind("NgayBatDau,NgayKetThucDuKien,NguyenVongLoaiPhong,GhiChu")] DangKyKTX m)
    {
        var maSv = MaSinhVienCuaToi();
        if (maSv == null) return RedirectToAction("TuChoi", "TaiKhoan");
        var sv = db.SinhViens.Find(maSv);
        if (sv != null && sv.TrangThai != TT.HoatDong)
        {
            TempData["Loi"] = "Tài khoản của bạn đang bị khóa, không thể đăng ký ở ký túc xá.";
            return RedirectToAction(nameof(Me));
        }
        if (db.DangKys.Any(d => d.MaSinhVien == maSv && d.TrangThai == TT.Cho))
        {
            TempData["Loi"] = "Bạn đã có một đăng ký đang chờ duyệt.";
            return RedirectToAction(nameof(Me));
        }
        if (db.DangKys.Any(d => d.MaSinhVien == maSv && d.TrangThai == TT.DaPhan))
        {
            TempData["Loi"] = "Bạn đang ở ký túc xá (đăng ký đã được phân phòng), không thể tạo thêm đăng ký. Hãy trả phòng trước nếu muốn đăng ký lại.";
            return RedirectToAction(nameof(Me));
        }
        m.MaSinhVien = maSv;
        m.NgayDangKy = DateTime.Now;
        m.TrangThai = TT.Cho;
        ModelState.Remove("MaSinhVien");
        if (!db.LoaiPhongs.Any(l => l.MaLoaiPhong == m.NguyenVongLoaiPhong && l.TrangThai == TT.HoatDong))
            ModelState.AddModelError("NguyenVongLoaiPhong", "Vui lòng chọn nguyện vọng loại phòng hợp lệ.");
        if (m.NgayKetThucDuKien <= m.NgayBatDau) ModelState.AddModelError("NgayKetThucDuKien", "Ngày kết thúc phải sau ngày bắt đầu.");
        if (m.NgayBatDau < DateTime.Today) ModelState.AddModelError("NgayBatDau", "Ngày bắt đầu không được ở quá khứ.");
        if (!ModelState.IsValid)
        {
            ViewBag.Loais = new SelectList(db.LoaiPhongs.Where(l => l.TrangThai == TT.HoatDong).ToList(), "MaLoaiPhong", "TenLoaiPhong", m.NguyenVongLoaiPhong);
            return View(m);
        }
        db.DangKys.Add(m); db.SaveChanges();
        TempData["Ok"] = "Đã gửi đăng ký ở ký túc xá, ban quản lý sẽ xét duyệt sớm nhất.";
        return RedirectToAction(nameof(Me));
    }

    [Quyen(TT.SinhVien), HttpPost, ValidateAntiForgeryToken]
    public IActionResult Huy(int id)
    {
        var maSv = MaSinhVienCuaToi();
        if (maSv == null) return RedirectToAction("TuChoi", "TaiKhoan");
        var dk = db.DangKys.FirstOrDefault(d => d.MaDangKy == id && d.MaSinhVien == maSv);
        if (dk == null) return NotFound();
        if (dk.TrangThai != TT.Cho)
        {
            TempData["Loi"] = "Chỉ có thể hủy đăng ký đang ở trạng thái Chờ duyệt.";
            return RedirectToAction(nameof(Me));
        }
        db.DangKys.Remove(dk); db.SaveChanges();
        TempData["Ok"] = "Đã hủy đăng ký ở ký túc xá.";
        return RedirectToAction(nameof(Me));
    }

    private string MaSinhVienCuaToi() =>
        HttpContext.Session.GetInt32("MaTaiKhoan") == null ? null
            : db.SinhViens.AsNoTracking().FirstOrDefault(s => s.MaTaiKhoan == HttpContext.Session.GetInt32("MaTaiKhoan"))?.MaSinhVien;

    [Quyen(TT.Admin)]
    public IActionResult Details(int id)
    {
        var d = db.DangKys.Include(x => x.SinhVien).Include(x => x.LoaiPhongNV)
            .Include(x => x.PhanPhongs).ThenInclude(p => p.Phong)
            .FirstOrDefault(x => x.MaDangKy == id);
        return d == null ? NotFound() : View(d);
    }

    [Quyen(TT.Admin)]
    public IActionResult Duyet(int id)
    {
        var dk = db.DangKys.Include(x => x.SinhVien).Include(x => x.LoaiPhongNV).FirstOrDefault(x => x.MaDangKy == id);
        if (dk == null)
            return NotFound();

        ViewBag.NgayVao = DateTime.Today.ToString("yyyy-MM-dd");
        ViewBag.Phongs = PhongPhuHop(dk.SinhVien.GioiTinh, null, dk.NguyenVongLoaiPhong);
        return View(dk);
    }

    [Quyen(TT.Admin), HttpPost, ValidateAntiForgeryToken]
    public IActionResult DuyetXacNhan(int id, int maPhong, DateTime? ngayVao)
    {
        var dk = db.DangKys.Include(x => x.SinhVien).Include(x => x.LoaiPhongNV).FirstOrDefault(x => x.MaDangKy == id);
        if (dk == null)
            return NotFound();

        string loi = null;
        if (dk.TrangThai != TT.Cho) loi = "Đăng ký không ở trạng thái Chờ duyệt.";
        else if (dk.SinhVien.TrangThai != TT.HoatDong) loi = "Sinh viên đang bị khóa (không hoạt động), không thể phân phòng.";
        else if (db.PhanPhongs.Any(p => p.TrangThai == TT.Dang && p.DangKy.MaSinhVien == dk.MaSinhVien)) loi = "Sinh viên đang ở một phòng khác.";
        else if (ngayVao == null) loi = "Vui lòng chọn ngày vào ở.";
        else if (ngayVao.Value.Date < DateTime.Today) loi = "Ngày vào ở không được ở quá khứ.";
        else if (ngayVao.Value.Date > dk.NgayKetThucDuKien.Date) loi = "Ngày vào ở không được sau ngày kết thúc dự kiến.";
        else loi = KiemTraPhong(maPhong, dk.SinhVien.GioiTinh);
        if (loi != null)
        {
            ViewBag.Loi = loi;
            ViewBag.MaPhong = maPhong;
            ViewBag.NgayVao = (ngayVao ?? DateTime.Today).ToString("yyyy-MM-dd");
            ViewBag.Phongs = PhongPhuHop(dk.SinhVien.GioiTinh, null, dk.NguyenVongLoaiPhong);
            return View(dk);
        }

        db.PhanPhongs.Add(new PhanPhong { MaDangKy = dk.MaDangKy, MaPhong = maPhong, NgayVao = ngayVao.Value.Date, TrangThai = TT.Dang });
        dk.TrangThai = TT.DaPhan;
        db.SaveChanges();
        TempData["Ok"] = "Duyệt và phân phòng thành công.";
        return RedirectToAction(nameof(Index));
    }

    [Quyen(TT.Admin)]
    public IActionResult TuChoi(int id)
    {
        var dk = db.DangKys.Include(x => x.SinhVien).Include(x => x.LoaiPhongNV).FirstOrDefault(x => x.MaDangKy == id);
        return dk == null ? NotFound() : View(dk);
    }

    [Quyen(TT.Admin), HttpPost, ValidateAntiForgeryToken]
    public IActionResult TuChoiXacNhan(int id, string ghiChu = null)
    {
        var dk = db.DangKys.Find(id);
        if (dk == null)
            return NotFound();

        if (dk.TrangThai != TT.Cho)
        {
            TempData["Loi"] = "Chỉ đăng ký ở trạng thái Chờ duyệt mới được từ chối.";
            return RedirectToAction(nameof(Index));
        }

        dk.TrangThai = TT.TuChoi;
        dk.GhiChu = string.IsNullOrWhiteSpace(ghiChu) ? null : ghiChu.Trim();
        db.SaveChanges();
        TempData["Ok"] = "Đã từ chối đăng ký.";
        return RedirectToAction(nameof(Index));
    }

    private List<PhongRow> PhongPhuHop(string gt, int? boQua = null, int? uuTien = null) =>
        db.Phongs.Where(p => p.TrangThai == TT.PhongDung && p.GioiTinhPhong == gt && p.MaPhong != boQua)
            .Select(p => new PhongRow { P = p, TenLoai = p.LoaiPhong.TenLoaiPhong, SucChua = p.LoaiPhong.SoNguoiToiDa, DangO = p.PhanPhongs.Count(x => x.TrangThai == TT.Dang) })
            .Where(r => r.SucChua - r.DangO > 0).ToList()
            .OrderByDescending(r => uuTien != null && r.P.MaLoaiPhong == uuTien).ThenBy(r => r.P.TenPhong).ToList();

    private string KiemTraPhong(int maPhong, string gt)
    {
        var p = db.Phongs.Include(x => x.LoaiPhong).FirstOrDefault(x => x.MaPhong == maPhong);
        if (p == null) return "Phòng không tồn tại.";
        if (p.TrangThai != TT.PhongDung) return "Phòng đang tạm ngừng.";
        if (p.GioiTinhPhong != gt) return $"Phòng {p.TenPhong} dành cho {p.GioiTinhPhong}, không phù hợp giới tính sinh viên.";
        int dangO = db.PhanPhongs.Count(x => x.MaPhong == maPhong && x.TrangThai == TT.Dang);
        if (dangO >= p.LoaiPhong.SoNguoiToiDa) return $"Phòng {p.TenPhong} đã đủ người ({dangO}/{p.LoaiPhong.SoNguoiToiDa}).";
        return null;
    }
}
