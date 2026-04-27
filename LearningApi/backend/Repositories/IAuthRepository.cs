using LearningApi.Models;

namespace LearningApi.Repositories;

public interface IAuthRepository
{
    void Register(User user);
    User? GetByEmail(string email);
    bool UpdateEmail(int userId, string email);
}