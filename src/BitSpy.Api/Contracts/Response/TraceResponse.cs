namespace BitSpy.Api.Contracts.Response;

public sealed class TraceResponse
{
    public required string Name { get; init; }
    public required DateTime StartTime { get; init; }
    public required DateTime EndTime { get; init; }
    public List<AttributeResponse> Attributes { get; init; } = new();
    public List<EventResponse> Events { get; init; } = new();
}