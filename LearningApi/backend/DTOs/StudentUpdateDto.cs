using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
namespace BrightPath.DTOs;

public class StudentUpdateDto
{
    [Required]
    [StringLength(100)]
    [DefaultValue("User Name")]
    [RegularExpression(@"^[A-Za-z][A-Za-z\s.'-]*$", ErrorMessage = "Name must contain letters and valid name characters only.")]
    public string Name { get; set; } = "";

    [Required]
    [EmailAddress]
    public string Email { get; set; } = "";
}