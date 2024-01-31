using BitSpy.Api.Models;

namespace BitSpy.Api.Repositories;

public sealed class LongTermTraceRepository : ILongTermTraceRepository
{
    public async Task<string> SaveAsync(TraceDomain trace)
    {
        // TODO: Implement
        return string.Empty;
    }
}