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

    // Get ALL Students
    [HttpGet]
    public IActionResult GetAll()
    {
        var data = _service.GetAll();

        return Ok(ResponseHelper.Success(data, "Students fetched successfully"));
    }

    // Get by ID
    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var student = _service.GetById(id);

        return Ok(ResponseHelper.Success(student, "Student fetched successfully"));
    }

    // Create
    [HttpPost]
    public IActionResult Add(StudentCreateDto dto)
    {
        var student = new Student
        {
            Name = dto.Name,
            Email = dto.Email
        };

        int id = _service.Add(student);

        return Ok(ResponseHelper.Success(id, "Student created successfully"));
    }

    // Delete
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        _service.Delete(id);

        return Ok(ResponseHelper.Success("Deleted successfully"));
    }

    // Current User Info
    [HttpGet("me")]
    public IActionResult GetCurrentUser()
    {
        var username = User.Identity?.Name;

        var email = User.FindFirst(
            System.Security.Claims.ClaimTypes.Email)?.Value;

        var data = new
        {
            username,
            email
        };

        return Ok(ResponseHelper.Success(data, "User fetched successfully"));
    }

    // Update
    [HttpPut("{id}")]
    public IActionResult Update(int id, StudentUpdateDto dto)
    {
        var student = new Student
        {
            StudentID = id,
            Name = dto.Name,
            Email = dto.Email
        };

        _service.Update(student);

        return Ok(ResponseHelper.Success("Updated successfully"));
    }
}