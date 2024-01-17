using BitSpy.Api.Models;

namespace BitSpy.Api.Repositories;

public interface ILogRepository
{
    Task<bool> SaveAsync(LogDomain log);
    Task<IEnumerable<LogDomain>> GetLogsAsync();
    Task<LogDomain> GetLogAsync(string id);
    Task<bool> UpdateAsync(LogDomain log);
    Task<bool> DeleteAsync(string id);
}