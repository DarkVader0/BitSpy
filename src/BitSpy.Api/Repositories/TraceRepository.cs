using BitSpy.Api.Models;
using Neo4j.Driver;

namespace BitSpy.Api.Repositories;

public class TraceRepository : ITraceRepository
{
    private readonly IDriver _driver;

    public TraceRepository(IConfiguration configuration)
    {
        _driver = GraphDatabase
            .Driver(configuration["Neo4j:Uri"],
            AuthTokens.Basic(configuration["Neo4j:Username"], 
                configuration["Neo4j:Password"]));
    }

    public async Task<IEnumerable<TraceDomain>> GetTracesAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<TraceDomain> GetTraceAsync(string id)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> SaveAsync(TraceDomain trace)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> UpdateAsync(TraceDomain trace)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteAsync(string id)
    {
        throw new NotImplementedException();
    }
}