using Microsoft.Data.SqlClient;

namespace WinFormsApp11.bai6._3.DAL;

internal static class Db
{
    // Change Server to the SQL Server instance used by the local machine if needed.
    public const string ConnectionString = "Server=(localdb)\\MSSQLLocalDB;Database=ProjectTaskDB;Trusted_Connection=True;TrustServerCertificate=True;";

    public static SqlConnection OpenConnection()
    {
        var connection = new SqlConnection(ConnectionString);
        connection.Open();
        return connection;
    }
}
