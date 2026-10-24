// Họ và tên: Nguyễn Anh Tú
// Mã sinh viên: 23203100027
// Nội dung thực hiện: Phân phòng, chuyển phòng, trả phòng, Dashboard, thống kê

namespace QuanLyKyTucXa_UNETI7_TI17A1ND.Models;

public class LichSuVM
{
    public List<PhanPhong> Items { get; set; } = new();
    public PagerVM Pager { get; set; }
    public string MaSinhVien { get; set; }
    public string HoTen { get; set; }
    public string TenPhong { get; set; }
    public string Tt { get; set; }
}
