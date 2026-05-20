namespace BrightPath.Constants;

public static class AppRoles
{
    public const string Admin = "Admin";
    public const string User = "User";

    public static readonly HashSet<string> All =
        new(StringComparer.OrdinalIgnoreCase)
        {
            Admin,
            User
        };
}