namespace BitSpy.Api.Contracts.Request;

public sealed class MetricRequest
{
    public required string Name { get; init; }
    public required decimal TimeInGCSinceLastGCPercentage { get; init; }
    public required ulong AllocationRatePerSecond { get; init; }
    public required decimal CPUUsage { get; init; }
    public required uint ExceptionCount { get; init; }
    public required uint Gen0CollectionCount { get; init; }
    public required ulong Gen0Size { get; init; }
    public required uint Gen1CollectionCount { get; init; }
    public required ulong Gen1Size { get; init; }
    public required uint Gen2CollectionCount { get; init; }
    public required ulong Gen2Size { get; init; }
    public required uint ThreadPoolCompletedItemsCount { get; init; }
    public required uint ThreadPoolQueueLength { get; init; }
    public required uint ThreadPoolThreadCount { get; init; }
    public required uint WorkingSet { get; init; }
}