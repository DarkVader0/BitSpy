namespace BitSpy.Api.Contracts.Request;

public sealed class LogRequest
{
    public required string LogTemplate { get; set; }
    public required List<string> LogValues { get; set; }
}