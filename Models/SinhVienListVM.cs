namespace QuanLyKyTucXa_UNETI7_TI17A1ND.Models;

public class SinhVienListVM
{
    public List<SinhVien> Items { get; set; } = new();
    public string MaSinhVien { get; set; }
    public string HoTen { get; set; }
    public string Lop { get; set; }
    public string Khoa { get; set; }
    public string GioiTinh { get; set; }
    public string Tt { get; set; }
}
