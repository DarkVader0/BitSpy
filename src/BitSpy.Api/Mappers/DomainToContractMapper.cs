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
    
    public static TraceResponse ToContract(this TraceDomain domain)
        => new()
        {
            Name = domain.Name,
            StartTime = domain.StartTime,
            EndTime = domain.EndTime,
            Attributes = domain.Attributes.Select(x => x.ToContract()).ToList(),
            Events = domain.Events.Select(x => x.ToContract()).ToList()
        };

    public static AttributeResponse ToContract(this AttributeDomain domain)
        => new()
        {
            Name = domain.Name,
            Value = domain.Value
        };
    
    public static EventResponse ToContract(this EventDomain domain)
        => new()
        {
            Name = domain.Name,
            Message = domain.Message,
            Attributes = domain.Attributes.Select(x => x.ToContract())
                .ToList(),
            Timestamp = new DateTime((long)domain.Duration)
        };
    //TODO: fix this
}