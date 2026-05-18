using System.ComponentModel.DataAnnotations;

namespace LearningApi.DTOs;

public class StudentUpdateDto
{
    [Required]
    [StringLength(100)]
    [RegularExpression(@"^[A-Za-z][A-Za-z\s.'-]*$", ErrorMessage = "Name must contain letters and valid name characters only.")]
    public string Name { get; set; } = "";

    [Required]
    [EmailAddress]
    public string Email { get; set; } = "";
}