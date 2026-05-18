using LearningApi.DTOs;
using LearningApi.Models;

namespace LearningApi.Services;

public interface IStudentService
{
    List<StudentResponseDto> GetAll();
    PagedResultDto<StudentResponseDto> GetPaged(int page, int pageSize);
    Student GetById(int id);

    int Add(Student student);

    void Delete(int id);

    void Update(Student student);
}