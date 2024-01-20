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
        return await _traceRepository.SaveAsync(trace);
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