using System.ComponentModel.DataAnnotations;

namespace LearningApi.DTOs;

public class EnrollmentCreateDto
{
    [Required]
    public int StudentID { get; set; }

    [Required]
    public int CourseID { get; set; }
}