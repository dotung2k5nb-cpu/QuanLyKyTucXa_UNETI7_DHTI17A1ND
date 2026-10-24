// Họ và tên: Nguyễn Anh Tú
// Mã sinh viên: 23203100027
// Nội dung thực hiện: Phân phòng, chuyển phòng, trả phòng, Dashboard, thống kê

using Microsoft.AspNetCore.Mvc;
using QuanLyKyTucXa_UNETI7_TI17A1ND.Data;
using QuanLyKyTucXa_UNETI7_TI17A1ND.Filters;
using QuanLyKyTucXa_UNETI7_TI17A1ND.Models;
namespace QuanLyKyTucXa_UNETI7_TI17A1ND.Controllers;

[Quyen(TT.Admin)]
public class ThongKeController : Controller
{
    private readonly AppDb db;
    public ThongKeController(AppDb db) { this.db = db; }

    public IActionResult Index()
    {
        var dang = db.PhanPhongs.Where(x => x.TrangThai == TT.Dang);
        int tongCho = db.Phongs.Where(p => p.TrangThai == TT.PhongDung).Sum(p => (int?)p.LoaiPhong.SoNguoiToiDa) ?? 0;
        int dung = dang.Count();
        var rows = db.Phongs.Select(p => new PhongRow { P = p, TenLoai = p.LoaiPhong.TenLoaiPhong, SucChua = p.LoaiPhong.SoNguoiToiDa, DangO = p.PhanPhongs.Count(x => x.TrangThai == TT.Dang) })
            .OrderBy(r => r.P.TenPhong).ToList();
        ViewBag.TongPhong = db.Phongs.Count();
        ViewBag.TongSV = db.SinhViens.Count();
        ViewBag.TongDK = db.DangKys.Count();
        ViewBag.ChoDuyet = db.DangKys.Count(d => d.TrangThai == TT.Cho);
        ViewBag.SVDangO = dang.Select(x => x.DangKy.MaSinhVien).Distinct().Count();
        ViewBag.TongCho = tongCho; ViewBag.Dung = dung; ViewBag.Con = tongCho - dung;
        ViewBag.PhongDay = rows.Count(r => r.ConCho <= 0);
        ViewBag.TyLe = tongCho == 0 ? 0 : Math.Round(dung * 100.0 / tongCho, 1);
        ViewBag.PhongTheoLoai = db.Phongs.GroupBy(p => p.LoaiPhong.TenLoaiPhong).Select(g => new KV { K = g.Key, V = g.Count() }).ToList();
        ViewBag.SvTheoToa = dang.GroupBy(x => x.Phong.ToaNha).Select(g => new KV { K = g.Key, V = g.Count() }).OrderBy(k => k.K).ToList();
        ViewBag.SvTheoLoai = dang.GroupBy(x => x.Phong.LoaiPhong.TenLoaiPhong).Select(g => new KV { K = g.Key, V = g.Count() }).ToList();
        ViewBag.DkTheoTT = db.DangKys.GroupBy(d => d.TrangThai).Select(g => new KV { K = g.Key, V = g.Count() }).ToList();
        var ngayVaoDauTien = db.PhanPhongs.GroupBy(x => x.MaDangKy).Select(g => g.Min(x => x.NgayVao)).ToList();
        ViewBag.TheoThang = ngayVaoDauTien.GroupBy(d => new { d.Year, d.Month }).OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
            .Select(g => new KV { K = $"{g.Key.Month:00}/{g.Key.Year}", V = g.Count() }).ToList();
        ViewBag.NhieuNhat = rows.OrderByDescending(r => r.DangO).FirstOrDefault();
        return View(rows);
    }
}
