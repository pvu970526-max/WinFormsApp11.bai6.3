namespace WinFormsApp11.bai6._3.DTO;

public sealed class MemberDto
{
    public int MaThanhVien { get; set; }
    public string HoTen { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? ChuyenMon { get; set; }

    public override string ToString() => $"{HoTen} ({ChuyenMon ?? "Chưa cập nhật"})";
}
