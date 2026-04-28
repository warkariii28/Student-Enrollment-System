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
    public IActionResult Register(RegisterDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ResponseHelper.Fail<object>("Invalid data"));

        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email
        };

        _service.Register(user, dto.Password);

        return StatusCode(201,
            ResponseHelper.Success<object>(null, "User registered successfully"));
    }

    [HttpPost("login")]
    public IActionResult Login(LoginDto dto)
    {
        var result = _service.Login(dto);

        if (result == null)
            return Unauthorized(ResponseHelper.Fail<object>("Invalid credentials"));

        var response = new AuthResponseDto
        {
            Token = result.Token,
            User = new UserDto
            {
                UserId = result.UserId,
                Name = result.Name,
                Email = result.Email
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
}