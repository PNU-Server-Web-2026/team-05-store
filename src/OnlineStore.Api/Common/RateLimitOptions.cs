namespace OnlineStore.Api.Common;

/// <summary>
/// Ліміти fixed-window політики для анонімних запитів (ключ — IP).
/// </summary>
public sealed class RateLimitOptions
{
    public const string SectionName = "RateLimiting";
    public const string PolicyName = "fixed-by-ip";

    public int PermitLimit { get; set; } = 100;

    public int WindowSeconds { get; set; } = 60;
}
