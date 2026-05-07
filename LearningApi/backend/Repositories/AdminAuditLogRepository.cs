using LearningApi.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace LearningApi.Repositories;

public class AdminAuditLogRepository : IAdminAuditLogRepository
{
    private readonly string _conn;

    public AdminAuditLogRepository(IConfiguration config)
    {
        _conn = config.GetConnectionString("DefaultConnection")!;
    }

    public void Add(AdminAuditLog log)
    {
        using SqlConnection conn = new SqlConnection(_conn);
        using SqlCommand cmd = new SqlCommand("AddAdminAuditLog", conn);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandTimeout = 30;

        cmd.Parameters.Add("@AdminUserID", SqlDbType.Int).Value = log.AdminUserID;
        cmd.Parameters.Add("@Action", SqlDbType.NVarChar, 100).Value = log.Action;
        cmd.Parameters.Add("@EntityName", SqlDbType.NVarChar, 100).Value = log.EntityName;
        cmd.Parameters.Add("@EntityID", SqlDbType.Int).Value = (object?)log.EntityID ?? DBNull.Value;
        cmd.Parameters.Add("@Details", SqlDbType.NVarChar, 500).Value = (object?)log.Details ?? DBNull.Value;
        cmd.Parameters.Add("@IpAddress", SqlDbType.NVarChar, 100).Value = (object?)log.IpAddress ?? DBNull.Value;

        conn.Open();
        cmd.ExecuteNonQuery();
    }
}
