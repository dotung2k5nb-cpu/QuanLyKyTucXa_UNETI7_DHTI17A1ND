// Họ và tên: Nguyễn Anh Tú
// Mã sinh viên: 23203100027
// Nội dung thực hiện: Nhận diện lỗi trùng khóa duy nhất / deadlock từ SQL Server để báo lỗi thân thiện

namespace QuanLyKyTucXa_UNETI7_TI17A1ND.Data;

public static class LoiCsdl
{
    public static bool LaTrungKhoa(Exception ex) => Co(ex, 2601, 2627);
    public static bool LaDeadlock(Exception ex) => Co(ex, 1205);

    private static bool Co(Exception ex, params int[] maLoi)
    {
        for (var e = ex; e != null; e = e.InnerException)
            if (e is Microsoft.Data.SqlClient.SqlException s && maLoi.Contains(s.Number)) return true;
        return false;
    }
}
