using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace LearningApi.DTOs;

public class LoginDto
{
    [Required]
    [EmailAddress]
    [DefaultValue("admin@example.com")]
    public string Email { get; set; } = "";

    [Required]
    [DefaultValue("Password123")]
    public string Password { get; set; } = "";
}
