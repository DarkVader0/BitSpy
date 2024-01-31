using BitSpy.Api;
using BitSpy.Api.Endpoints.Internal;
using BitSpy.Api.Middleware;
using BitSpy.Api.Repositories;
using BitSpy.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<ILogService, LogService>();
builder.Services.AddScoped<IMetricService, MetricService>();
builder.Services.AddScoped<ITraceService, TraceService>();

builder.Services.AddScoped<ILogRepository, LogRepository>();
builder.Services.AddScoped<IMetricRepository, MetricRepository>();
builder.Services.AddScoped<ITraceRepository, TraceRepository>();
builder.Services.AddScoped<ILongTermTraceRepository, LongTermTraceRepository>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

await DatabaseInitializer.InitializeAsync(builder.Configuration);
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseSwagger();
app.UseSwaggerUI();
app.UseEndpoints();
app.Run();