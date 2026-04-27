using LearningApi.DTOs;
using LearningApi.Models;

namespace LearningApi.Repositories;

public interface ICourseRepository
{
    List<CourseResponseDto> GetAll();
    int Add(Course course);
    bool Delete(int id);

    Course? GetById(int id);
    bool Update(Course course);
}