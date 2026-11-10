using Microsoft.Data.SqlClient;
using WinFormsApp11.bai6._3.DTO;

namespace WinFormsApp11.bai6._3.DAL;

public sealed class ProjectDal
{
    public List<ProjectDto> GetAll()
    {
        const string sql = "SELECT MaDuAn, TenDuAn, NgayBatDau, NgayKetThuc, TrangThai FROM DuAn ORDER BY MaDuAn DESC";
        using var connection = Db.OpenConnection();
        using var command = new SqlCommand(sql, connection);
        using var reader = command.ExecuteReader();
        var result = new List<ProjectDto>();
        while (reader.Read())
        {
            result.Add(new ProjectDto
            {
                MaDuAn = reader.GetInt32(0), TenDuAn = reader.GetString(1),
                NgayBatDau = reader.IsDBNull(2) ? null : reader.GetDateTime(2),
                NgayKetThuc = reader.IsDBNull(3) ? null : reader.GetDateTime(3),
                TrangThai = reader.GetString(4)
            });
        }
        return result;
    }

    public int Insert(ProjectDto project)
    {
        const string sql = "INSERT INTO DuAn (TenDuAn, NgayBatDau, NgayKetThuc, TrangThai) OUTPUT INSERTED.MaDuAn VALUES (@name, @start, @end, @status)";
        using var connection = Db.OpenConnection();
        using var command = new SqlCommand(sql, connection);
        AddParameters(command, project);
        return (int)command.ExecuteScalar()!;
    }

    public void Update(ProjectDto project)
    {
        const string sql = "UPDATE DuAn SET TenDuAn=@name, NgayBatDau=@start, NgayKetThuc=@end, TrangThai=@status WHERE MaDuAn=@id";
        using var connection = Db.OpenConnection();
        using var command = new SqlCommand(sql, connection);
        AddParameters(command, project);
        command.Parameters.AddWithValue("@id", project.MaDuAn);
        command.ExecuteNonQuery();
    }

    public void Delete(int projectId)
    {
        using var connection = Db.OpenConnection();
        using var command = new SqlCommand("DELETE FROM DuAn WHERE MaDuAn=@id", connection);
        command.Parameters.AddWithValue("@id", projectId);
        command.ExecuteNonQuery();
    }

    private static void AddParameters(SqlCommand command, ProjectDto project)
    {
        command.Parameters.AddWithValue("@name", project.TenDuAn);
        command.Parameters.AddWithValue("@start", (object?)project.NgayBatDau ?? DBNull.Value);
        command.Parameters.AddWithValue("@end", (object?)project.NgayKetThuc ?? DBNull.Value);
        command.Parameters.AddWithValue("@status", project.TrangThai);
    }
}
