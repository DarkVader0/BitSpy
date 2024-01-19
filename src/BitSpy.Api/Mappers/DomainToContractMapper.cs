using BitSpy.Api.Contracts.Response;
using BitSpy.Api.Models;

namespace BitSpy.Api.Mappers;

public static class DomainToContractMapper
{
    public static LogResponse ToContract(this LogDomain domain)
        => new LogResponse
        {
            Level = domain.Level,
            LogTemplate = domain.LogTemplate,
            LogValues = domain.LogValues,
            Timestamp = domain.Timestamp
        };

    public static MetricResponse ToContract(this MetricDomain domain)
        => new MetricResponse
        {
            Name = domain.Name,
            TimeInGCSinceLastGCPercentage = domain.TimeInGCSinceLastGCPercentage,
            AllocationRatePerSecond = domain.AllocationRatePerSecond,
            CPUUsage = domain.CPUUsage,
            ExceptionCount = domain.ExceptionCount,
            Gen0CollectionCount = domain.Gen0CollectionCount,
            Gen0Size = domain.Gen0Size,
            Gen1CollectionCount = domain.Gen1CollectionCount,
            Gen1Size = domain.Gen1Size,
            Gen2CollectionCount = domain.Gen2CollectionCount,
            Gen2Size = domain.Gen2Size,
            ThreadPoolCompletedItemsCount = domain.ThreadPoolCompletedItemsCount,
            ThreadPoolQueueLength = domain.ThreadPoolQueueLength,
            ThreadPoolThreadCount = domain.ThreadPoolThreadCount,
            WorkingSet = domain.WorkingSet,
            Timestamp = domain.Timestamp
        };
}