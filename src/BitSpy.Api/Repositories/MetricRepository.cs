using BitSpy.Api.Models;
using Cassandra;
using ISession = Cassandra.ISession;

namespace BitSpy.Api.Repositories;

public class MetricRepository : IMetricRepository
{
    private readonly ISession _session;

    public MetricRepository(IConfiguration configuration)
    {
        var cluster = Cluster.Builder()
            .AddContactPoint(configuration["Cassandra:Host"]!)
            .WithPort(int.Parse(configuration["Cassandra:Port"]!))
            .Build();

        _session = cluster.Connect(configuration["Cassandra:Keyspace"]);
    }

    public async Task<bool> SaveAsync(MetricDomain metric)
    {
        var query = await _session.PrepareAsync(
            "INSERT INTO metrics (name, timeInGCSinceLastGCPercentage, allocationRatePerSecond, cpuUsage, exceptionCount, gen0CollectionCount, gen0Size, gen1CollectionCount, gen1Size, gen2CollectionCount, gen2Size, threadPoolCompletedItemsCount, threadPoolQueueLength, threadPoolThreadCount, workingSet, timestamp) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)");
        var bound = query.Bind(metric.Name, metric.TimeInGCSinceLastGCPercentage, metric.AllocationRatePerSecond,
            metric.CPUUsage, metric.ExceptionCount, metric.Gen0CollectionCount, metric.Gen0Size,
            metric.Gen1CollectionCount, metric.Gen1Size, metric.Gen2CollectionCount, metric.Gen2Size,
            metric.ThreadPoolCompletedItemsCount, metric.ThreadPoolQueueLength, metric.ThreadPoolThreadCount,
            metric.WorkingSet, metric.Timestamp);
        var result = await _session.ExecuteAsync(bound);
        return result.IsFullyFetched;
    }

    public async Task<IEnumerable<MetricDomain>> GetMetricsAsync(DateTime startingTimestamp, DateTime endingTimestamp)
    {
        var query = await _session.PrepareAsync("SELECT * FROM metrics WHERE timestamp >= ? AND timestamp <= ? ALLOW FILTERING");
        var bound = query.Bind(startingTimestamp, endingTimestamp);
        var result = await _session.ExecuteAsync(bound);
        return result.Select(row => new MetricDomain
        {
            Name = row.GetValue<string>("name"),
            TimeInGCSinceLastGCPercentage = row.GetValue<decimal>("timeInGCSinceLastGCPercentage".ToLower()),
            AllocationRatePerSecond = row.GetValue<long>("allocationRatePerSecond".ToLower()),
            CPUUsage = row.GetValue<decimal>("cpuUsage".ToLower()),
            ExceptionCount = row.GetValue<long>("exceptionCount".ToLower()),
            Gen0CollectionCount = row.GetValue<long>("gen0CollectionCount".ToLower()),
            Gen0Size = row.GetValue<long>("gen0Size".ToLower()),
            Gen1CollectionCount = row.GetValue<long>("gen1CollectionCount".ToLower()),
            Gen1Size = row.GetValue<long>("gen1Size".ToLower()),
            Gen2CollectionCount = row.GetValue<long>("gen2CollectionCount".ToLower()),
            Gen2Size = row.GetValue<long>("gen2Size".ToLower()),
            ThreadPoolCompletedItemsCount = row.GetValue<long>("threadPoolCompletedItemsCount".ToLower()),
            ThreadPoolQueueLength = row.GetValue<long>("threadPoolQueueLength".ToLower()),
            ThreadPoolThreadCount = row.GetValue<long>("threadPoolThreadCount".ToLower()),
            WorkingSet = row.GetValue<long>("workingSet".ToLower()),
            Timestamp = row.GetValue<DateTime>("timestamp".ToLower())
        });
    }

    public async Task<MetricDomain?> GetMetricAsync(string name,
        decimal cpuUsage,
        DateTime timestamp)
    {
        var query = await _session.PrepareAsync(
            "SELECT * FROM metrics WHERE name = ? AND cpuUsage = ? AND timestamp = ? ALLOW FILTERING");
        var bound = query.Bind(name, cpuUsage, timestamp);
        var result = await _session.ExecuteAsync(bound);
        var row = result.FirstOrDefault();
        if (row is null)
            return null;
        return new MetricDomain
        {
            Name = row.GetValue<string>("name"),
            TimeInGCSinceLastGCPercentage = row.GetValue<decimal>("timeInGCSinceLastGCPercentage".ToLower()),
            AllocationRatePerSecond = row.GetValue<long>("allocationRatePerSecond".ToLower()),
            CPUUsage = row.GetValue<decimal>("cpuUsage".ToLower()),
            ExceptionCount = row.GetValue<long>("exceptionCount".ToLower()),
            Gen0CollectionCount = row.GetValue<long>("gen0CollectionCount".ToLower()),
            Gen0Size = row.GetValue<long>("gen0Size".ToLower()),
            Gen1CollectionCount = row.GetValue<long>("gen1CollectionCount".ToLower()),
            Gen1Size = row.GetValue<long>("gen1Size".ToLower()),
            Gen2CollectionCount = row.GetValue<long>("gen2CollectionCount".ToLower()),
            Gen2Size = row.GetValue<long>("gen2Size".ToLower()),
            ThreadPoolCompletedItemsCount = row.GetValue<long>("threadPoolCompletedItemsCount".ToLower()),
            ThreadPoolQueueLength = row.GetValue<long>("threadPoolQueueLength".ToLower()),
            ThreadPoolThreadCount = row.GetValue<long>("threadPoolThreadCount".ToLower()),
            WorkingSet = row.GetValue<long>("workingSet".ToLower()),
            Timestamp = row.GetValue<DateTime>("timestamp".ToLower())
        };
    }

    public async Task<bool> UpdateAsync(MetricDomain metric)
    {
        var query = await _session.PrepareAsync(
            "UPDATE metrics SET timeInGCSinceLastGCPercentage = ?, allocationRatePerSecond = ?, cpuUsage = ?, exceptionCount = ?, gen0CollectionCount = ?, gen0Size = ?, gen1CollectionCount = ?, gen1Size = ?, gen2CollectionCount = ?, gen2Size = ?, threadPoolCompletedItemsCount = ?, threadPoolQueueLength = ?, threadPoolThreadCount = ?, workingSet = ?, timestamp = ? WHERE name = ?");
        var bound = query.Bind(metric.TimeInGCSinceLastGCPercentage, metric.AllocationRatePerSecond, metric.CPUUsage,
            metric.ExceptionCount, metric.Gen0CollectionCount, metric.Gen0Size, metric.Gen1CollectionCount,
            metric.Gen1Size, metric.Gen2CollectionCount, metric.Gen2Size, metric.ThreadPoolCompletedItemsCount,
            metric.ThreadPoolQueueLength, metric.ThreadPoolThreadCount, metric.WorkingSet, metric.Timestamp,
            metric.Name);
        var result = await _session.ExecuteAsync(bound);
        return result.IsFullyFetched;
    }

    public async Task<bool> DeleteAsync(string name,
        decimal cpuUsage,
        DateTime timestamp)
    {
        var query = await _session.PrepareAsync(
            "DELETE FROM metrics WHERE name = ? AND cpuUsage = ? AND timestamp = ?");
        var bound = query.Bind(name, cpuUsage, timestamp);
        var result = await _session.ExecuteAsync(bound);
        return result.IsFullyFetched;
    }
}