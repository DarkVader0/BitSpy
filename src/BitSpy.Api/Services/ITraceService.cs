using BitSpy.Api.Models;

namespace BitSpy.Api.Services;

public interface ITraceService
{
    Task<bool> SaveAsync(TraceDomain trace);
    Task<IEnumerable<TraceDomain>> GetTracesAsync();
    Task<TraceDomain> GetTraceAsync(string id);
    Task<bool> UpdateAsync(TraceDomain trace);
    Task<bool> DeleteAsync(string id);
}