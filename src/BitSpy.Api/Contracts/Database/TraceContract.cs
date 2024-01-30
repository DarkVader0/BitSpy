using BitSpy.Api.Models;

namespace BitSpy.Api.Contracts.Database;

public sealed class TraceContract
{
    public required string Name { get; init; }
    public required decimal AverageDuration { get; set; }
    public required long TraceCounter { get; set; }
    public List<AttributeDomain> Attributes { get; init; } = new();
    public List<EventContract> Events { get; init; } = new();
}