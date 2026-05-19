using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
namespace LearningApi.DTOs;

public class EnrollmentCreateDto
{
    [Required]
    [Range(1, int.MaxValue)]
    [DefaultValue(1)]
    public int StudentID { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    [DefaultValue(1)]
    public int CourseID { get; set; }
}