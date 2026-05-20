using BrightPath.Models;
using BrightPath.DTOs;

namespace BrightPath.Repositories;

public interface IAdminAuditLogRepository
{
    void Add(AdminAuditLog log);
    PagedResultDto<AdminAuditLogResponseDto> GetPaged(int page, int pageSize, string? search);
}