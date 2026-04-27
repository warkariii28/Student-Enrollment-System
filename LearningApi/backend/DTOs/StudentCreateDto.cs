using System.ComponentModel.DataAnnotations;

namespace LearningApi.DTOs;

public class StudentCreateDto
{
    [Required]
    public string Name { get; set; } = "";

    [Required]
    [EmailAddress]
    public string Email { get; set; } = "" ;
}