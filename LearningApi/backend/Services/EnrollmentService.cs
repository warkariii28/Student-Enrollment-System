using LearningApi.DTOs;
using LearningApi.Models;
using LearningApi.Repositories;
using LearningApi.Exceptions;

namespace LearningApi.Services;

public class EnrollmentService : IEnrollmentService
{
    private readonly IEnrollmentRepository _repo;

    public EnrollmentService(IEnrollmentRepository repo)
    {
        _repo = repo;
    }

    public List<EnrollmentResponseDto> GetAll()
    {
        return _repo.GetAll();
    }

    public void Add(Enrollment enrollment)
    {
        if (enrollment.StudentID <= 0 || enrollment.CourseID <= 0)
            throw new BadRequestException("Invalid enrollment data");

        try
        {
            _repo.Add(enrollment);
        }
        catch (Exception ex)
        {
            throw new BadRequestException(ex.Message);
        }
    }

    public void Delete(int id)
    {
        if (id <= 0)
            throw new BadRequestException("Invalid ID");

        bool deleted = _repo.Delete(id);

        if (!deleted)
            throw new NotFoundException("Enrollment not found");
    }
}