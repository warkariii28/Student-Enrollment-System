using LearningApi.DTOs;
using LearningApi.Models;

namespace LearningApi.Services;

public interface IAuthService
{
    void Register(User user, string password);

    AuthResultDto Login(LoginDto dto);

    void UpdateEmail(int userId, string email);
}