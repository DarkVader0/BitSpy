using BitSpy.Api.Contracts.Database;
using BitSpy.Api.Contracts.Database.Relationships;
using BitSpy.Api.Models;
using BitSpy.Api.Repositories;

namespace BitSpy.Api.Services;

public sealed class TraceService : ITraceService
{
    private readonly ITraceRepository _traceRepository;

    public TraceService(ITraceRepository traceRepository)
    {
        _traceRepository = traceRepository;
    }

    public async Task<bool> SaveAsync(TraceDomain trace)
    {
        var (existingIp, existingRelationship, existingTrace) =
            await _traceRepository.GetIpAndTraceAsync(trace.IpAddress, trace.Name);

        var transaction = await _traceRepository.BeginTransactionAsync();
        
        if (existingIp is null)
        {
            existingIp = await _traceRepository.CreateIpAsync(new IpUserContract
            {
                IpAddress = trace.IpAddress
            });
        }

        if (existingTrace is null)
        {
            existingTrace = await _traceRepository.CreateTraceAsync(new TraceContract
            {
                Name = trace.Name,
                Attributes = trace.Attributes,
                AverageDuration = (trace.EndTime - trace.StartTime).Milliseconds,
                TraceCounter = 1
            });
        }
        else
        {
            existingTrace.AverageDuration =
                (existingTrace.AverageDuration * existingTrace.TraceCounter +
                 (trace.EndTime - trace.StartTime).Milliseconds) / ++existingTrace.TraceCounter;
            
            await _traceRepository.UpdateAsync(existingTrace);
        }

        if (existingRelationship is null)
        {
            await _traceRepository
                .CreateIpTraceRelationshipAsync(existingIp, existingTrace);
        }
        else
        {
            existingRelationship.RequestCounter++;
            existingRelationship.RequestIds.Add(await _traceRepository.SaveToCassandraAsync(trace));
            await _traceRepository.UpdateAsync(existingRelationship);
        }

        foreach(var eventToUpdate in existingTrace.Events)
        {
             await _traceRepository.UpdateRelationshipAsync(eventToUpdate.Name, existingTrace.Name);
        }
        
        var existingEvents = await _traceRepository
            .GetEventsAsync(trace.Events.Select(x => x.Name).ToList());
        
        var eventsToAdd = trace.Events
            .Where(x => existingEvents.All(y => y.Name != x.Name));
        
        foreach (var eventToAdd in eventsToAdd)
        {
            await _traceRepository.AddEventWithRelationshipAsync(existingTrace, eventToAdd);
        }
        
        await transaction!.CommitAsync();
        
        return true;
    }

    public async Task<IEnumerable<TraceDomain>> GetTracesAsync(string name)
    {
        return await _traceRepository.GetTracesAsync(name);
    }

    public async Task<TraceDomain?> GetTraceAsync(string name)
    {
        return await _traceRepository.GetTraceAsync(name);
    }

    public async Task<bool> UpdateAsync(TraceDomain trace)
    {
        var existingTrace = await _traceRepository.GetTraceAsync(trace.Name);
        if (existingTrace is null)
            return false;

        return await _traceRepository.UpdateAsync(trace);
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var existingTrace = await _traceRepository.GetTraceAsync(id);
        if (existingTrace is null)
            return false;

        return await _traceRepository.DeleteAsync(id);
    }
}