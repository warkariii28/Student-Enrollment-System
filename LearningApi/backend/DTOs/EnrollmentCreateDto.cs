using System.ComponentModel.DataAnnotations;

namespace LearningApi.DTOs;

public class EnrollmentCreateDto
{
    [Required]
    [Range(1, int.MaxValue)]
    public int StudentID { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int CourseID { get; set; }
}