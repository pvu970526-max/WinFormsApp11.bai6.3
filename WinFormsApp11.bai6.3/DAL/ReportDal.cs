using Microsoft.Data.SqlClient;
using WinFormsApp11.bai6._3.DTO;

namespace WinFormsApp11.bai6._3.DAL;

public sealed class ReportDal
{
    public List<MemberProgressDto> GetMemberProgress(int projectId)
    {
        const string sql = "SELECT tv.HoTen, COUNT(pc.MaTask) AS TotalTasks, SUM(CASE WHEN c.TrangThai=N'Done' THEN 1 ELSE 0 END) AS CompletedTasks, CAST(CASE WHEN COUNT(pc.MaTask)=0 THEN 0 ELSE SUM(CASE WHEN c.TrangThai=N'Done' THEN 1 ELSE 0 END) * 100.0 / COUNT(pc.MaTask) END AS decimal(5,2)) AS CompletionRate FROM ThanhVien tv INNER JOIN PhanCong pc ON pc.MaThanhVien=tv.MaThanhVien INNER JOIN CongViec c ON c.MaTask=pc.MaTask WHERE c.MaDuAn=@projectId GROUP BY tv.MaThanhVien, tv.HoTen ORDER BY tv.HoTen";
        using var connection = Db.OpenConnection();
        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@projectId", projectId);
        using var reader = command.ExecuteReader();
        var result = new List<MemberProgressDto>();
        while (reader.Read())
        {
            result.Add(new MemberProgressDto { HoTen = reader.GetString(0), TotalTasks = reader.GetInt32(1), CompletedTasks = reader.GetInt32(2), CompletionRate = reader.GetDecimal(3) });
        }
        return result;
    }
}
