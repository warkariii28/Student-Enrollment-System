using LearningApi.DTOs;
using LearningApi.Models;

namespace LearningApi.Repositories;

public interface IEnrollmentRepository
{
    List<EnrollmentResponseDto> GetAll();
    void Add(Enrollment enrollment);
    bool Delete(int id);
}