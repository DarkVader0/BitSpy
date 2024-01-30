using BitSpy.Api.Models;

namespace BitSpy.Api.Contracts.Database;

public sealed class EventContract
{
    public required string Name { get; init; }
    public required string Message { get; init; }
    public required decimal Duration { get; init; }
    public required List<AttributeDomain> Attributes { get; init; } = new();
}