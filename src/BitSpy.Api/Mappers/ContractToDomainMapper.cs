using BitSpy.Api.Contracts.Request;
using BitSpy.Api.Models;

namespace BitSpy.Api.Mappers;

public static class ContractToDomainMapper
{
    public static LogDomain ToDomain(this LogRequest request)
        => new()
        {
            Level = request.Level,
            Timestamp = request.Timestamp,
            LogTemplate = request.LogTemplate,
            LogValues = request.LogValues
        };
    
    public static MetricDomain ToDomain(this MetricRequest request)
        => new()
        {
            Name = request.Name,
            TimeInGCSinceLastGCPercentage = request.TimeInGCSinceLastGCPercentage,
            AllocationRatePerSecond = request.AllocationRatePerSecond,
            CPUUsage = request.CPUUsage,
            ExceptionCount = request.ExceptionCount,
            Gen0CollectionCount = request.Gen0CollectionCount,
            Gen0Size = request.Gen0Size,
            Gen1CollectionCount = request.Gen1CollectionCount,
            Gen1Size = request.Gen1Size,
            Gen2CollectionCount = request.Gen2CollectionCount,
            Gen2Size = request.Gen2Size,
            ThreadPoolCompletedItemsCount = request.ThreadPoolCompletedItemsCount,
            ThreadPoolQueueLength = request.ThreadPoolQueueLength,
            ThreadPoolThreadCount = request.ThreadPoolThreadCount,
            WorkingSet = request.WorkingSet,
            Timestamp = request.Timestamp
        };
}