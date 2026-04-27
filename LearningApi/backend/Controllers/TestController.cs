using LearningApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace LearningApi.Controllers;

[ApiController]
[Route("api/test")]

public class TestController : ControllerBase
{
    // Create Student
    [HttpPost("student")]
    public IActionResult CreateStudent(Student student)
    {
        return Ok(student);
    }

    // Get Student
    [HttpGet("student")]
    public IActionResult GetStudent()
    {
        var s = new Student
        {
            StudentID = 1,
            Name = "Atharv",
            Email = "atharv@email.com"
        };

        return Ok(s);
    }

    // Get Student with ID 
    [HttpGet("student/{id}")]
    public IActionResult GetStudentById(int id)
    {
        return Ok($"Student ID received: {id}");
    }

    // Rate and limit per page
    [HttpGet("students")]
    public IActionResult GetStudents(int page, int limit)
    {
        return Ok($"page={page}, limit={limit}");
    }

    // Filtering example
    [HttpGet("search")]
    public IActionResult Search(string name)
    {
        return Ok($"Search for {name}");
    }

    // Combine filtering and pagination
    [HttpGet("students-filter")]
    public IActionResult Filter(string?name,int page=1,int limit=10)
    {
        return Ok(new{name,page,limit});
    }    
}