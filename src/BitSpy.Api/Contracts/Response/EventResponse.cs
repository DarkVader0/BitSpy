namespace BitSpy.Api.Contracts.Response;

public sealed class EventResponse
{
    public required string Name { get; init; }
    public required string Message { get; init; }
    public required DateTime Timestamp { get; init; }
    public required List<AttributeResponse> Attributes { get; init; } = new();
}