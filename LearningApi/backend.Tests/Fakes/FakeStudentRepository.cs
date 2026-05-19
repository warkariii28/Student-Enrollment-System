using LearningApi.DTOs;
using LearningApi.Models;
using LearningApi.Repositories;

namespace backend.Tests.Fakes;

public class FakeStudentRepository : IStudentRepository
{
    public List<StudentResponseDto> Students { get; } = [];
    public Student? StudentToReturn { get; set; }
    public bool DeleteResult { get; set; } = true;
    public bool UpdateResult { get; set; } = true;
    public Student? AddedStudent { get; private set; }
    public Student? UpdatedStudent { get; private set; }
    public int DeletedId { get; private set; }

    public List<StudentResponseDto> GetAll()
    {
        return Students;
    }

    public PagedResultDto<StudentResponseDto> GetPaged(int page, int pageSize, string? search)
    {
        return new PagedResultDto<StudentResponseDto>
        {
            Items = Students,
            TotalCount = Students.Count,
            Page = page,
            PageSize = pageSize
        };
    }

    public Student? GetById(int id)
    {
        return StudentToReturn;
    }

    public int Add(Student student)
    {
        AddedStudent = student;
        return 10;
    }

    public bool Delete(int id)
    {
        DeletedId = id;
        return DeleteResult;
    }

    public bool Update(Student student)
    {
        UpdatedStudent = student;
        return UpdateResult;
    }
}