namespace WinFormsApp11.bai6._3.DTO;

public sealed class MemberProgressDto
{
    public string HoTen { get; set; } = string.Empty;
    public int TotalTasks { get; set; }
    public int CompletedTasks { get; set; }
    public decimal CompletionRate { get; set; }
}
