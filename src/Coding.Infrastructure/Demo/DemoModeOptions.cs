namespace Coding.Infrastructure.Demo;

public sealed class DemoModeOptions
{
    public const string SectionName = "DemoMode";

    public bool Enabled { get; init; }
    public string DatabaseNameMarker { get; init; } = "Demo";
    public int AccessTokenMinutes { get; init; } = 20;
    public int RefreshTokenHours { get; init; } = 2;
    public int ResetIntervalMinutes { get; init; } = 60;
    public int MaxUploadBytes { get; init; } = 1_048_576;
    public string[] BlockedFileExtensions { get; init; } =
    [
        ".app", ".bat", ".cmd", ".com", ".dll", ".dmg", ".exe", ".jar",
        ".msi", ".pkg", ".ps1", ".scr", ".vbs"
    ];
}

public static class DemoDataIds
{
    public static readonly Guid OwnerUserId =
        Guid.Parse("de000001-0000-4000-8000-000000000001");
    public static readonly Guid AdminUserId =
        Guid.Parse("de000001-0000-4000-8000-000000000002");
    public static readonly Guid MemberUserId =
        Guid.Parse("de000001-0000-4000-8000-000000000003");
    public static readonly Guid ProjectId =
        Guid.Parse("de000002-0000-4000-8000-000000000001");
    public static readonly Guid ConversationId =
        Guid.Parse("de000003-0000-4000-8000-000000000001");

    public static readonly Guid[] UserIds =
    [
        OwnerUserId,
        AdminUserId,
        MemberUserId
    ];
}
