using LearningApi.Models;
using LearningApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using LearningApi.DTOs;
using LearningApi.Helpers;

[ApiController]
[Route("api")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _service;

    public AuthController(IAuthService service)
    {
        _service = service;
    }

    [HttpPost("register")]
    public IActionResult Register(User user)
    {
        _service.Register(user, user.PasswordHash);

        return Ok(ResponseHelper.Success("User registered successfully"));
    }

    [HttpPost("login")]
    public IActionResult Login(LoginDto dto)
    {
        var result = _service.Login(dto);

        if (result == null)
            return BadRequest(ResponseHelper.Fail("Invalid credentials"));

        var response = new AuthResponseDto
        {
            Token = result.Token,
            User = new
            {
                result.UserId,
                result.Name,
                result.Email
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
            return Unauthorized(ResponseHelper.Fail("Invalid token"));

        int userId = int.Parse(userIdClaim);

        _service.UpdateEmail(userId, email);

        return Ok(ResponseHelper.Success("Email updated successfully"));
    }
}