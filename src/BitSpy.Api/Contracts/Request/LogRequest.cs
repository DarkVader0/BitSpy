namespace BitSpy.Api.Contracts.Request;

public sealed class LogRequest
{
    public required string LogTemplate { get; init; }
    public required List<string> LogValues { get; init; }
}