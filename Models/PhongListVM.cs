// Họ và tên: Hoàng Tiến Nam
// Mã sinh viên: 23203100029
// Nội dung thực hiện: Quản lý phòng ký túc xá, tìm kiếm, lọc, sắp xếp và phân trang

namespace QuanLyKyTucXa_UNETI7_TI17A1ND.Models;

public class PhongListVM
{
    public List<PhongRow> Items { get; set; } = new();
    public int Page { get; set; } public int Pages { get; set; } public int Total { get; set; }
    public string Q, Toa, Gt, Tt, Cho, Sort; public int? Loai, Tang;
    public List<LoaiPhong> Loais { get; set; } = new();
    public Dictionary<string, string> Route(int page) => new()
    { ["q"] = Q, ["toa"] = Toa, ["gt"] = Gt, ["tt"] = Tt, ["cho"] = Cho, ["sort"] = Sort, ["loai"] = Loai?.ToString(), ["tang"] = Tang?.ToString(), ["page"] = page.ToString() };
}
