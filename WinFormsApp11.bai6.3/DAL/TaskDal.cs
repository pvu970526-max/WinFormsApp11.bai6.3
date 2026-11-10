using Microsoft.Data.SqlClient;
using WinFormsApp11.bai6._3.DTO;

namespace WinFormsApp11.bai6._3.DAL;

public sealed class TaskDal
{
    public List<TaskDto> GetByProject(int projectId, string? status = null)
    {
        const string sql = "SELECT c.MaTask, c.MaDuAn, c.TenTask, c.MoTa, c.HanNop, c.DoUuTien, c.TrangThai, COALESCE(STRING_AGG(tv.HoTen, N', '), N'') AS AssignedMembers FROM CongViec c LEFT JOIN PhanCong pc ON pc.MaTask=c.MaTask LEFT JOIN ThanhVien tv ON tv.MaThanhVien=pc.MaThanhVien WHERE c.MaDuAn=@projectId AND (@status IS NULL OR c.TrangThai=@status) GROUP BY c.MaTask, c.MaDuAn, c.TenTask, c.MoTa, c.HanNop, c.DoUuTien, c.TrangThai ORDER BY c.HanNop";
        using var connection = Db.OpenConnection();
        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@projectId", projectId);
        command.Parameters.AddWithValue("@status", (object?)status ?? DBNull.Value);
        return ReadTasks(command);
    }

    public List<TaskDto> GetOverdue()
    {
        const string sql = "SELECT c.MaTask, c.MaDuAn, c.TenTask, c.MoTa, c.HanNop, c.DoUuTien, c.TrangThai, COALESCE(STRING_AGG(tv.HoTen, N', '), N'') FROM CongViec c LEFT JOIN PhanCong pc ON pc.MaTask=c.MaTask LEFT JOIN ThanhVien tv ON tv.MaThanhVien=pc.MaThanhVien WHERE c.HanNop < GETDATE() AND c.TrangThai <> N'Done' GROUP BY c.MaTask, c.MaDuAn, c.TenTask, c.MoTa, c.HanNop, c.DoUuTien, c.TrangThai ORDER BY c.HanNop";
        using var connection = Db.OpenConnection();
        using var command = new SqlCommand(sql, connection);
        return ReadTasks(command);
    }

    public int Insert(TaskDto task)
    {
        const string sql = "INSERT INTO CongViec (MaDuAn, TenTask, MoTa, HanNop, DoUuTien, TrangThai) OUTPUT INSERTED.MaTask VALUES (@project, @name, @description, @due, @priority, @status)";
        using var connection = Db.OpenConnection();
        using var command = new SqlCommand(sql, connection);
        AddParameters(command, task);
        return (int)command.ExecuteScalar()!;
    }

    public void UpdateStatus(int taskId, string status)
    {
        using var connection = Db.OpenConnection();
        using var command = new SqlCommand("UPDATE CongViec SET TrangThai=@status WHERE MaTask=@id", connection);
        command.Parameters.AddWithValue("@status", status);
        command.Parameters.AddWithValue("@id", taskId);
        command.ExecuteNonQuery();
    }

    public void Assign(int taskId, int memberId)
    {
        const string sql = "IF NOT EXISTS (SELECT 1 FROM PhanCong WHERE MaTask=@task AND MaThanhVien=@member) INSERT INTO PhanCong (MaTask, MaThanhVien) VALUES (@task, @member)";
        using var connection = Db.OpenConnection();
        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@task", taskId);
        command.Parameters.AddWithValue("@member", memberId);
        command.ExecuteNonQuery();
    }

    private static List<TaskDto> ReadTasks(SqlCommand command)
    {
        using var reader = command.ExecuteReader();
        var result = new List<TaskDto>();
        while (reader.Read())
        {
            result.Add(new TaskDto
            {
                MaTask = reader.GetInt32(0), MaDuAn = reader.GetInt32(1), TenTask = reader.GetString(2),
                MoTa = reader.IsDBNull(3) ? null : reader.GetString(3),
                HanNop = reader.IsDBNull(4) ? null : reader.GetDateTime(4), DoUuTien = reader.GetString(5),
                TrangThai = reader.GetString(6), AssignedMembers = reader.IsDBNull(7) ? string.Empty : reader.GetString(7)
            });
        }
        return result;
    }

    private static void AddParameters(SqlCommand command, TaskDto task)
    {
        command.Parameters.AddWithValue("@project", task.MaDuAn);
        command.Parameters.AddWithValue("@name", task.TenTask);
        command.Parameters.AddWithValue("@description", (object?)task.MoTa ?? DBNull.Value);
        command.Parameters.AddWithValue("@due", (object?)task.HanNop ?? DBNull.Value);
        command.Parameters.AddWithValue("@priority", task.DoUuTien);
        command.Parameters.AddWithValue("@status", task.TrangThai);
    }
}
