// Họ và tên: Nguyễn Anh Tú
// Mã sinh viên: 23203100027
// Nội dung thực hiện: Phân phòng, chuyển phòng, trả phòng, Dashboard, thống kê

using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyKyTucXa_UNETI7_TI17A1ND.Data;
using QuanLyKyTucXa_UNETI7_TI17A1ND.Filters;
using QuanLyKyTucXa_UNETI7_TI17A1ND.Models;
namespace QuanLyKyTucXa_UNETI7_TI17A1ND.Controllers;

[Quyen(TT.Admin)]
public class PhanPhongController : Controller
{
    private readonly AppDb db;
    public PhanPhongController(AppDb db) { this.db = db; }

    public IActionResult Index(string maSv, string hoTen, string tenPhong, string tt, int page = 1)
    {
        var cho = db.DangKys.Where(d => d.TrangThai == TT.Cho);
        ViewBag.ChoTong = cho.Count();
        ViewBag.Cho = cho.Include(d => d.SinhVien).OrderBy(d => d.NgayDangKy).Take(10).ToList();

        var h = db.PhanPhongs.Include(p => p.Phong).Include(p => p.DangKy).ThenInclude(d => d.SinhVien).AsQueryable();
        if (!string.IsNullOrWhiteSpace(maSv)) { maSv = maSv.Trim(); h = h.Where(p => p.DangKy.MaSinhVien.Contains(maSv)); }
        if (!string.IsNullOrWhiteSpace(hoTen)) { hoTen = hoTen.Trim(); h = h.Where(p => p.DangKy.SinhVien.HoTen.Contains(hoTen)); }
        if (!string.IsNullOrWhiteSpace(tenPhong)) { tenPhong = tenPhong.Trim(); h = h.Where(p => p.Phong.TenPhong.Contains(tenPhong)); }
        if (!string.IsNullOrEmpty(tt)) h = h.Where(p => p.TrangThai == tt);
        var pager = new PagerVM(h.Count(), page, ("maSv", maSv), ("hoTen", hoTen), ("tenPhong", tenPhong), ("tt", tt));
        var items = h.OrderByDescending(p => p.NgayVao).ThenByDescending(p => p.MaPhanPhong)
            .Skip(pager.Skip).Take(PagerVM.Size).ToList();
        return View(new LichSuVM { Items = items, Pager = pager, MaSinhVien = maSv, HoTen = hoTen, TenPhong = tenPhong, Tt = tt });
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

    public IActionResult Assign(int id)
    {
        var d = db.DangKys.Include(x => x.SinhVien).Include(x => x.LoaiPhongNV).FirstOrDefault(x => x.MaDangKy == id);
        if (d == null) return NotFound();
        if (d.TrangThai != TT.Cho) { TempData["Loi"] = $"Đăng ký #{d.MaDangKy} đang ở trạng thái \"{d.TrangThai}\", chỉ đăng ký Chờ duyệt mới được phân phòng."; return RedirectToAction(nameof(Index)); }
        if (d.SinhVien.TrangThai != TT.HoatDong) ViewBag.Loi = "Sinh viên đang bị khóa (không hoạt động), không thể phân phòng.";
        ViewBag.Phongs = PhongPhuHop(d.SinhVien.GioiTinh, null, d.NguyenVongLoaiPhong);
        ViewBag.NgayVao = (d.NgayBatDau.Date > DateTime.Today ? d.NgayBatDau.Date : DateTime.Today).ToString("yyyy-MM-dd");
        return View(d);
    }

    private IActionResult ThuLai(Func<IActionResult> viec)
    {
        const int toiDa = 3;
        for (int lan = 1; ; lan++)
        {
            try { return viec(); }
            catch (Exception ex) when (LoiCsdl.LaDeadlock(ex) || LoiCsdl.LaTrungKhoa(ex))
            {
                db.ChangeTracker.Clear();
                if (lan >= toiDa) return XungDot();
                Thread.Sleep(Random.Shared.Next(50, 150) * lan);
            }
        }
    }

    private IActionResult XungDot()
    {
        TempData["Loi"] = "Dữ liệu vừa được thay đổi bởi một thao tác khác (có thể do hai người cùng xử lý). Vui lòng kiểm tra lại danh sách rồi thực hiện lại.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Assign(int maDangKy, int maPhong, DateTime? ngayVao)
    {
        return ThuLai(() => AssignTrongTransaction(maDangKy, maPhong, ngayVao));
    }

    private IActionResult AssignTrongTransaction(int maDangKy, int maPhong, DateTime? ngayVao)
    {
        using var tx = db.Database.BeginTransaction(IsolationLevel.Serializable);
        var d = db.DangKys.Include(x => x.SinhVien).Include(x => x.LoaiPhongNV).FirstOrDefault(x => x.MaDangKy == maDangKy);
        if (d == null) return NotFound();
        if (d.TrangThai != TT.Cho) { TempData["Loi"] = "Đăng ký không ở trạng thái Chờ duyệt."; return RedirectToAction(nameof(Index)); }
        string loi = null;
        if (d.SinhVien.TrangThai != TT.HoatDong) loi = "Sinh viên đang bị khóa (không hoạt động), không thể phân phòng.";
        else if (db.PhanPhongs.Any(p => p.TrangThai == TT.Dang && p.DangKy.MaSinhVien == d.MaSinhVien)) loi = "Sinh viên đang ở một phòng khác.";
        else if (ngayVao == null) loi = "Vui lòng chọn ngày vào ở.";
        else if (ngayVao.Value.Date < DateTime.Today) loi = "Ngày vào ở không được ở quá khứ.";
        else if (ngayVao.Value.Date > d.NgayKetThucDuKien.Date) loi = "Ngày vào ở không được sau ngày kết thúc dự kiến.";
        else loi = KiemTraPhong(maPhong, d.SinhVien.GioiTinh);
        if (loi != null) { ViewBag.NgayVao = (ngayVao ?? DateTime.Today).ToString("yyyy-MM-dd"); ViewBag.Loi = loi; ViewBag.Phongs = PhongPhuHop(d.SinhVien.GioiTinh, null, d.NguyenVongLoaiPhong); return View(d); }
        db.PhanPhongs.Add(new PhanPhong { MaDangKy = d.MaDangKy, MaPhong = maPhong, NgayVao = ngayVao.Value.Date, TrangThai = TT.Dang });
        d.TrangThai = TT.DaPhan;
        db.SaveChanges(); tx.Commit();
        TempData["Ok"] = "Phân phòng thành công.";
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Transfer(int id)
    {
        var pp = db.PhanPhongs.Include(x => x.Phong).Include(x => x.DangKy).ThenInclude(d => d.SinhVien).FirstOrDefault(x => x.MaPhanPhong == id);
        if (pp == null) return NotFound();
        if (pp.TrangThai != TT.Dang) { TempData["Loi"] = "Sinh viên không còn ở phòng này."; return RedirectToAction(nameof(Index)); }
        ViewBag.Phongs = PhongPhuHop(pp.DangKy.SinhVien.GioiTinh, pp.MaPhong);
        return View(pp);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Transfer(int id, int maPhongMoi)
    {
        return ThuLai(() => TransferTrongTransaction(id, maPhongMoi));
    }

    private IActionResult TransferTrongTransaction(int id, int maPhongMoi)
    {
        using var tx = db.Database.BeginTransaction(IsolationLevel.Serializable);
        var pp = db.PhanPhongs.Include(x => x.Phong).Include(x => x.DangKy).ThenInclude(d => d.SinhVien).FirstOrDefault(x => x.MaPhanPhong == id);
        if (pp == null) return NotFound();
        string loi = null;
        if (pp.TrangThai != TT.Dang) loi = "Sinh viên không còn ở phòng này.";
        else if (pp.MaPhong == maPhongMoi) loi = "Không thể chuyển vào chính phòng đang ở.";
        else loi = KiemTraPhong(maPhongMoi, pp.DangKy.SinhVien.GioiTinh);
        if (loi != null) { ViewBag.Loi = loi; ViewBag.Phongs = PhongPhuHop(pp.DangKy.SinhVien.GioiTinh, pp.MaPhong); return View(pp); }
        var ngayChuyen = pp.NgayVao.Date > DateTime.Today ? pp.NgayVao.Date : DateTime.Today;
        pp.TrangThai = TT.DaChuyen; pp.NgayRa = ngayChuyen;
        db.SaveChanges();
        db.PhanPhongs.Add(new PhanPhong { MaDangKy = pp.MaDangKy, MaPhong = maPhongMoi, NgayVao = ngayChuyen, TrangThai = TT.Dang });
        db.SaveChanges(); tx.Commit();
        TempData["Ok"] = "Chuyển phòng thành công.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Return(int id)
    {
        try
        {
            using var tx = db.Database.BeginTransaction();
            var pp = db.PhanPhongs.AsNoTracking().Where(x => x.MaPhanPhong == id).Select(x => new { x.MaDangKy, x.NgayVao }).FirstOrDefault();
            if (pp == null) return NotFound();
            DateTime ngayRa = pp.NgayVao.Date > DateTime.Today ? pp.NgayVao.Date : DateTime.Today;
            int n = db.PhanPhongs.Where(x => x.MaPhanPhong == id && x.TrangThai == TT.Dang)
                .ExecuteUpdate(s => s.SetProperty(x => x.TrangThai, TT.DaTra).SetProperty(x => x.NgayRa, (DateTime?)ngayRa));
            if (n == 0) { TempData["Loi"] = "Sinh viên đã trả phòng hoặc không còn ở phòng này."; return RedirectToAction(nameof(Index)); }
            db.DangKys.Where(d => d.MaDangKy == pp.MaDangKy).ExecuteUpdate(s => s.SetProperty(d => d.TrangThai, TT.DaTra));
            tx.Commit();
        }
        catch (Exception ex) when (LoiCsdl.LaDeadlock(ex)) { return XungDot(); }
        TempData["Ok"] = "Trả phòng thành công.";
        return RedirectToAction(nameof(Index));
    }
}
