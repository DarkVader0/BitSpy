using BitSpy.Api.Models;

namespace BitSpy.Api.Repositories;

public interface ITraceRepository
{
    Task<IEnumerable<TraceDomain>> GetTracesAsync();
    Task<TraceDomain> GetTraceAsync(string id);
    Task<bool> SaveAsync(TraceDomain trace);
    Task<bool> UpdateAsync(TraceDomain trace);
    Task<bool> DeleteAsync(string id);
}