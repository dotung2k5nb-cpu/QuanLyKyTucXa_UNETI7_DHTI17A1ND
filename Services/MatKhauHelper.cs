// Họ và tên: Đỗ Văn Tùng
// Mã sinh viên: 23203100028
// Nội dung thực hiện: Băm và kiểm tra mật khẩu bằng PBKDF2 có salt, tạo mật khẩu tạm thời

using System.Security.Cryptography;
using System.Text;
namespace QuanLyKyTucXa_UNETI7_TI17A1ND.Services;

public static class MatKhauHelper
{
    private const string Tien = "PBKDF2";
    private const int SoVong = 100_000, DoDaiSalt = 16, DoDaiHash = 32;
    private const string KyTuMatKhauTam = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz23456789";

    private static readonly string BamGia = Bam("mat-khau-gia-de-can-bang-thoi-gian-xu-ly");

    public static string Bam(string matKhau)
    {
        var salt = RandomNumberGenerator.GetBytes(DoDaiSalt);
        var hash = Rfc2898DeriveBytes.Pbkdf2(matKhau, salt, SoVong, HashAlgorithmName.SHA256, DoDaiHash);
        return $"{Tien}${SoVong}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
    }

    public static bool Kiem(string matKhau, string daLuu)
    {
        if (string.IsNullOrEmpty(matKhau)) return false;
        if (string.IsNullOrEmpty(daLuu)) { Kiem(matKhau, BamGia); return false; }

        var p = daLuu.Split('$');
        if (p.Length == 4 && p[0] == Tien)
        {
            if (!int.TryParse(p[1], out var vong) || vong < 1) return false;
            try
            {
                var salt = Convert.FromBase64String(p[2]);
                var hashLuu = Convert.FromBase64String(p[3]);
                if (hashLuu.Length == 0) return false;
                var hash = Rfc2898DeriveBytes.Pbkdf2(matKhau, salt, vong, HashAlgorithmName.SHA256, hashLuu.Length);
                return CryptographicOperations.FixedTimeEquals(hash, hashLuu);
            }
            catch (FormatException) { return false; }
        }

        var cu = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(matKhau)));
        return CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(cu), Encoding.UTF8.GetBytes(daLuu));
    }

    public static bool CanBamLai(string daLuu)
    {
        var p = daLuu?.Split('$');
        return p == null || p.Length != 4 || p[0] != Tien || !int.TryParse(p[1], out var vong) || vong < SoVong;
    }

    public static string TaoMatKhauTam(int doDai = 10)
    {
        var s = new char[doDai];
        for (int i = 0; i < doDai; i++) s[i] = KyTuMatKhauTam[RandomNumberGenerator.GetInt32(KyTuMatKhauTam.Length)];
        return new string(s);
    }
}
