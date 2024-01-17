using BitSpy.Api.Models;
using BitSpy.Api.Repositories;

namespace BitSpy.Api.Services;

public sealed class LogService : ILogService
{
    private readonly ILogRepository _logRepository;

    public LogService(ILogRepository logRepository)
    {
        _logRepository = logRepository;
    }
    
    public async Task<bool> SaveAsync(LogDomain log)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<LogDomain>> GetLogsAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<LogDomain> GetLogAsync(string id)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> UpdateAsync(LogDomain log)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteAsync(string id)
    {
        throw new NotImplementedException();
    }
}