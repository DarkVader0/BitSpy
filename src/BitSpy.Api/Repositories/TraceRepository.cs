using BitSpy.Api.Contracts.Database;
using BitSpy.Api.Contracts.Database.Relationships;
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
    public async Task<(IpUserContract?, IpUserTraceRelationship?, TraceContract?)> GetIpAndTraceAsync(string ipAddress, string traceName)
    {
        var session = _driver.AsyncSession();
        var result = await session.RunAsync(
            "MATCH (ip:IpUserContract {IpAddress: $ipAddress}) OPTIONAL MATCH (ip)-[r:IpUserTraceRelationship]->(t:TraceContract {Name: $traceName}) RETURN ip, r, t",
            new Dictionary<string, object>
            {
                {"ipAddress", ipAddress},
                {"traceName", traceName}
            });

        var record = await result.SingleAsync();

        if (record == null)
        {
            return (null, null, null);
        }

        var ipUserContractNode = record["ip"].As<INode>();
        var ipUserContract = ipUserContractNode is not null ? new IpUserContract
        {
            IpAddress = ipUserContractNode.Properties["IpAddress"].As<string>(),
        } : null;

        var traceContractNode = record["t"].As<INode>();
        var traceContract = traceContractNode is not null ? new TraceContract
            {
                Name = traceContractNode.Properties["Name"]
                    .As<string>(),
                AverageDuration = traceContractNode.Properties["AverageDuration"]
                    .As<long>(),
                Attributes = traceContractNode.Properties["Attributes"]
                    .As<List<AttributeDomain>>(),
                TraceCounter = traceContractNode.Properties["TraceCounter"]
                    .As<long>()
            }
            : null;

        var ipUserTraceRelationshipNode = record["r"].As<IRelationship>();
        var ipUserTraceRelationship = ipUserTraceRelationshipNode is not null ? new IpUserTraceRelationship
            {
                RequestCounter = ipUserTraceRelationshipNode.Properties["RequestCounter"].As<long>(),
                RequestIds = ipUserTraceRelationshipNode.Properties["RequestIds"].As<List<string>>()
            }
            : null;

        return (ipUserContract, ipUserTraceRelationship, traceContract);
    }

    public async Task<TraceContract> CreateTraceAsync(TraceContract trace)
    {
        var session = _driver.AsyncSession();
        var result = await session.RunAsync(
            "CREATE (t:TraceContract {Name: $name, AverageDuration: $averageDuration, Attributes: $attributes, TraceCounter: $traceCounter}) RETURN t",
            new Dictionary<string, object>
            {
                {"name", trace.Name},
                {"averageDuration", trace.AverageDuration},
                {"attributes", trace.Attributes},
                {"traceCounter", trace.TraceCounter}
            });

        var record = await result.SingleAsync();

        var traceContractNode = record["t"].As<INode>();
        var createdTraceContract = new TraceContract
        {
            Name = traceContractNode.Properties["Name"]
                .As<string>(),
            AverageDuration = traceContractNode.Properties["AverageDuration"]
                .As<long>(),
            Attributes = traceContractNode.Properties["Attributes"]
                .As<List<AttributeDomain>>(),
            TraceCounter = 0
        };

        return createdTraceContract;
    }

    public async Task<IAsyncTransaction?> BeginTransactionAsync()
    {
        var session = _driver.AsyncSession();
        return await session.BeginTransactionAsync();
    }

    public async Task<IpUserContract> CreateIpAsync(IpUserContract ip)
    {
        var session = _driver.AsyncSession();
        var result = await session.RunAsync(
            "CREATE (ip:IpUserContract {IpAddress: $ipAddress}) RETURN ip",
            new Dictionary<string, object>
            {
                {"ipAddress", ip.IpAddress},
            });

        var record = await result.SingleAsync();

        var ipUserContractNode = record["ip"].As<INode>();
        var createdIpUserContract = new IpUserContract
        {
            IpAddress = ipUserContractNode.Properties["IpAddress"].As<string>(),
        };

        return createdIpUserContract;
    }

    public async Task UpdateAsync(TraceContract trace)
    {
        var session = _driver.AsyncSession();
        await session.RunAsync(
            "MATCH (t:TraceContract {Name: $name}) SET t.AverageDuration = $averageDuration, t.Attributes = $attributes, t.TraceCounter = $traceCounter",
            new Dictionary<string, object>
            {
                {"name", trace.Name},
                {"averageDuration", trace.AverageDuration},
                {"attributes", trace.Attributes},
                {"traceCounter", trace.TraceCounter}
            });
    }

    public async Task UpdateAsync(IpUserTraceRelationship ipUserTraceRelationship)
    {
        var session = _driver.AsyncSession();
        await session.RunAsync(
            "MATCH (ip:IpUserContract {IpAddress: $ipAddress})-[r:IpUserTraceRelationship]->(t:TraceContract {Name: $traceName}) SET r.RequestCounter = $requestCounter, r.RequestIds = $requestIds",
            new Dictionary<string, object>
            {
                {"requestCounter", ipUserTraceRelationship.RequestCounter},
                {"requestIds", ipUserTraceRelationship.RequestIds}
            });
    }

    public async Task CreateIpTraceRelationshipAsync(IpUserContract ip, TraceContract trace)
    {
        var session = _driver.AsyncSession();
        await session.RunAsync(
            "MATCH (ip:IpUserContract {IpAddress: $ipAddress}), (t:TraceContract {Name: $traceName}) CREATE (ip)-[:IpUserTraceRelationship]->(t)",
            new Dictionary<string, object>
            {
                {"ipAddress", ip.IpAddress},
                {"traceName", trace.Name}
            });
    }

    public async Task UpdateRelationshipAsync(string eventName, string traceName)
    {
        var session = _driver.AsyncSession();
        await session.RunAsync(
            "MATCH (e:Event {Name: $eventName})-[r:TraceEventRelationship]->(t:Trace {Name: $traceName}) SET r.EventCounter = r.EventCounter + 1, r.EventAvgDuration = (r.EventAvgDuration * r.EventCounter + t.AverageDuration) / (r.EventCounter + 1)",
            new Dictionary<string, object>
            {
                {"eventName", eventName},
                {"traceName", traceName}
            });
    }

    public async Task<IEnumerable<EventContract>> GetEventsAsync(IEnumerable<string> names)
    {
        var session = _driver.AsyncSession();
        var result = await session.RunAsync(
            "UNWIND $names AS name MATCH (e:EventContract {Name: name}) RETURN e",
            new Dictionary<string, object>
            {
                {"names", names}
            });

        var records = await result.ToListAsync();

        var eventContracts = new List<EventContract>();
        foreach (var record in records)
        {
            var eventContractNode = record["e"].As<INode>();
            var eventContract = new EventContract
            {
                Name = eventContractNode.Properties["Name"].As<string>(),
                Message = eventContractNode.Properties["Message"].As<string>(),
                Duration = eventContractNode.Properties["Duration"].As<decimal>(),
                Attributes = eventContractNode.Properties["Attributes"].As<List<AttributeDomain>>()
            };
            eventContracts.Add(eventContract);
        }

        return eventContracts;
    }

    public async Task AddEventWithRelationshipAsync(TraceContract trace, EventDomain eventContract)
    {
        var session = _driver.AsyncSession();
        var result = await session.RunAsync(
            "CREATE (e:EventContract {Name: $eventName, Duration: $duration}) WITH e MATCH (t:TraceContract {Name: $traceName}) CREATE (e)-[:EventTraceRelationship]->(t)",
            new Dictionary<string, object>
            {
                {"eventName", eventContract.Name},
                {"duration", eventContract.Duration},
                {"traceName", trace.Name}
            });
    }

    public async Task<EventContract> CreateEventAsync(EventContract eventContract)
    {
        var session = _driver.AsyncSession();
        var result = await session.RunAsync(
            "CREATE (e:EventContract {Name: $name, Message: $message, Duration: $duration, Attributes: $attributes}) RETURN e",
            new Dictionary<string, object>
            {
                {"name", eventContract.Name},
                {"message", eventContract.Message},
                {"duration", eventContract.Duration},
                {"attributes", eventContract.Attributes}
            });

        var record = await result.SingleAsync();

        var eventContractNode = record["e"].As<INode>();
        var createdEventContract = new EventContract
        {
            Name = eventContractNode.Properties["Name"].As<string>(),
            Message = eventContractNode.Properties["Message"].As<string>(),
            Duration = eventContractNode.Properties["Duration"].As<decimal>(),
            Attributes = eventContractNode.Properties["Attributes"].As<List<AttributeDomain>>()
        };

        return createdEventContract;
    }
}