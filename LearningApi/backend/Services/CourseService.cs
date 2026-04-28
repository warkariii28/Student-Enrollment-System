using LearningApi.DTOs;
using LearningApi.Models;
using LearningApi.Repositories;
using LearningApi.Exceptions;

namespace LearningApi.Services;

public class CourseService : ICourseService
{
    private readonly ICourseRepository _repo;

    public CourseService(ICourseRepository repo)
    {
        _repo = repo;
    }

    public List<CourseResponseDto> GetAll()
    {
        return _repo.GetAll();
    }

    public Course GetById(int id)
    {
        var course = _repo.GetById(id);

        if (course == null)
            throw new NotFoundException("Course not found");

        return course;
    }

    public int Add(Course course)
    {
        return _repo.Add(course);
    }

    public void Delete(int id)
    {
        if (id <= 0)
            throw new BadRequestException("Invalid ID");

        bool deleted = _repo.Delete(id);

        if (!deleted)
            throw new NotFoundException("Course not found");
    }

    public void Update(Course course)
    {
        bool updated = _repo.Update(course);

        if (!updated)
            throw new NotFoundException("Course not found");
    }
}