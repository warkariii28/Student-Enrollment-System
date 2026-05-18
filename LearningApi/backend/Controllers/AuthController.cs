using LearningApi.Models;
using LearningApi.Services;
using LearningApi.Repositories;
using LearningApi.DTOs;
using LearningApi.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using System.ComponentModel.DataAnnotations;


[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _service;
    private static readonly EmailAddressAttribute EmailValidator = new();
    private readonly IAdminAuditLogRepository _auditRepo;

    public AuthController(IAuthService service,
    IAdminAuditLogRepository auditRepo)
    {
        _service = service;
        _auditRepo = auditRepo;
    }

    private static readonly HashSet<string> AllowedRoles =
    new(StringComparer.OrdinalIgnoreCase)
    {
        "Admin",
        "User"
    };

    [EnableRateLimiting("AuthPolicy")]
    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ResponseHelper.Fail<object>("Invalid data"));

        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            Role = "User"
        };

        _service.Register(user, dto.Password);

        return StatusCode(201,
            ResponseHelper.Success<object>(null, "User registered successfully"));
    }

    [EnableRateLimiting("AuthPolicy")]
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ResponseHelper.Fail<object>("Invalid data"));

        var result = _service.Login(dto);

        var response = new AuthResponseDto
        {
            Token = result.Token,
            RefreshToken = result.RefreshToken,
            User = new UserDto
            {
                UserId = result.UserId,
                Name = result.Name,
                Email = result.Email,
                Role = result.Role
            }
        };

        return Ok(ResponseHelper.Success(response, "Login successful"));
    }

    [Authorize]
    [HttpPut("update-email")]
    public IActionResult UpdateEmail([FromQuery] string email)
    {
        var userIdClaim = User.FindFirst("UserID")?.Value;

        if (!int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized(ResponseHelper.Fail<object>("Invalid token"));
        }

        if (string.IsNullOrWhiteSpace(email) || !EmailValidator.IsValid(email))
        {
            return BadRequest(ResponseHelper.Fail<object>("Invalid email"));
        }

        _service.UpdateEmail(userId, email);

        return Ok(ResponseHelper.Success<object>(null, "Email updated successfully"));
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("assign-role")]
    public IActionResult AssignRole([FromQuery] int userId, [FromQuery] string role)
    {
        if (userId <= 0)
        {
            return BadRequest(ResponseHelper.Fail<object>("Invalid user ID"));
        }

        if (string.IsNullOrWhiteSpace(role) || !AllowedRoles.Contains(role))
        {
            return BadRequest(ResponseHelper.Fail<object>("Invalid role"));
        }

        var normalizedRole = AllowedRoles.First(r =>
            string.Equals(r, role, StringComparison.OrdinalIgnoreCase));

        _service.UpdateRole(userId, normalizedRole);
        
        var adminUserIdClaim = User.FindFirst("UserID")?.Value;

        if (int.TryParse(adminUserIdClaim, out int adminUserId))
        {
            _auditRepo.Add(new AdminAuditLog
            {
                AdminUserID = adminUserId,
                Action = "AssignRole",
                EntityName = "User",
                EntityID = userId,
                Details = $"Role changed to {normalizedRole}",
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
            });
        }

        return Ok(ResponseHelper.Success<object>(null, "Role updated"));
    }

    [Authorize]
    [EnableRateLimiting("AuthPolicy")]
    [HttpPost("refresh")]
    public IActionResult Refresh([FromBody] RefreshRequest request)
    {

        if (request == null || string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            return BadRequest(ResponseHelper.Fail<object>("Refresh token is required"));
        }

        var stored = _service.ValidateRefreshToken(request.RefreshToken);

        if (stored == null)
            return Unauthorized(ResponseHelper.Fail<object>("Invalid refresh token"));

        var user = _service.GetUserById(stored.UserId);

        var newAccess = _service.GenerateToken(user);
        var newRefresh = _service.RotateRefreshToken(request.RefreshToken);

        return Ok(ResponseHelper.Success(new
        {
            token = newAccess,
            refreshToken = newRefresh
        }, "Token refreshed"));
    }

    [Authorize]
    [HttpPost("logout")]
    public IActionResult Logout([FromBody] RefreshRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            return BadRequest(ResponseHelper.Fail<object>("Refresh token is required"));
        }

        _service.RevokeRefreshToken(request.RefreshToken);

        return Ok(ResponseHelper.Success<object>(null, "Logged out successfully"));
    }    
}

// ✅ MUST be here (after controller)
public class RefreshRequest
{
    public string RefreshToken { get; set; } = "";
}