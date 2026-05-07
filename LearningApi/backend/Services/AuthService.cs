using LearningApi.DTOs;
using LearningApi.Models;
using LearningApi.Repositories;
using LearningApi.Exceptions;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;

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

        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            throw new BadRequestException("Invalid credentials");

        var accessToken = GenerateToken(user);
        var refreshToken = GenerateRefreshToken();
        var refreshTokenHash = HashRefreshToken(refreshToken);

        _repo.SaveRefreshToken(user.UserID, refreshTokenHash);

        return new AuthResultDto
        {
            Token = accessToken,
            RefreshToken = refreshToken,
            UserId = user.UserID,
            Name = user.Username,
            Email = user.Email,
            Role = user.Role
        };


    }

    public void UpdateEmail(int userId, string email)
    {
        bool updated = _repo.UpdateEmail(userId, email);

        if (!updated)
            throw new NotFoundException("User not found");
    }

    public string GenerateToken(User user)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:Key"]!)
        );

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim("UserID", user.UserID.ToString()),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public void UpdateRole(int userId, string role)
    {
        bool updated = _repo.UpdateRole(userId, role);

        if (!updated)
            throw new NotFoundException("User not found");
    }

    private string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }

    public RefreshToken? ValidateRefreshToken(string token)
    {
        var tokenHash = HashRefreshToken(token);
        var stored = _repo.GetRefreshToken(tokenHash);

        if (stored == null || stored.ExpiresAt < DateTime.UtcNow)
            return null;

        return stored;
    }

    public User GetUserById(int userId)
    {
        var user = _repo.GetById(userId);

        if (user == null)
            throw new NotFoundException("User not found");

        return user;
    }

    private static string HashRefreshToken(string token)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(bytes);
    }


    public string RotateRefreshToken(string oldToken)
    {
        var oldTokenHash = HashRefreshToken(oldToken);
        var stored = _repo.GetRefreshToken(oldTokenHash);

        if (stored == null)
            throw new BadRequestException("Invalid refresh token");

        _repo.RevokeRefreshToken(oldTokenHash);

        var newToken = GenerateRefreshToken();
        var newTokenHash=HashRefreshToken(newToken);

        _repo.SaveRefreshToken(stored.UserId, newTokenHash);

        return newToken;
    }


}