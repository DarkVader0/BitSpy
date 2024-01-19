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

app.MapPost("/traces", async (TraceRequest traceRequest, ITraceService traceService) => { Results.Ok(); });

app.Run();