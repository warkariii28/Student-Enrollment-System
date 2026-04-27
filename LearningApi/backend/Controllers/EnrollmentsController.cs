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

    [HttpGet]
    public IActionResult GetAll()
    {
        var data = _service.GetAll();

        return Ok(ResponseHelper.Success(data, "Enrollments fetched successfully"));
    }

    [HttpPost]
    public IActionResult Add(EnrollmentCreateDto dto)
    {
        var enrollment = new Enrollment
        {
            StudentID = dto.StudentID,
            CourseID = dto.CourseID
        };

        _service.Add(enrollment);

        return Ok(ResponseHelper.Success("Enrollment created successfully"));
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        _service.Delete(id);

        return Ok(ResponseHelper.Success("Deleted successfully"));
    }
}