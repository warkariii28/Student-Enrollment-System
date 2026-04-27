namespace LearningApi.DTOs;

public class AuthResultDto
{
    public string Token { get; set; } = "";
    public int UserId { get; set; }
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
}
