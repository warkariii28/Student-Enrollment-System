using System.ComponentModel.DataAnnotations;

namespace LearningApi.DTOs;

public class StudentUpdateDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = "";

    [Required]
    [EmailAddress]
    public string Email { get; set; }= "";
}