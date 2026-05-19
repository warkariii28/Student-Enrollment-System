using LearningApi.DTOs;
using LearningApi.Models;
using LearningApi.Repositories;

namespace backend.Tests.Fakes;

public class FakeEnrollmentRepository : IEnrollmentRepository
{
    public List<EnrollmentResponseDto> Enrollments { get; } = [];
    public bool DeleteResult { get; set; } = true;
    public Enrollment? AddedEnrollment { get; private set; }
    public int DeletedId { get; private set; }

    public List<EnrollmentResponseDto> GetAll()
    {
        return Enrollments;
    }

    public PagedResultDto<EnrollmentResponseDto> GetPaged(int page, int pageSize, string? search)
    {
        return new PagedResultDto<EnrollmentResponseDto>
        {
            Items = Enrollments,
            TotalCount = Enrollments.Count,
            Page = page,
            PageSize = pageSize
        };
    }

    public void Add(Enrollment enrollment)
    {
        AddedEnrollment = enrollment;
    }

    public bool Delete(int id)
    {
        DeletedId = id;
        return DeleteResult;
    }
}