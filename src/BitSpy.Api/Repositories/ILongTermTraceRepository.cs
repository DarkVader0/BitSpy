using BitSpy.Api.Contracts.Database;
using BitSpy.Api.Models;

namespace BitSpy.Api.Repositories;

public interface ILongTermTraceRepository
{
    Task<string> SaveAsync(TraceDomain trace);
}