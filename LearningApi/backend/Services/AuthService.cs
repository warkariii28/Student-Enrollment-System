using LearningApi.DTOs;
using LearningApi.Models;
using LearningApi.Repositories;
using LearningApi.Exceptions;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LearningApi.Services;

public class AuthService : IAuthService
{
    private readonly IAuthRepository _repo;
    private readonly IConfiguration _config;

    public AuthService(IAuthRepository repo, IConfiguration config)
    {
        _repo = repo;
        _config = config;
    }

    public void Register(User user, string password)
    {
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
        _repo.Register(user);
    }

    public AuthResultDto Login(LoginDto dto)
    {
        var user = _repo.GetByEmail(dto.Email);

        if (user == null)
            throw new BadRequestException("Invalid credentials");

        bool valid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);

        if (!valid)
            throw new BadRequestException("Invalid credentials");

        return new AuthResultDto
        {
            Token = GenerateToken(user),
            UserId = user.UserID,
            Name = user.Username,
            Email = user.Email
        };
    }

    public void UpdateEmail(int userId, string email)
    {
        bool updated = _repo.UpdateEmail(userId, email);

        if (!updated)
            throw new NotFoundException("User not found");
    }

    private string GenerateToken(User user)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:Key"]!)
        );

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim("UserID", user.UserID.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(2),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}