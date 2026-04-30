using LearningApi.Models;
using LearningApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using LearningApi.DTOs;
using LearningApi.Helpers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _service;

    public AuthController(IAuthService service)
    {
        _service = service;
    }

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

        if (userIdClaim == null)
            return Unauthorized(ResponseHelper.Fail<object>("Invalid token"));

        int userId = int.Parse(userIdClaim);

        _service.UpdateEmail(userId, email);

        return Ok(ResponseHelper.Success<object>(null, "Email updated successfully"));
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("assign-role")]
    public IActionResult AssignRole([FromQuery] int userId, [FromQuery] string role)
    {
        _service.UpdateRole(userId, role);
        return Ok(ResponseHelper.Success<object>(null, "Role updated"));
    }

    [HttpPost("refresh")]
    public IActionResult Refresh([FromBody] RefreshRequest request)
    {
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
}

// ✅ MUST be here (after controller)
public class RefreshRequest
{
    public string RefreshToken { get; set; } = "";
}