// Họ và tên: Phạm Quang Nhật
// Mã sinh viên: 23203100085
// Nội dung thực hiện: Quản lý sinh viên, đăng ký ở KTX, xem thông tin/lịch sử ở

namespace QuanLyKyTucXa_UNETI7_TI17A1ND.Models;

public class SinhVienLichSuVM
{
    public SinhVien SinhVien { get; set; }
    public List<DangKyKTX> DangKys { get; set; } = new();
    public List<PhanPhong> PhanPhongs { get; set; } = new();
    public int SoLanChuyen { get; set; }
    public int SoPhongDaO { get; set; }
}
