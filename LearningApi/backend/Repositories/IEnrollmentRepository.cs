using LearningApi.DTOs;
using LearningApi.Models;

namespace LearningApi.Repositories;

public interface IEnrollmentRepository
{
    List<EnrollmentResponseDto> GetAll();
    PagedResultDto<EnrollmentResponseDto> GetPaged(int page, int pageSize);
    void Add(Enrollment enrollment);
    bool Delete(int id);
}