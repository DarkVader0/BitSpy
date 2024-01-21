using System.Runtime.InteropServices.JavaScript;
using BitSpy.Api.Contracts.Request;
using BitSpy.Api.Endpoints.Internal;
using BitSpy.Api.Mappers;
using BitSpy.Api.Repositories;
using BitSpy.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace BitSpy.Api.Endpoints;

//TODO: handle returns better
public class MetricEndpoints : IEndpoint
{
    private const string BaseRoute = "/metrics";
    public static void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost(BaseRoute, AddMetric);
        app.MapGet(BaseRoute, GetMetric);
        app.MapGet(BaseRoute + "/all", GetAll);
    }

    private static async Task<IResult> AddMetric(
        [FromBody]MetricRequest metricRequest,
        IMetricService metricService)
    {
        var added = await metricService.SaveAsync(metricRequest.ToDomain());

        return added ? Results.Created() : Results.Problem();
    }

    private static async Task<IResult> GetMetric(
        string name,
        decimal cpuUsage,
        DateTime timeStamp,
        IMetricService metricService)
    {
        var metric = await metricService.GetMetricAsync(name, cpuUsage, timeStamp);

        return metric is null ?
            Results.NotFound() :
            Results.Ok(metric);
    }

    private static async Task<IResult> GetAll(
        DateTime startingTimeStamp,
        DateTime endingTimeStamp,
        IMetricService metricService)
    {
        var result =
            await metricService.GetMetricsAsync(startingTimeStamp, endingTimeStamp);
        return Results.Ok(result);
    }
}