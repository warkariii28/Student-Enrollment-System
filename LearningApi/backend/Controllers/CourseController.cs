using LearningApi.DTOs;
using LearningApi.Models;
using LearningApi.Services;
using LearningApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LearningApi.Helpers;

[Authorize]
[ApiController]
[Route("api/courses")]
public class CoursesController : ControllerBase
{
    private readonly ICourseService _service;
    private readonly IAdminAuditLogRepository _auditRepo;

    public CoursesController(
        ICourseService service,
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

        var data = _service.GetPaged(page, pageSize);

        return Ok(ResponseHelper.Success(data, "Courses fetched successfully"));
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        if (id <= 0)
        {
            return BadRequest(ResponseHelper.Fail<object>("Invalid course ID"));
        }

        var course = _service.GetById(id);
        return Ok(ResponseHelper.Success(course, "Course fetched successfully"));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public IActionResult Add(CourseCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ResponseHelper.Fail<object>("Invalid course data"));

        var course = new Course
        {
            CourseName = dto.CourseName,
            Fee = dto.Fee,
            DurationWeeks = dto.DurationWeeks
        };

        int id = _service.Add(course);

        WriteAdminAudit("CreateCourse", id, $"Created course {dto.CourseName}");

        return StatusCode(201, ResponseHelper.Success(id, "Course created successfully"));
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public IActionResult Update(int id, CourseUpdateDto dto)
    {
        if (id <= 0)
        {
            return BadRequest(ResponseHelper.Fail<object>("Invalid course ID"));
        }

        if (!ModelState.IsValid)
            return BadRequest(ResponseHelper.Fail<object>("Invalid course data"));

        var course = new Course
        {
            CourseID = id,
            CourseName = dto.CourseName,
            Fee = dto.Fee,
            DurationWeeks = dto.DurationWeeks
        };

        _service.Update(course);

        WriteAdminAudit("UpdateCourse", id, $"Updated course {dto.CourseName}");

        return Ok(ResponseHelper.Success<object>(null, "Updated successfully"));
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        if (id <= 0)
        {
            return BadRequest(ResponseHelper.Fail<object>("Invalid course ID"));
        }

        _service.Delete(id);

        WriteAdminAudit("DeleteCourse", id);

        return Ok(ResponseHelper.Success<object>(null, "Course deleted successfully"));
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
            EntityName = "Course",
            EntityID = entityId,
            Details = details,
            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
        });
    }
}
