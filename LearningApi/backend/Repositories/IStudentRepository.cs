namespace LearningApi.Repositories;

using LearningApi.DTOs;
using LearningApi.Models;

public interface IStudentRepository
{
    List<StudentResponseDto> GetAll();
    Student? GetById(int id);
    int Add(Student student);
    bool Delete(int id);
    bool Update(Student student);
}