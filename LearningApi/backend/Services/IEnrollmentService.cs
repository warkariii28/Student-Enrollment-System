using LearningApi.DTOs;
using LearningApi.Models;

namespace LearningApi.Services;

public interface IEnrollmentService
{
    List<EnrollmentResponseDto> GetAll();
    PagedResultDto<EnrollmentResponseDto> GetPaged(int page, int pageSize);
    void Add(Enrollment enrollment);
    void Delete(int id);
}