// Họ và tên: Phạm Quang Nhật
// Mã sinh viên: 23203100085
// Nội dung thực hiện: Quản lý sinh viên, đăng ký ở KTX, xem thông tin/lịch sử ở

using System.ComponentModel.DataAnnotations;
namespace QuanLyKyTucXa_UNETI7_TI17A1ND.Models;

public class DangKyKTX
{
    [Key] public int MaDangKy { get; set; }
    [Required, StringLength(20)] public string MaSinhVien { get; set; }
    public SinhVien SinhVien { get; set; }
    public DateTime NgayDangKy { get; set; }
    [DataType(DataType.Date), Display(Name = "Ngày bắt đầu")] public DateTime NgayBatDau { get; set; } = DateTime.Today;
    [DataType(DataType.Date), Display(Name = "Ngày kết thúc dự kiến")] public DateTime NgayKetThucDuKien { get; set; } = DateTime.Today.AddMonths(6);
    [Display(Name = "Nguyện vọng loại phòng")]
    [Required]
    public int NguyenVongLoaiPhong { get; set; }
    public LoaiPhong LoaiPhongNV { get; set; }
    [StringLength(500), Display(Name = "Ghi chú")] public string GhiChu { get; set; }
    [Required, StringLength(20)] public string TrangThai { get; set; } = TT.Cho;
    public List<PhanPhong> PhanPhongs { get; set; } = new();
}
