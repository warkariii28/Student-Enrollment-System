using LearningApi.Models;
using LearningApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using LearningApi.DTOs;
using LearningApi.Helpers;

[Authorize]
[ApiController]
[Route("api/students")]
public class StudentsController : ControllerBase
{
    private readonly IStudentService _service;

    public StudentsController(IStudentService service)
    {
        _service = service;
    }

    // ✅ All users
    [HttpGet]
    public IActionResult GetAll()
    {
        var data = _service.GetAll();
        return Ok(ResponseHelper.Success(data, "Students fetched successfully"));
    }

    // ✅ All users
    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var student = _service.GetById(id);
        return Ok(ResponseHelper.Success(student, "Student fetched successfully"));
    }

    // 🔒 Admin only
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

        return StatusCode(201, ResponseHelper.Success(id, "Created"));
    }

    // 🔒 Admin only
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public IActionResult Update(int id, StudentUpdateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ResponseHelper.Fail<object>("Invalid student data"));

        var student = new Student
        {
            StudentID = id,
            Name = dto.Name,
            Email = dto.Email
        };

        _service.Update(student);

        return Ok(ResponseHelper.Success<object>(null, "Updated successfully"));
    }

    // 🔒 Admin only
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        _service.Delete(id);
        return NoContent();
    }

    // ✅ Any logged-in user
    [HttpGet("me")]
    public IActionResult GetCurrentUser()
    {
        var username = User.Identity?.Name;
        var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;

        return Ok(ResponseHelper.Success(new { username, email }, "User fetched successfully"));
    }
}