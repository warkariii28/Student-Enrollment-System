using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
namespace BrightPath.DTOs;

public class StudentCreateDto
{
    [Required]
    [StringLength(100)]
    [DefaultValue("User Name")]
    [RegularExpression(@"^[A-Za-z][A-Za-z\s.'-]*$", ErrorMessage = "Name must contain letters and valid name characters only.")]
    public string Name { get; set; } = "";

    [Required]
    [EmailAddress]
    [DefaultValue("user@example.com")]
    public string Email { get; set; } = "";
}