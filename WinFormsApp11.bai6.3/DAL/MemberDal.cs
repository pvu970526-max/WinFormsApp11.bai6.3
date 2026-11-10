using Microsoft.Data.SqlClient;
using WinFormsApp11.bai6._3.DTO;

namespace WinFormsApp11.bai6._3.DAL;

public sealed class MemberDal
{
    public List<MemberDto> GetAll()
    {
        using var connection = Db.OpenConnection();
        using var command = new SqlCommand("SELECT MaThanhVien, HoTen, Email, ChuyenMon FROM ThanhVien ORDER BY HoTen", connection);
        using var reader = command.ExecuteReader();
        var result = new List<MemberDto>();
        while (reader.Read())
        {
            result.Add(new MemberDto
            {
                MaThanhVien = reader.GetInt32(0), HoTen = reader.GetString(1),
                Email = reader.IsDBNull(2) ? null : reader.GetString(2),
                ChuyenMon = reader.IsDBNull(3) ? null : reader.GetString(3)
            });
        }
        return result;
    }
}
