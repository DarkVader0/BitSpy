using BitSpy.Api.Contracts.Request;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapPost("/logs", async (LogRequest logRequest, HttpContext ctx) => { Results.Ok(); });

app.MapPost("/metrics", async (MetricRequest metricRequest, HttpContext ctx) => { Results.Ok(); });

app.MapPost("/traces", async (TraceRequest traceRequest, HttpContext ctx) => { Results.Ok(); });

app.Run();