using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
namespace BrightPath.DTOs;

public class RegisterDto
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    [DefaultValue("User Name")]
    public string Username { get; set; } = "";

    [Required]
    [EmailAddress]
    [DefaultValue("user@email.com")]
    public string Email { get; set; } = "";

    [Required]
    [StringLength(100, MinimumLength = 8)]
    [DefaultValue("Password123")]
    [RegularExpression(
    @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$",
    ErrorMessage = "Password must contain uppercase, lowercase, and number")]
    public string Password { get; set; } = "";
}