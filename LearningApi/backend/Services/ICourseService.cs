using LearningApi.DTOs;
using LearningApi.Models;

namespace LearningApi.Services;

public interface ICourseService
{
    List<CourseResponseDto> GetAll();

    Course GetById(int id);

    int Add(Course course);

    void Delete(int id);

    void Update(Course course);
}