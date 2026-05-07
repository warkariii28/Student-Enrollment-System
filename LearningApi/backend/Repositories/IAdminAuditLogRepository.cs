using LearningApi.Models;

namespace LearningApi.Repositories;

public interface IAdminAuditLogRepository
{
    void Add(AdminAuditLog log);
}
