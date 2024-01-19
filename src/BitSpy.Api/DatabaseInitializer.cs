using Cassandra;
using ISession = Cassandra.ISession;

namespace BitSpy.Api;

public static class DatabaseInitializer
{
    private static ISession _session;

    public static async Task InitializeAsync(IConfiguration configuration)
    {
        var cluster = Cluster.Builder()
            .AddContactPoint(configuration["Cassandra:Host"]!)
            .WithPort(int.Parse(configuration["Cassandra:Port"]!))
            .Build();

        _session = await cluster.ConnectAsync(configuration["Cassandra:Keyspace"]);
        await CreateMetricsTableAsync();
        await CreateLogsTableAsync();
    }

    private static async Task CreateMetricsTableAsync()
    {
        var query = @"CREATE TABLE IF NOT EXISTS metrics (
                        name text,
                        timeInGCSinceLastGCPercentage decimal,
                        allocationRatePerSecond bigint,
                        cpuUsage decimal,
                        exceptionCount int,
                        gen0CollectionCount int,
                        gen0Size bigint,
                        gen1CollectionCount int,
                        gen1Size bigint,
                        gen2CollectionCount int,
                        gen2Size bigint,
                        threadPoolCompletedItemsCount int,
                        threadPoolQueueLength int,
                        threadPoolThreadCount int,
                        workingSet int,
                        timestamp timestamp,
                        PRIMARY KEY (name, cpuUsage, timestamp)
                      );";
        await _session.ExecuteAsync(new SimpleStatement(query));
    }

    private static async Task CreateLogsTableAsync()
    {
        var query = @"CREATE TABLE IF NOT EXISTS logs (
                        level text,
                        logTemplate text,
                        logValues list<text>,
                        timestamp timestamp,
                        PRIMARY KEY (level, timestamp)
                      );";
        await _session.ExecuteAsync(new SimpleStatement(query));
    }
}