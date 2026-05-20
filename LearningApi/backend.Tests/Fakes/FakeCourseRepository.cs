using BrightPath.DTOs;
using BrightPath.Models;
using BrightPath.Repositories;

namespace backend.Tests.Fakes;

public class FakeCourseRepository : ICourseRepository
{
    public List<CourseResponseDto> Courses { get; } = [];
    public Course? CourseToReturn { get; set; }
    public bool DeleteResult { get; set; } = true;
    public bool UpdateResult { get; set; } = true;
    public Course? AddedCourse { get; private set; }
    public Course? UpdatedCourse { get; private set; }
    public int DeletedId { get; private set; }

    public List<CourseResponseDto> GetAll()
    {
        return Courses;
    }

    public PagedResultDto<CourseResponseDto> GetPaged(int page, int pageSize, string? search)
    {
        return new PagedResultDto<CourseResponseDto>
        {
            Items = Courses,
            TotalCount = Courses.Count,
            Page = page,
            PageSize = pageSize
        };
    }

    public Course? GetById(int id)
    {
        return CourseToReturn;
    }

    public int Add(Course course)
    {
        AddedCourse = course;
        return 20;
    }

    public bool Delete(int id)
    {
        DeletedId = id;
        return DeleteResult;
    }

    public bool Update(Course course)
    {
        UpdatedCourse = course;
        return UpdateResult;
    }
}