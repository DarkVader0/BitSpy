namespace BitSpy.Api.Models;

public sealed class TraceDomain
{
    public required string Name { get; init; }
    public required DateTime StartTime { get; init; }
    public required DateTime EndTime { get; init; }
    public required string IpAddress { get; init; }
    public List<AttributeDomain> Attributes { get; init; } = new();
    public List<EventDomain> Events { get; init; } = new();
}