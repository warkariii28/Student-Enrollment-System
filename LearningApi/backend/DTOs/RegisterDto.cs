using System.ComponentModel.DataAnnotations;

namespace LearningApi.DTOs;

public class RegisterDto
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Username { get; set; } = "";

    [Required]
    [EmailAddress]
    public string Email { get; set; } = "";

    [Required]
    [StringLength(100, MinimumLength = 8)]
    [RegularExpression(
    @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$",
    ErrorMessage = "Password must contain uppercase, lowercase, and number")]
    public string Password { get; set; } = "";
}