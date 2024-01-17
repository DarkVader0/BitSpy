using BitSpy.Api.Models;

namespace BitSpy.Api.Repositories;

public interface IMetricRepository
{
    Task<bool> SaveAsync(MetricDomain metric);
    Task<IEnumerable<MetricDomain>> GetMetricsAsync();
    Task<MetricDomain> GetMetricAsync(string id);
    Task<bool> UpdateAsync(MetricDomain metric);
    Task<bool> DeleteAsync(string id);
}