using System.Text.Json;
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

    public async Task<(IpUserContract?, IpUserTraceRelationship?, TraceContract?)> GetIpAndTraceAsync(string ipAddress,
        string traceName)
    {
        var session = _driver.AsyncSession();
        var result = await session.RunAsync(
            "OPTIONAL MATCH (t:TraceContract {Name: $traceName}) OPTIONAL MATCH (ip:IpUserContract {IpAddress: $ipAddress}) OPTIONAL MATCH (ip)-[r:IpUserTraceRelationship]->(t) RETURN ip, r, t",
            new Dictionary<string, object>
            {
                { "ipAddress", ipAddress },
                { "traceName", traceName }
            });

        var record = (await result.ToListAsync()).FirstOrDefault();

        if (record == null)
        {
            return (null, null, null);
        }

        var ipUserContractNode = record["ip"].As<INode>();
        var ipUserContract = ipUserContractNode is not null
            ? new IpUserContract
            {
                IpAddress = ipUserContractNode.Properties["IpAddress"].As<string>(),
            }
            : null;

        var traceContractNode = record["t"].As<INode>();
        var traceContract = traceContractNode is not null
            ? new TraceContract
            {
                Name = traceContractNode.Properties["Name"]
                    .As<string>(),
                AverageDuration = traceContractNode.Properties["AverageDuration"]
                    .As<decimal>(),
                Attributes = JsonSerializer.Deserialize<List<AttributeDomain>>(traceContractNode
                    .Properties["Attributes"]
                    .As<string>())!,
                TraceCounter = traceContractNode.Properties["TraceCounter"]
                    .As<long>()
            }
            : null;

        var ipUserTraceRelationshipNode = record["r"].As<IRelationship>();
        var ipUserTraceRelationship = ipUserTraceRelationshipNode is not null
            ? new IpUserTraceRelationship
            {
                RequestCounter = ipUserTraceRelationshipNode.Properties["RequestCounter"].As<long>(),
                RequestIds =
                    JsonSerializer.Deserialize<List<string>>(ipUserTraceRelationshipNode.Properties["RequestIds"]
                        .As<string>())!
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
                { "name", trace.Name },
                { "averageDuration", trace.AverageDuration },
                { "attributes", JsonSerializer.Serialize(trace.Attributes) },
                { "traceCounter", trace.TraceCounter }
            });

        var record = await result.SingleAsync();

        var traceContractNode = record["t"].As<INode>();
        var createdTraceContract = new TraceContract
        {
            Name = traceContractNode.Properties["Name"]
                .As<string>(),
            AverageDuration = traceContractNode.Properties["AverageDuration"]
                .As<decimal>(),
            Attributes = JsonSerializer.Deserialize<List<AttributeDomain>>(traceContractNode.Properties["Attributes"]
                .As<string>())!,
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
                { "ipAddress", ip.IpAddress },
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
                { "name", trace.Name },
                { "averageDuration", trace.AverageDuration },
                { "attributes", JsonSerializer.Serialize(trace.Attributes) },
                { "traceCounter", trace.TraceCounter }
            });
    }

    public async Task UpdateAsync(string ip,
        string traceName,
        IpUserTraceRelationship ipUserTraceRelationship)
    {
        var session = _driver.AsyncSession();
        await session.RunAsync(
            "MATCH (ip:IpUserContract {IpAddress: $ipAddress})-[r:IpUserTraceRelationship]->(t:TraceContract {Name: $traceName}) SET r.RequestCounter = $requestCounter, r.RequestIds = $requestIds",
            new Dictionary<string, object>
            {
                { "ipAddress", ip },
                { "traceName", traceName },
                { "requestCounter", ipUserTraceRelationship.RequestCounter },
                { "requestIds", JsonSerializer.Serialize(ipUserTraceRelationship.RequestIds) }
            });
    }

    public async Task CreateIpTraceRelationshipAsync(IpUserContract ip,
        TraceContract trace,
        string requestId)
    {
        var session = _driver.AsyncSession();
        await session.RunAsync(
            "MATCH (ip:IpUserContract {IpAddress: $ipAddress}), (t:TraceContract {Name: $traceName}) CREATE (ip)-[:IpUserTraceRelationship {RequestCounter: $requestCounter, RequestIds: $requestIds}]->(t)",
            new Dictionary<string, object>
            {
                { "ipAddress", ip.IpAddress },
                { "traceName", trace.Name },
                { "requestCounter", 1 },
                { "requestIds", JsonSerializer.Serialize(new List<string> { requestId }) }
            });
    }

    public async Task UpdateRelationshipAsync(string traceName, string eventName, TraceEventRelationship relationship)
    {
        var session = _driver.AsyncSession();
        await session.RunAsync(
            "MATCH (t:TraceContract {Name: $traceName})-[r:TraceEventRelationship]->(e:EventContract {Name: $eventName}) SET r.EventCounter = $eventCounter, r.EventAvgDuration = $eventAvgDuration",
            new Dictionary<string, object>
            {
                { "eventName", eventName },
                { "traceName", traceName },
                { "eventCounter", relationship.EventCounter },
                { "eventAvgDuration", relationship.EventAvgDuration }
            });
    }

    public async Task<IEnumerable<(EventContract, TraceEventRelationship?)>> GetEventsAsync(IEnumerable<string> names, string traceName)
    {
        var session = _driver.AsyncSession();
        var result = await session.RunAsync(
            "UNWIND $names AS name MATCH (e:EventContract {Name: name}) OPTIONAL MATCH (t:TraceContract {Name: $traceName})-[r:TraceEventRelationship]->(e:EventContract {Name: name}) RETURN e, r",
            new Dictionary<string, object>
            {
                { "names", names },
                { "traceName", traceName }
            });

        var records = await result.ToListAsync();

        var eventContracts = new List<(EventContract, TraceEventRelationship?)>();
        foreach (var record in records)
        {
            var eventContractNode = record["e"].As<INode>();
            var eventContract = new EventContract
            {
                Name = eventContractNode.Properties["Name"].As<string>(),
                Message = eventContractNode.Properties["Message"].As<string>(),
                Duration = eventContractNode.Properties["Duration"].As<decimal>(),
                Attributes =
                    JsonSerializer.Deserialize<List<AttributeDomain>>(eventContractNode.Properties["Attributes"]
                        .As<string>())!
            };

            var relationshipNode = record["r"].As<IRelationship>();
            TraceEventRelationship? relationship = null;
            if (relationshipNode != null)
            {
                relationship = new TraceEventRelationship
                {
                    EventCounter = relationshipNode.Properties["EventCounter"].As<long>(),
                    EventAvgDuration = relationshipNode.Properties["EventAvgDuration"].As<decimal>()
                };
            }

            eventContracts.Add((eventContract, relationship));
        }

        return eventContracts;
    }

    public async Task AddEventWithRelationshipAsync(TraceContract trace, EventDomain eventContract)
    {
        var session = _driver.AsyncSession();
        var result = await session.RunAsync(
            "CREATE (e:EventContract {Name: $eventName, Duration: $duration, Message: $message, Attributes: $attributes}) WITH e MATCH (t:TraceContract {Name: $traceName}) CREATE (t)-[:TraceEventRelationship {EventCounter: $eventCounter, EventAvgDuration: $eventAvgDuration}]->(e)",
            new Dictionary<string, object>
            {
                { "eventName", eventContract.Name },
                { "message", eventContract.Message },
                { "duration", eventContract.Duration },
                { "attributes", JsonSerializer.Serialize(eventContract.Attributes) },
                { "traceName", trace.Name },
                { "eventCounter", 1 },
                { "eventAvgDuration", eventContract.Duration }
            });
    }

    public async Task AddRelationshipAsync(EventContract traceEventRelationship, TraceContract traceContract)
    {
        var session = _driver.AsyncSession();
        await session.RunAsync(
            "MATCH (e:EventContract {Name: $eventName}), (t:TraceContract {Name: $traceName}) CREATE (t)-[:TraceEventRelationship {EventCounter: $eventCounter, EventAvgDuration: $eventAvgDuration}]->(e)",
            new Dictionary<string, object>
            {
                { "eventName", traceEventRelationship.Name },
                { "traceName", traceContract.Name },
                { "eventCounter", 1 },
                { "eventAvgDuration", traceEventRelationship.Duration }
            });
    }

    public async Task<EventContract> CreateEventAsync(EventContract eventContract)
    {
        var session = _driver.AsyncSession();
        var result = await session.RunAsync(
            "CREATE (e:EventContract {Name: $name, Message: $message, Duration: $duration, Attributes: $attributes}) RETURN e",
            new Dictionary<string, object>
            {
                { "name", eventContract.Name },
                { "message", eventContract.Message },
                { "duration", eventContract.Duration },
                { "attributes", eventContract.Attributes }
            });

        var record = await result.SingleAsync();

        var eventContractNode = record["e"].As<INode>();
        var createdEventContract = new EventContract
        {
            Name = eventContractNode.Properties["Name"].As<string>(),
            Message = eventContractNode.Properties["Message"].As<string>(),
            Duration = eventContractNode.Properties["Duration"].As<decimal>(),
            Attributes =
                JsonSerializer.Deserialize<List<AttributeDomain>>(eventContractNode.Properties["Attributes"]
                    .As<string>())!
        };

        return createdEventContract;
    }
}