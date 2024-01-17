using BitSpy.Api.Models;
using BitSpy.Api.Repositories;

namespace BitSpy.Api.Services;

public sealed class MetricService : IMetricService
{
    private readonly IMetricRepository _metricRepository;

    public MetricService(IMetricRepository metricRepository)
    {
        _metricRepository = metricRepository;
    }
    
    public async Task<bool> SaveAsync(MetricDomain metric)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<MetricDomain>> GetMetricsAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<MetricDomain> GetMetricAsync(string id)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> UpdateAsync(MetricDomain metric)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteAsync(string id)
    {
        throw new NotImplementedException();
    }
}