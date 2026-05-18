namespace LearningApi.DTOs;

public class AdminAuditLogResponseDto
{
    public string AdminName { get; set; } = "";
    public string AdminEmail { get; set; } = "";
    public int AuditLogID { get; set; }
    public int AdminUserID { get; set; }
    public string Action { get; set; } = "";
    public string EntityName { get; set; } = "";
    public int? EntityID { get; set; }
    public string? Details { get; set; }
    public string? IpAddress { get; set; }
    public DateTime CreatedAt { get; set; }
}