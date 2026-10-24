// Họ và tên: Nguyễn Anh Tú
// Mã sinh viên: 23203100027
// Nội dung thực hiện: Phân phòng, chuyển phòng, trả phòng, Dashboard, thống kê

using System.ComponentModel.DataAnnotations;
namespace QuanLyKyTucXa_UNETI7_TI17A1ND.Models;

public class PhanPhong
{
    [Key] public int MaPhanPhong { get; set; }
    public int MaDangKy { get; set; }
    public DangKyKTX DangKy { get; set; }
    public int MaPhong { get; set; }
    public PhongKTX Phong { get; set; }
    public DateTime NgayVao { get; set; }
    public DateTime? NgayRa { get; set; }
    [Required, StringLength(20)] public string TrangThai { get; set; } = TT.Dang;
}
