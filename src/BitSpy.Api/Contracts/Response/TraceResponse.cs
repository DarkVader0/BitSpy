using BitSpy.Api.Contracts.Request;

namespace BitSpy.Api.Contracts.Response;

public sealed class TraceResponse
{
    public required string Name { get; init; }
    public required DateTime StartTime { get; init; }
    public required DateTime EndTime { get; init; }
    public List<AttributeRequest> Attributes { get; init; } = new();
    public List<EventRequest> Events { get; init; } = new();
}