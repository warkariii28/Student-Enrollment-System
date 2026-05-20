using BrightPath.Helpers;
using BrightPath.Repositories;
using BrightPath.Constants;
using BrightPath.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = AppRoles.Admin)]
[ApiController]
[Route("api/admin-audit-logs")]
public class AdminAuditLogsController : ControllerBase
{
    private readonly IAdminAuditLogRepository _repo;

    public AdminAuditLogsController(IAdminAuditLogRepository repo)
    {
        _repo = repo;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResultDto<AdminAuditLogResponseDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public IActionResult GetAll(
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 20,
    [FromQuery] string? search = null)
    {
        if (page <= 0 || pageSize <= 0 || pageSize > 100)
        {
            return BadRequest(ResponseHelper.Fail<object>("Invalid pagination values"));
        }

        var logs = _repo.GetPaged(page, pageSize, search);
        return Ok(ResponseHelper.Success(logs, "Audit logs fetched successfully"));
    }
}
