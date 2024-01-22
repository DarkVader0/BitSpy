namespace BitSpy.Api.Models;

public class MetricDomain
{
    public required string Name { get; init; }
    public required decimal TimeInGCSinceLastGCPercentage { get; init; }
    public required long AllocationRatePerSecond { get; init; }
    public required decimal CPUUsage { get; init; }
    public required long ExceptionCount { get; init; }
    public required long Gen0CollectionCount { get; init; }
    public required long Gen0Size { get; init; }
    public required long Gen1CollectionCount { get; init; }
    public required long Gen1Size { get; init; }
    public required long Gen2CollectionCount { get; init; }
    public required long Gen2Size { get; init; }
    public required long ThreadPoolCompletedItemsCount { get; init; }
    public required long ThreadPoolQueueLength { get; init; }
    public required long ThreadPoolThreadCount { get; init; }
    public required long WorkingSet { get; init; }
    public required DateTime Timestamp { get; init; }
}