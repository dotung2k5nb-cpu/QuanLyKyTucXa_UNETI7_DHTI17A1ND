// Họ và tên: Phạm Quang Nhật
// Mã sinh viên: 23203100085
// Nội dung thực hiện: Quản lý sinh viên, đăng ký ở KTX, xem thông tin/lịch sử ở

using System.ComponentModel.DataAnnotations;
namespace QuanLyKyTucXa_UNETI7_TI17A1ND.Models;

public class SinhVien
{
    [Key, StringLength(20), Required(ErrorMessage = "Nhập mã sinh viên"), Display(Name = "Mã sinh viên")] public string MaSinhVien { get; set; }
    public int MaTaiKhoan { get; set; }
    public TaiKhoan TaiKhoan { get; set; }
    [Required(ErrorMessage = "Nhập họ tên"), StringLength(100), Display(Name = "Họ tên")] public string HoTen { get; set; }
    [DataType(DataType.Date), Display(Name = "Ngày sinh")] public DateTime NgaySinh { get; set; } = new DateTime(2004, 1, 1);
    [Required, StringLength(10), AllowedValues(TT.Nam, TT.Nu, ErrorMessage = "{0} không hợp lệ"), Display(Name = "Giới tính")] public string GioiTinh { get; set; } = TT.Nam;
    [StringLength(50)] public string Lop { get; set; }
    [StringLength(100)] public string Khoa { get; set; }
    [RegularExpression(@"^(0|\+84)\d{9}$", ErrorMessage = "SĐT không hợp lệ (10 số, bắt đầu 0 hoặc +84)"), StringLength(15), Display(Name = "Số điện thoại")] public string SoDienThoai { get; set; }
    [EmailAddress(ErrorMessage = "Email không đúng định dạng"), StringLength(100)] public string Email { get; set; }
    [StringLength(200), Display(Name = "Địa chỉ")] public string DiaChi { get; set; }
    [Required, StringLength(20), AllowedValues(TT.HoatDong, TT.Khoa, ErrorMessage = "{0} không hợp lệ"), Display(Name = "Trạng thái")] public string TrangThai { get; set; } = TT.HoatDong;
    public List<DangKyKTX> DangKys { get; set; } = new();
}
