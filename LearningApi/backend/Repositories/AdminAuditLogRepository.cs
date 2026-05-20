using BrightPath.Models;
using BrightPath.DTOs;
using Microsoft.Data.SqlClient;
using System.Data;

namespace BrightPath.Repositories;

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

    public PagedResultDto<AdminAuditLogResponseDto> GetPaged(int page, int pageSize, string? search)
    {
        var logs = new List<AdminAuditLogResponseDto>();
        var totalCount = 0;

        using SqlConnection conn = new SqlConnection(_conn);
        using SqlCommand cmd = new SqlCommand("GetAdminAuditLogs", conn);

        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandTimeout = 30;

        cmd.Parameters.Add("@Page", SqlDbType.Int).Value = page;
        cmd.Parameters.Add("@PageSize", SqlDbType.Int).Value = pageSize;
        cmd.Parameters.Add("@Search", SqlDbType.NVarChar, 200).Value =
            string.IsNullOrWhiteSpace(search) ? DBNull.Value : search.Trim();

        conn.Open();

        using SqlDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            logs.Add(new AdminAuditLogResponseDto
            {
                AuditLogID = Convert.ToInt32(reader["AuditLogID"]),
                AdminUserID = Convert.ToInt32(reader["AdminUserID"]),
                AdminName = reader["AdminName"]?.ToString() ?? "",
                AdminEmail = reader["AdminEmail"]?.ToString() ?? "",
                Action = reader["Action"]?.ToString() ?? "",
                EntityName = reader["EntityName"]?.ToString() ?? "",
                EntityID = reader["EntityID"] == DBNull.Value ? null : Convert.ToInt32(reader["EntityID"]),
                Details = reader["Details"] == DBNull.Value ? null : reader["Details"].ToString(),
                IpAddress = reader["IpAddress"] == DBNull.Value ? null : reader["IpAddress"].ToString(),
                CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
            });
        }

        if (reader.NextResult() && reader.Read())
        {
            totalCount = Convert.ToInt32(reader["TotalCount"]);
        }

        return new PagedResultDto<AdminAuditLogResponseDto>
        {
            Items = logs,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }
}
