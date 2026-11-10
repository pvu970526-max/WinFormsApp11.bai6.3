namespace WinFormsApp11.bai6._3.DTO;

public sealed class ProjectDto
{
    public int MaDuAn { get; set; }
    public string TenDuAn { get; set; } = string.Empty;
    public DateTime? NgayBatDau { get; set; }
    public DateTime? NgayKetThuc { get; set; }
    public string TrangThai { get; set; } = "Đang chạy";

    public override string ToString() => $"{MaDuAn} - {TenDuAn}";
}
