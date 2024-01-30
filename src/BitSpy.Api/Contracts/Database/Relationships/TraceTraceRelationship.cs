namespace BitSpy.Api.Contracts.Database.Relationships;

public sealed class TraceTraceRelationship
{
    public List<string> SameEvents { get; init; } = new();
}