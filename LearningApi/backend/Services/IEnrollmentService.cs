using LearningApi.DTOs;
using LearningApi.Models;

namespace LearningApi.Services;

public interface IEnrollmentService
{
    List<EnrollmentResponseDto> GetAll();

    void Add(Enrollment enrollment);

    void Delete(int id);
}