using LearningApi.DTOs;
using LearningApi.Models;
using LearningApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LearningApi.Helpers;

[Authorize]
[ApiController]
[Route("api/enrollments")]
public class EnrollmentsController : ControllerBase
{
    private readonly IEnrollmentService _service;

    public EnrollmentsController(IEnrollmentService service)
    {
        _service = service;
    }

    // ✅ All users
    [HttpGet]
    public IActionResult GetAll()
    {
        var data = _service.GetAll();
        return Ok(ResponseHelper.Success(data, "Enrollments fetched successfully"));
    }

    // 🔒 Admin only
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

        return StatusCode(201, ResponseHelper.Success<object>(null, "Enrollment created successfully"));
    }

    // 🔒 Admin only
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        _service.Delete(id);
        return NoContent();
    }
}