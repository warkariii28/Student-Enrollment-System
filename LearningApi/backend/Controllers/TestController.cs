using LearningApi.Models;
using LearningApi.DTOs;
using Microsoft.AspNetCore.Mvc;
using LearningApi.Helpers;

namespace LearningApi.Controllers;

[ApiController]
[Route("api/test")]

public class TestController : ControllerBase
{
    // Create Student
    [HttpPost("student")]
    public IActionResult CreateStudent(StudentCreateDto dto)
    {
        var student = new Student { Name = dto.Name, Email = dto.Email };
        return Ok(ResponseHelper.Success(student, "Student created"));
    }

    // Get Student
    [HttpGet("student")]
    public IActionResult GetStudent()
    {
        var s = new StudentResponseDto
        {
            StudentID = 1,
            Name = "Atharv",
            Email = "atharv@email.com"
        };

        return Ok(ResponseHelper.Success(s, "Student fetched"));
    }

    // Get Student with ID 
    [HttpGet("student/{id}")]
    public IActionResult GetStudentById(int id)
    {
        return Ok(ResponseHelper.Success<object>(null, $"Student ID received: {id}"));
    }

    // Rate and limit per page
    [HttpGet("students")]
    public IActionResult GetStudents(int page, int limit)
    {
        return Ok(ResponseHelper.Success<object>(null, $"page={page}, limit={limit}"));
    }

    // Filtering example
    [HttpGet("search")]
    public IActionResult Search(string name)
    {
        return Ok(ResponseHelper.Success<object>(null, $"Search for {name}"));
    }

    // Combine filtering and pagination
    [HttpGet("students-filter")]
    public IActionResult Filter(string?name,int page=1,int limit=10)
    {
        var result = new { name, page, limit };
        return Ok(ResponseHelper.Success(result, "Filtered results"));
    }    
}