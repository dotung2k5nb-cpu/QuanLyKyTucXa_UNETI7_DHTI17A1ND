// Họ và tên: Đỗ Văn Tùng
// Mã sinh viên: 23203100028
// Nội dung thực hiện: Tài khoản, đăng nhập, phân quyền, quản lý loại phòng

using Microsoft.AspNetCore.Mvc;
using QuanLyKyTucXa_UNETI7_TI17A1ND.Data;
using QuanLyKyTucXa_UNETI7_TI17A1ND.Filters;
using QuanLyKyTucXa_UNETI7_TI17A1ND.Models;
namespace QuanLyKyTucXa_UNETI7_TI17A1ND.Controllers;

[Quyen(TT.Admin)]
public class LoaiPhongController : Controller
{
    private readonly AppDb db;
    public LoaiPhongController(AppDb db) { this.db = db; }

    public IActionResult Index() => View(db.LoaiPhongs.OrderBy(x => x.TenLoaiPhong).ToList());
    public IActionResult Create() => View("Form", new LoaiPhong());

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Create([Bind("TenLoaiPhong,SoNguoiToiDa,DonGiaThang,MoTa,TrangThai")] LoaiPhong m)
    {
        if (db.LoaiPhongs.Any(x => x.TenLoaiPhong == m.TenLoaiPhong)) ModelState.AddModelError("TenLoaiPhong", "Tên loại phòng đã tồn tại");
        if (!ModelState.IsValid) return View("Form", m);
        db.LoaiPhongs.Add(m); db.SaveChanges(); return RedirectToAction(nameof(Index));
    }

    public IActionResult Edit(int id)
    {
        var m = db.LoaiPhongs.Find(id);
        return m == null ? NotFound() : View("Form", m);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Edit(int id, [Bind("MaLoaiPhong,TenLoaiPhong,SoNguoiToiDa,DonGiaThang,MoTa,TrangThai")] LoaiPhong m)
    {
        if (id != m.MaLoaiPhong) return NotFound();
        if (db.LoaiPhongs.Any(x => x.TenLoaiPhong == m.TenLoaiPhong && x.MaLoaiPhong != id)) ModelState.AddModelError("TenLoaiPhong", "Tên loại phòng đã tồn tại");
        int maxDangO = db.Phongs.Where(p => p.MaLoaiPhong == id)
            .Select(p => p.PhanPhongs.Count(x => x.TrangThai == TT.Dang)).Max(x => (int?)x) ?? 0;
        if (m.SoNguoiToiDa < maxDangO)
            ModelState.AddModelError("SoNguoiToiDa", $"Không thể giảm xuống {m.SoNguoiToiDa}: có phòng thuộc loại này đang có {maxDangO} sinh viên ở.");
        if (!ModelState.IsValid) return View("Form", m);
        db.Update(m); db.SaveChanges(); return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Delete(int id)
    {
        var m = db.LoaiPhongs.Find(id);
        if (m == null) return NotFound();
        if (db.Phongs.Any(p => p.MaLoaiPhong == id)) { TempData["Loi"] = "Không thể xóa: loại phòng đang có phòng sử dụng. Hãy chuyển sang không hoạt động."; return RedirectToAction(nameof(Index)); }
        if (db.DangKys.Any(d => d.NguyenVongLoaiPhong == id)) { TempData["Loi"] = "Không thể xóa: loại phòng đã được dùng làm nguyện vọng trong đăng ký KTX. Hãy chuyển sang không hoạt động."; return RedirectToAction(nameof(Index)); }
        db.LoaiPhongs.Remove(m); db.SaveChanges(); return RedirectToAction(nameof(Index));
    }
}
