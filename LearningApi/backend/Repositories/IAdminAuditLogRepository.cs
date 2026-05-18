using LearningApi.Models;
using LearningApi.DTOs;

namespace LearningApi.Repositories;

public interface IAdminAuditLogRepository
{
    void Add(AdminAuditLog log);
    PagedResultDto<AdminAuditLogResponseDto> GetPaged(int page, int pageSize, string? search);
}