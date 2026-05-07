namespace LearningApi.Models;

public class AdminAuditLog
{
    public int AdminUserID { get; set; }
    public string Action { get; set; } = "";
    public string EntityName { get; set; } = "";
    public int? EntityID { get; set; }
    public string? Details { get; set; }
    public string? IpAddress { get; set; }
}
