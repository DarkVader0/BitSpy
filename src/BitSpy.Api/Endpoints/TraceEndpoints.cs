using BitSpy.Api.Contracts.Request;
using BitSpy.Api.Endpoints.Internal;
using BitSpy.Api.Mappers;
using BitSpy.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace BitSpy.Api.Endpoints;

public class TraceEndpoints : IEndpoint
{
    private const string BaseRoute = "/traces";
    public static void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost(BaseRoute, AddTrace);
        app.MapGet(BaseRoute + "/{name}", GetTrace);
        app.MapGet(BaseRoute, GetAllTraces);
        app.MapPut(BaseRoute + "/{name}", UpdateTrace);
        app.MapDelete(BaseRoute + "/{name}", DeleteTrace);
    }
    private static async Task<IResult> AddTrace(
        TraceRequest traceRequest,
        ITraceService traceService)
    {
        var result = await traceService.SaveAsync(traceRequest.ToDomain());
        return Results.Created();
    }

    private static async Task<IResult> GetTrace(
        [FromRoute] string name,
        ITraceService traceService)
    {
        var result = await traceService.GetTraceAsync(name);
        return result is null ? Results.NotFound() : Results.Ok(result.ToContract());
    }

    private static async Task<IResult> GetAllTraces(
        string name,
        ITraceService traceService)
    {
        var result = await traceService.GetTracesAsync(name);
        return Results.Ok(result.Select(x => x.ToContract()));
    }

    private static async Task<IResult> UpdateTrace(
        string name,
        [FromBody] TraceRequest traceRequest,
        ITraceService traceService)
    {
        var existingTrace = await traceService.GetTraceAsync(name);
        if (existingTrace is null)
            return Results.NotFound();

        var result = await traceService.UpdateAsync(traceRequest.ToDomain());
        return result ? Results.Ok() : Results.NotFound();
    }

    private static async Task<IResult> DeleteTrace(
        string name,
        ITraceService traceService)
    {
        var existingTrace = await traceService.GetTraceAsync(name);
        if (existingTrace is null)
            return Results.NotFound();

        var result = await traceService.DeleteAsync(name);
        return result ? Results.Ok() : Results.NotFound();
    }
}