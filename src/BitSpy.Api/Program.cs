using BitSpy.Api.Contracts.Request;
using BitSpy.Api.Mappers;
using BitSpy.Api.Repositories;
using BitSpy.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<ILogService, LogService>();
builder.Services.AddScoped<IMetricService, MetricService>();
builder.Services.AddScoped<ITraceService, TraceService>();

builder.Services.AddScoped<ILogRepository, LogRepository>();
builder.Services.AddScoped<IMetricRepository, MetricRepository>();
builder.Services.AddScoped<ITraceRepository, TraceRepository>();

var app = builder.Build();

app.MapPost("/logs",
    async (LogRequest logRequest, ILogService logService) =>
    {
        await logService.SaveAsync(logRequest.ToDomain());
        Results.Created();
    });


app.MapPost("/metrics", async (MetricRequest metricRequest, IMetricService metricService) => { Results.Ok(); });

// app.MapPost("/traces", async (TraceRequest traceRequest, ITraceService traceService) => { Results.Ok(); });

app.MapPost("/traces", async (TraceRequest traceRequest, ITraceService traceService) => 
{
    var result = await traceService.SaveAsync(traceRequest.ToDomain());
    return Results.Created();
});

app.MapGet("/traces/{name}", async (string name, ITraceService traceService) => 
{
    var result = await traceService.GetTraceAsync(name);
    return result is null ? Results.NotFound() : Results.Ok(result);
});

app.MapGet("/traces", async (string name, ITraceService traceService) => 
{
    var result = await traceService.GetTracesAsync(name);
    return Results.Ok(result);
});

app.MapPut("/traces/{name}", async (string name, TraceRequest traceRequest, ITraceService traceService) => 
{
    var existingTrace = await traceService.GetTraceAsync(name);
    if (existingTrace is null)
        return Results.NotFound();

    var result = await traceService.UpdateAsync(traceRequest.ToDomain());
    return result ? Results.Ok() : Results.NotFound();
});

app.MapDelete("/traces/{name}", async (string name, ITraceService traceService) => 
{
    var existingTrace = await traceService.GetTraceAsync(name);
    if (existingTrace is null)
        return Results.NotFound();

    var result = await traceService.DeleteAsync(name);
    return result ? Results.Ok() : Results.NotFound();
});

app.Run();