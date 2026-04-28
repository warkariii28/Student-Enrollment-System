using LearningApi.DTOs;
using LearningApi.Models;
using LearningApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LearningApi.Helpers;

[Authorize]
[ApiController]
[Route("api/courses")]
public class CoursesController : ControllerBase
{
    private readonly ICourseService _service;

    public CoursesController(ICourseService service)
    {
        _service = service;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var data = _service.GetAll();

        return Ok(ResponseHelper.Success(data, "Courses fetched successfully"));
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var course = _service.GetById(id);

        return Ok(ResponseHelper.Success(course, "Course fetched successfully"));
    }

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

        return StatusCode(201, ResponseHelper.Success(id, "Course created successfully"));
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, CourseUpdateDto dto)
    {
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

        return Ok(ResponseHelper.Success<object>(null, "Updated successfully"));
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        _service.Delete(id);

        return NoContent();
    }
}