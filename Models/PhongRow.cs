// Họ và tên: Hoàng Tiến Nam
// Mã sinh viên: 23203100029
// Nội dung thực hiện: Quản lý phòng ký túc xá, tìm kiếm, lọc, sắp xếp và phân trang

namespace QuanLyKyTucXa_UNETI7_TI17A1ND.Models;

public class PhongRow { public PhongKTX P { get; set; } public string TenLoai { get; set; } public int SucChua { get; set; } public int DangO { get; set; } public int ConCho => SucChua - DangO; }
