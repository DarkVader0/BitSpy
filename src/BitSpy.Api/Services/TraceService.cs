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
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<TraceDomain>> GetTracesAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<TraceDomain> GetTraceAsync(string id)
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