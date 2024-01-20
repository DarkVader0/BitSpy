using BitSpy.Api.Models;

namespace BitSpy.Api.Services;

public interface ITraceService
{
    Task<bool> SaveAsync(TraceDomain trace);
    Task<IEnumerable<TraceDomain>> GetTracesAsync(string name);
    Task<TraceDomain?> GetTraceAsync(string name);
    Task<bool> UpdateAsync(TraceDomain trace);
    Task<bool> DeleteAsync(string id);
}