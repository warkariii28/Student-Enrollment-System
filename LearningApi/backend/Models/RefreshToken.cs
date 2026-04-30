public class RefreshToken
{
    public int UserId { get; set; }
    public string Token { get; set; } = "";
    public DateTime ExpiresAt { get; set; }
}