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
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<MetricDomain>> GetMetricsAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<MetricDomain> GetMetricAsync(string id)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> UpdateAsync(MetricDomain metric)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteAsync(string id)
    {
        throw new NotImplementedException();
    }
}