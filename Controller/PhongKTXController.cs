// Họ và tên: Hoàng Tiến Nam
// Mã sinh viên: 23203100029
// Nội dung thực hiện: Quản lý phòng ký túc xá, tìm kiếm, lọc, sắp xếp và phân trang

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyKyTucXa_UNETI7_TI17A1ND.Data;
using QuanLyKyTucXa_UNETI7_TI17A1ND.Filters;
using QuanLyKyTucXa_UNETI7_TI17A1ND.Models;
namespace QuanLyKyTucXa_UNETI7_TI17A1ND.Controllers;

[Quyen(TT.Admin)]
public class PhongKTXController : Controller
{
    private readonly AppDb db;
    private const int PageSize = 8;
    public PhongKTXController(AppDb db) { this.db = db; }

    public async Task<IActionResult> Index(string q, string toa, int? loai, int? tang, string gt, string tt, string cho, string sort, int page = 1)
    {
        if (HttpContext.Session.GetString("VaiTro") == TT.SinhVien)
{
    tt = TT.PhongDung;
    cho = "con";
    gt = HttpContext.Session.GetString("GioiTinh"); // Lấy giới tính từ session
}
        var query = db.Phongs.AsNoTracking();

if (HttpContext.Session.GetString("VaiTro") == TT.SinhVien)
{
    query = query.Where(p => p.TrangThai == TT.PhongDung);
}

var rows = query.Select(p => new PhongRow
{
    P = p, 
    TenLoai = p.LoaiPhong.TenLoaiPhong, 
    SucChua = p.LoaiPhong.SoNguoiToiDa,
    DangO = p.PhanPhongs.Count(x => x.TrangThai == TT.Dang)
});
        if (!string.IsNullOrWhiteSpace(q))
{
    q = q.Trim();
    rows = rows.Where(r => r.P.TenPhong.Contains(q));
}
        if (!string.IsNullOrWhiteSpace(toa)) { toa = toa.Trim(); rows = rows.Where(r => r.P.ToaNha.Contains(toa)); }
        if (loai != null) rows = rows.Where(r => r.P.MaLoaiPhong == loai);
        if (tang != null) rows = rows.Where(r => r.P.Tang == tang);
        if (!string.IsNullOrEmpty(gt)) rows = rows.Where(r => r.P.GioiTinhPhong == gt);
        if (!string.IsNullOrEmpty(tt)) rows = rows.Where(r => r.P.TrangThai == tt);
        if (cho == "con") rows = rows.Where(r => r.SucChua - r.DangO > 0);
        else if (cho == "het") rows = rows.Where(r => r.SucChua - r.DangO <= 0);
        rows = sort switch
        {
            "ten_desc" => rows.OrderByDescending(r => r.P.TenPhong),
            "tang_asc" => rows.OrderBy(r => r.P.Tang).ThenBy(r => r.P.TenPhong),
            "tang_desc" => rows.OrderByDescending(r => r.P.Tang).ThenBy(r => r.P.TenPhong),
            "cho_asc" => rows.OrderBy(r => r.SucChua - r.DangO).ThenBy(r => r.P.TenPhong),
            "cho_desc" => rows.OrderByDescending(r => r.SucChua - r.DangO).ThenBy(r => r.P.TenPhong),
            _ => rows.OrderBy(r => r.P.TenPhong)
        };
        int total = await rows.CountAsync();
        int pages = Math.Max(1, (int)Math.Ceiling(total / (double)PageSize));
        page = Math.Clamp(page, 1, pages);
        var items = await rows.Skip((page - 1) * PageSize).Take(PageSize).ToListAsync();
var loais = await db.LoaiPhongs.OrderBy(l => l.TenLoaiPhong).ToListAsync();

var vm = new PhongListVM
{
    Items = items,
            Page = page, Pages = pages, Total = total,
            Q = q, Toa = toa, Loai = loai, Tang = tang, Gt = gt, Tt = tt, Cho = cho, Sort = sort,
            Loais = db.LoaiPhongs.OrderBy(l => l.TenLoaiPhong).ToList()
        };
        return View(vm);
    }

    private void Nap(PhongKTX m) => ViewBag.Loais = new SelectList(
        db.LoaiPhongs.Where(l => l.TrangThai == TT.HoatDong || l.MaLoaiPhong == m.MaLoaiPhong)
            .OrderBy(l => l.TenLoaiPhong)
            .Select(l => new { l.MaLoaiPhong, Ten = l.TrangThai == TT.HoatDong ? l.TenLoaiPhong : l.TenLoaiPhong + " (ngừng hoạt động)" }),
        "MaLoaiPhong", "Ten", m.MaLoaiPhong);

    [Quyen(TT.Admin)] public IActionResult Create() { var m = new PhongKTX(); Nap(m); return View("Form", m); }

    [Quyen(TT.Admin), HttpPost, ValidateAntiForgeryToken]
    public IActionResult Create([Bind("TenPhong,MaLoaiPhong,ToaNha,Tang,GioiTinhPhong,TrangThai,MoTa")] PhongKTX m)
    {
        if (db.Phongs.Any(x => x.TenPhong == m.TenPhong)) ModelState.AddModelError("TenPhong", "Tên phòng đã tồn tại");
        if (!db.LoaiPhongs.Any(l => l.MaLoaiPhong == m.MaLoaiPhong && l.TrangThai == TT.HoatDong)) ModelState.AddModelError("MaLoaiPhong", "Loại phòng không tồn tại hoặc đã ngừng hoạt động");
        if (!ModelState.IsValid) { Nap(m); return View("Form", m); }
        db.Phongs.Add(m); db.SaveChanges(); return RedirectToAction(nameof(Index));
    }

    [Quyen(TT.Admin)]
    public IActionResult Edit(int id)
    {
        var m = db.Phongs.Find(id); if (m == null) return NotFound();
        Nap(m); return View("Form", m);
    }

    [Quyen(TT.Admin), HttpPost, ValidateAntiForgeryToken]
    public IActionResult Edit(int id, [Bind("MaPhong,TenPhong,MaLoaiPhong,ToaNha,Tang,GioiTinhPhong,TrangThai,MoTa")] PhongKTX m)
    {
        if (id != m.MaPhong) return NotFound();
        if (db.Phongs.Any(x => x.TenPhong == m.TenPhong && x.MaPhong != id)) ModelState.AddModelError("TenPhong", "Tên phòng đã tồn tại");
        int dangO = db.PhanPhongs.Count(x => x.MaPhong == id && x.TrangThai == TT.Dang);
        var sucChuaMoi = db.LoaiPhongs.Where(l => l.MaLoaiPhong == m.MaLoaiPhong).Select(l => (int?)l.SoNguoiToiDa).FirstOrDefault();
        int? loaiCu = db.Phongs.AsNoTracking().Where(x => x.MaPhong == id).Select(x => (int?)x.MaLoaiPhong).FirstOrDefault();
        if (loaiCu == null) return NotFound();
        if (m.MaLoaiPhong != loaiCu && !db.LoaiPhongs.Any(l => l.MaLoaiPhong == m.MaLoaiPhong && l.TrangThai == TT.HoatDong))
            ModelState.AddModelError("MaLoaiPhong", "Loại phòng không tồn tại hoặc đã ngừng hoạt động");
        else if (sucChuaMoi == null) ModelState.AddModelError("MaLoaiPhong", "Loại phòng không tồn tại");
        else if (dangO > sucChuaMoi) ModelState.AddModelError("MaLoaiPhong", $"Không thể đổi sang loại phòng này: phòng đang có {dangO} sinh viên, vượt sức chứa {sucChuaMoi}.");
        if (dangO > 0 && db.PhanPhongs.Any(x => x.MaPhong == id && x.TrangThai == TT.Dang && x.DangKy.SinhVien.GioiTinh != m.GioiTinhPhong))
            ModelState.AddModelError("GioiTinhPhong", "Không thể đổi giới tính phòng: phòng đang có sinh viên giới tính khác ở.");
        if (m.TrangThai == TT.PhongTam && dangO > 0)
            ModelState.AddModelError("TrangThai", $"Không thể chuyển sang Tạm ngừng: phòng đang có {dangO} sinh viên ở. Hãy chuyển phòng hoặc trả phòng cho các sinh viên này trước.");
        if (!ModelState.IsValid) { Nap(m); return View("Form", m); }
        db.Update(m); db.SaveChanges(); return RedirectToAction(nameof(Index));
    }

    [Quyen(TT.Admin)]
    public async Task<IActionResult> Details(int id)
    {
        var m = await db.Phongs
            .Include(p => p.LoaiPhong)
            .Include(p => p.PhanPhongs.Where(x => x.TrangThai == TT.Dang))
                .ThenInclude(x => x.DangKy)
                .ThenInclude(d => d.SinhVien)
            .FirstOrDefaultAsync(p => p.MaPhong == id);
        return m == null ? NotFound() : View(m);
    }

    [Quyen(TT.Admin), HttpPost, ValidateAntiForgeryToken]
    public IActionResult Delete(int id)
    {
        var m = db.Phongs.Find(id); if (m == null) return NotFound();
        if (db.PhanPhongs.Any(x => x.MaPhong == id && x.TrangThai == TT.Dang) || db.PhanPhongs.Any(pp => pp.MaPhong == id && pp.DangKy.TrangThai == TT.DaPhan))
{
    TempData["Loi"] = "Phòng đã có sinh viên ở hoặc đã được đăng ký, không thể xóa. Hãy chuyển sang Tạm ngừng.";
    return RedirectToAction(nameof(Index));
}
        db.Phongs.Remove(m); db.SaveChanges(); return RedirectToAction(nameof(Index));
    }
}
