using BitSpy.Api.Models;
using Cassandra;
using ISession = Cassandra.ISession;

namespace BitSpy.Api.Repositories;

public class LogRepository : ILogRepository
{
    private readonly ISession _session;

    public LogRepository(IConfiguration configuration)
    {
        var cluster = Cluster.Builder()
            .AddContactPoint(configuration["Cassandra:Host"]!)
            .WithPort(int.Parse(configuration["Cassandra:Port"]!))
            .Build();

        _session = cluster.Connect(configuration["Cassandra:Keyspace"]);
    }

    public async Task<bool> SaveAsync(LogDomain log)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<LogDomain>> GetLogsAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<LogDomain> GetLogAsync(string id)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> UpdateAsync(LogDomain log)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteAsync(string id)
    {
        throw new NotImplementedException();
    }
}