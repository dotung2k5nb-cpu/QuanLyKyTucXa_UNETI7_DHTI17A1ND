// Họ và tên: Phạm Quang Nhật
// Mã sinh viên: 23203100085
// Nội dung thực hiện: Phân trang dùng chung cho danh sách đăng ký KTX
//
// Họ và tên: Nguyễn Anh Tú
// Mã sinh viên: 23203100027
// Nội dung thực hiện: Phân trang dùng chung cho lịch sử phân phòng

namespace QuanLyKyTucXa_UNETI7_TI17A1ND.Models;

public class PagerVM
{
    public const int Size = 10;
    public int Page { get; }
    public int Pages { get; }
    public int Total { get; }
    private readonly Dictionary<string, string> query = new();

    public PagerVM(int total, int page, params (string Key, string Value)[] conditions)
    {
        Total = total;
        Pages = Math.Max(1, (int)Math.Ceiling(total / (double)Size));
        Page = Math.Clamp(page, 1, Pages);
        foreach (var (k, v) in conditions) if (!string.IsNullOrEmpty(v)) query[k] = v;
    }

    public int Skip => (Page - 1) * Size;

    public Dictionary<string, string> Route(int page)
    {
        var d = new Dictionary<string, string>(query) { ["page"] = page.ToString() };
        return d;
    }
}
