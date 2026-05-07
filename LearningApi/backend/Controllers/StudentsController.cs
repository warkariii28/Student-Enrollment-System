using LearningApi.Models;
using LearningApi.Services;
using LearningApi.Models;
using LearningApi.Repositories;
using LearningApi.DTOs;
using LearningApi.Helpers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

[Authorize]
[ApiController]
[Route("api/students")]
public class StudentsController : ControllerBase
{
    private readonly IStudentService _service;
    private readonly IAdminAuditLogRepository _auditRepo;

    public StudentsController(IStudentService service, IAdminAuditLogRepository auditRepo)
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
        return Ok(ResponseHelper.Success(data, "Students fetched successfully"));
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        if (id <= 0)
        {
            return BadRequest(ResponseHelper.Fail<object>("Invalid student ID"));
        }

        var student = _service.GetById(id);
        return Ok(ResponseHelper.Success(student, "Student fetched successfully"));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public IActionResult Add(StudentCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ResponseHelper.Fail<object>("Invalid data"));

        var student = new Student
        {
            Name = dto.Name,
            Email = dto.Email
        };

        var id = _service.Add(student);
        WriteAdminAudit("CreateStudent", id, $"Created student email {dto.Email}");

        return StatusCode(201, ResponseHelper.Success(id, "Created"));
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public IActionResult Update(int id, StudentUpdateDto dto)
    {

        if (id <= 0)
        {
            return BadRequest(ResponseHelper.Fail<object>("Invalid student ID"));
        }

        if (!ModelState.IsValid)
            return BadRequest(ResponseHelper.Fail<object>("Invalid student data"));

        var student = new Student
        {
            StudentID = id,
            Name = dto.Name,
            Email = dto.Email
        };

        _service.Update(student);
        WriteAdminAudit("UpdateStudent", id, $"Updated student email {dto.Email}");


        return Ok(ResponseHelper.Success<object>(null, "Updated successfully"));
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {

        if (id <= 0)
        {
            return BadRequest(ResponseHelper.Fail<object>("Invalid student ID"));
        }
        _service.Delete(id);
        WriteAdminAudit("DeleteStudent", id);

        return NoContent();
    }

    [HttpGet("me")]
    public IActionResult GetCurrentUser()
    {
        var username = User.Identity?.Name;
        var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;

        return Ok(ResponseHelper.Success(new { username, email }, "User fetched successfully"));
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
            EntityName = "Student",
            EntityID = entityId,
            Details = details,
            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
        });
    }

}