using LearningApi.DTOs;
using LearningApi.Models;
using LearningApi.Repositories;
using LearningApi.Exceptions;

namespace LearningApi.Services;

public class StudentService : IStudentService
{
    private readonly IStudentRepository _repo;

    public StudentService(IStudentRepository repo)
    {
        _repo = repo;
    }

    public List<StudentResponseDto> GetAll()
    {
        return _repo.GetAll();
    }

    public Student GetById(int id)
    {
        var student = _repo.GetById(id);

        if (student == null)
            throw new NotFoundException("Student not found");

        return student;
    }

    public int Add(Student student)
    {
        return _repo.Add(student);
    }

    public void Delete(int id)
    {
        if (id <= 0)
            throw new BadRequestException("Invalid ID");

        bool deleted = _repo.Delete(id);

        if (!deleted)
            throw new NotFoundException("Student not found");
    }

    public void Update(Student student)
    {
        bool updated = _repo.Update(student);

        if (!updated)
            throw new NotFoundException("Student not found");
    }
}