using BitSpy.Api.Models;
using BitSpy.Api.Repositories;
using BitSpy.Api.Contracts.Database;

namespace BitSpy.Api.Services;

public sealed class TraceService : ITraceService
{
    private readonly ITraceRepository _traceRepository;
    private readonly ILongTermTraceRepository _longTermTraceRepository;

    public TraceService(ITraceRepository traceRepository,
        ILongTermTraceRepository longTermTraceRepository)
    {
        _traceRepository = traceRepository;
        _longTermTraceRepository = longTermTraceRepository;
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
                AverageDuration = (decimal)(trace.EndTime - trace.StartTime).TotalMilliseconds,
                TraceCounter = 1
            });
        }
        else
        {
            existingTrace.AverageDuration =
                (existingTrace.AverageDuration * existingTrace.TraceCounter +
                 (decimal)(trace.EndTime - trace.StartTime).TotalMilliseconds) / ++existingTrace.TraceCounter;

            await _traceRepository.UpdateAsync(existingTrace);
        }

        if (existingRelationship is null)
        {
            var longTermTraceId = await _longTermTraceRepository.SaveAsync(trace);
            await _traceRepository
                .CreateIpTraceRelationshipAsync(existingIp, existingTrace, longTermTraceId);
        }
        else
        {
            existingRelationship.RequestCounter++;
            existingRelationship.RequestIds.Add(await _longTermTraceRepository.SaveAsync(trace));
            await _traceRepository.UpdateAsync(existingIp.IpAddress, existingTrace.Name, existingRelationship);
        }

        var existingEvents = (await _traceRepository
            .GetEventsAsync(trace.Events.Select(x => x.Name), existingTrace.Name))
            .ToList();

        var eventsWithRelationships = existingEvents
            .Where(x => x.Item2 is not null);

        foreach (var eventWithRelationship in eventsWithRelationships)
        {
            eventWithRelationship.Item2!.EventAvgDuration =
                (eventWithRelationship.Item2!.EventAvgDuration * eventWithRelationship.Item2!.EventCounter +
                 eventWithRelationship.Item1.Duration) / ++eventWithRelationship.Item2!.EventCounter;

            await _traceRepository.UpdateRelationshipAsync(existingTrace.Name, eventWithRelationship.Item1.Name, eventWithRelationship.Item2!);
        }
        
        var eventsWithoutRelationships = existingEvents
            .Where(x => x.Item2 is null)
            .Select(x => x.Item1);

        foreach (var eventWithoutRelationship in eventsWithoutRelationships)
        {
            await _traceRepository.AddRelationshipAsync(eventWithoutRelationship, existingTrace);
        }

        var eventsToAdd = trace.Events
            .Where(x => existingEvents.All(y => y.Item1.Name != x.Name));

        foreach (var eventToAdd in eventsToAdd)
        {
            await _traceRepository.AddEventWithRelationshipAsync(existingTrace, eventToAdd);
        }

        await transaction!.CommitAsync();

        return true;
    }

    public async Task<IEnumerable<TraceDomain>> GetTracesAsync(string name)
    {
        throw new NotImplementedException();
    }

    public async Task<TraceDomain?> GetTraceAsync(string name)
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