namespace OnlineStore.Api.Data;

/// <summary>
/// Minimal row storage used by integration tests to verify Respawn database reset (issue #6).
/// Not part of the domain model; safe to remove when real entities cover the same scenarios.
/// </summary>
public sealed class IntegrationResetProbe
{
    public Guid Id { get; set; }

    public required string Label { get; set; }
}
