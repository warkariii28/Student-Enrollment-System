using LearningApi.DTOs;
using LearningApi.Models;
using LearningApi.Services;
using LearningApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LearningApi.Helpers;

[Authorize]
[ApiController]
[Route("api/enrollments")]
public class EnrollmentsController : ControllerBase
{
    private readonly IEnrollmentService _service;
    private readonly IAdminAuditLogRepository _auditRepo;

    public EnrollmentsController(
        IEnrollmentService service,
        IAdminAuditLogRepository auditRepo)
    {
        _service = service;
        _auditRepo = auditRepo;
    }

    [HttpGet]
    public IActionResult GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        if (page <= 0 || pageSize <= 0 || pageSize > 100)
        {
            return BadRequest(ResponseHelper.Fail<object>("Invalid pagination values"));
        }

        var data = _service.GetAll()
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return Ok(ResponseHelper.Success(data, "Enrollments fetched successfully"));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public IActionResult Add(EnrollmentCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ResponseHelper.Fail<object>("Invalid enrollment data"));

        var enrollment = new Enrollment
        {
            StudentID = dto.StudentID,
            CourseID = dto.CourseID
        };

        _service.Add(enrollment);

        WriteAdminAudit(
            "CreateEnrollment",
            null,
            $"StudentID={dto.StudentID}, CourseID={dto.CourseID}");

        return StatusCode(201, ResponseHelper.Success<object>(null, "Enrollment created successfully"));
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        if (id <= 0)
        {
            return BadRequest(ResponseHelper.Fail<object>("Invalid enrollment ID"));
        }

        _service.Delete(id);

        WriteAdminAudit("DeleteEnrollment", id);

        return NoContent();
    }

    private void WriteAdminAudit(string action, int? entityId, string? details = null)
    {
        var adminUserIdClaim = User.FindFirst("UserID")?.Value;

        if (!int.TryParse(adminUserIdClaim, out int adminUserId))
            return;

        _auditRepo.Add(new AdminAuditLog
        {
            AdminUserID = adminUserId,
            Action = action,
            EntityName = "Enrollment",
            EntityID = entityId,
            Details = details,
            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
        });
    }
}
