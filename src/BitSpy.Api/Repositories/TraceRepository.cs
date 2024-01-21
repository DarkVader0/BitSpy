using System.Globalization;
using System.Text.Json;
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

    public async Task<IEnumerable<TraceDomain>> GetTracesAsync(string startingMethod)
    {
        var session = _driver.AsyncSession();
        var result = await session.RunAsync($"MATCH (t:Trace) WHERE t.Name STARTS WITH '{startingMethod}' RETURN t");
        var records = await result.ToListAsync();
        return records.Select(r => new TraceDomain
        {
            Name = r["t"].As<INode>().Properties["Name"].As<string>(),
            StartTime = DateTime.Parse(r["t"].As<INode>().Properties["StartTime"].As<ZonedDateTime>().ToString()),
            EndTime = DateTime.Parse(r["t"].As<INode>().Properties["EndTime"].As<ZonedDateTime>().ToString()),
            Attributes = JsonSerializer.Deserialize<List<AttributeDomain>>(r["t"].As<INode>().Properties["Attributes"].As<string>())!,
            Events = JsonSerializer.Deserialize<List<EventDomain>>(r["t"].As<INode>().Properties["Events"].As<string>())!
        });
    }

    public async Task<TraceDomain?> GetTraceAsync(string name)
    {
        var session = _driver.AsyncSession();
        var result = await session.RunAsync($"MATCH (t:Trace) WHERE t.Name = '{name}' RETURN t");
        var r = (await result.ToListAsync()).FirstOrDefault();
        if (r is null)
            return null;

        return new TraceDomain
        {
            Name = r["t"].As<INode>().Properties["Name"].As<string>(),
            StartTime = DateTime.Parse(r["t"].As<INode>().Properties["StartTime"].As<ZonedDateTime>().ToString()),
            EndTime = DateTime.Parse(r["t"].As<INode>().Properties["EndTime"].As<ZonedDateTime>().ToString()),
            Attributes = JsonSerializer.Deserialize<List<AttributeDomain>>(r["t"].As<INode>().Properties["Attributes"].As<string>())!,
            Events = JsonSerializer.Deserialize<List<EventDomain>>(r["t"].As<INode>().Properties["Events"].As<string>())!
        };
    }

    public async Task<bool> SaveAsync(TraceDomain trace)
    {
        var session = _driver.AsyncSession();
        var result = await session.RunAsync(
            "CREATE (t:Trace {Name: $name, StartTime: datetime($startTime), EndTime: datetime($endTime), Attributes: $attributes, Events: $events})",
            new Dictionary<string, object>
            {
                {"name", trace.Name},
                {"startTime", trace.StartTime.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz", CultureInfo.InvariantCulture)},
                {"endTime", trace.EndTime.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz", CultureInfo.InvariantCulture)},
                {"attributes", JsonSerializer.Serialize(trace.Attributes)},
                {"events", JsonSerializer.Serialize(trace.Events)}
            });

        return true;
    }

    public async Task<bool> UpdateAsync(TraceDomain trace)
    {
        var session = _driver.AsyncSession();
        var result = await session.RunAsync(
            "MATCH (t:Trace) WHERE t.Name = $name SET t = {Name: $name, StartTime: datetime($startTime), EndTime: datetime($endTime), Attributes: $attributes, Events: $events}",
            new Dictionary<string, object>
            {
                {"name", trace.Name},
                {"startTime", trace.StartTime.ToString("O")},
                {"endTime", trace.EndTime.ToString("O")},
                {"attributes", JsonSerializer.Serialize(trace.Attributes)},
                {"events", JsonSerializer.Serialize(trace.Events)}
            });
        
        return true;
    }

    public async Task<bool> DeleteAsync(string name)
    {
        var session = _driver.AsyncSession();
        var result = await session.RunAsync($"MATCH (t:Trace) WHERE t.Name = '{name}' DELETE t");
        return true;
    }
}