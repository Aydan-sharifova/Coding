namespace Coding.Infrastructure.Authentication;

public static class SystemRoles
{
    public const string Admin = "Admin";
    public const string Developer = "Developer";
    public const string Guest = "Guest";

    public static readonly string[] All = [Admin, Developer, Guest];
}
