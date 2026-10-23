// Họ và tên: Phạm Quang Nhật
// Mã sinh viên: 23203100085
// Nội dung thực hiện: Quản lý sinh viên, đăng ký ở KTX, xem thông tin/lịch sử ở

namespace QuanLyKyTucXa_UNETI7_TI17A1ND.Models;

public class DangKyListVM
{
    public List<DangKyKTX> Items { get; set; } = new();
    public PagerVM Pager { get; set; }
    public string MaSinhVien { get; set; }
    public string HoTen { get; set; }
    public string Tt { get; set; }
}
