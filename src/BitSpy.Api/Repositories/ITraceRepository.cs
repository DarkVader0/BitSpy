using BitSpy.Api.Contracts.Database;
using BitSpy.Api.Contracts.Database.Relationships;
using BitSpy.Api.Models;
using Neo4j.Driver;

namespace BitSpy.Api.Repositories;

public interface ITraceRepository
{
    Task<(IpUserContract?, IpUserTraceRelationship?, TraceContract?)> GetIpAndTraceAsync(string ipAddress, string traceName);
    Task<TraceContract> CreateTraceAsync(TraceContract trace);
    Task<IAsyncTransaction?> BeginTransactionAsync();
    Task<IpUserContract> CreateIpAsync(IpUserContract ip);
    Task UpdateAsync(TraceContract trace);
    Task UpdateAsync(IpUserTraceRelationship ipUserTraceRelationship);
    Task CreateIpTraceRelationshipAsync(IpUserContract ip, TraceContract trace);
    Task UpdateRelationshipAsync(string eventName, string traceName);
    Task<IEnumerable<EventContract>> GetEventsAsync(IEnumerable<string> names);
    Task AddEventWithRelationshipAsync(TraceContract trace, EventDomain eventContract);
}