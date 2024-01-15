using System.Diagnostics;
using BitSpy.Api.Generator.Services;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var tracingOtlpEndpoint = "https://localhost:7266";

using var tracerProvider = Sdk.CreateTracerProviderBuilder()
    .AddSource(builder.Environment.ApplicationName)
    .SetResourceBuilder(
        ResourceBuilder.CreateDefault()
            .AddService(serviceName: builder.Environment.ApplicationName, serviceVersion: "1.0.0"))
    .AddAspNetCoreInstrumentation()
    .AddGrpcClientInstrumentation(options =>
    {
        // Note: Only called on .NET & .NET Core runtimes.
        options.EnrichWithHttpRequestMessage = (activity, httpRequestMessage) =>
        {
            activity.SetTag("requestVersion", httpRequestMessage.Version);
        };
        // Note: Only called on .NET & .NET Core runtimes.
        options.EnrichWithHttpResponseMessage = (activity, httpResponseMessage) =>
        {
            activity.SetTag("responseVersion", httpResponseMessage.Version);
        };
    })
    .AddOtlpExporter(otlpOptions =>
    {
        otlpOptions.Endpoint = new Uri(tracingOtlpEndpoint);
        otlpOptions.ExportProcessorType = ExportProcessorType.Simple;
        otlpOptions.Protocol = OtlpExportProtocol.Grpc;
    })
    .AddConsoleExporter()
    .Build();

builder.Services.AddScoped<IWeatherForecastService, WeatherForecastService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var activitySource = new ActivitySource(builder.Environment.ApplicationName);

app.MapGet("/weatherforecast",
        async (IWeatherForecastService weatherForecastService) =>
        {
            // ReSharper disable once ExplicitCallerInfoArgument
            using var activity = activitySource.StartActivity("WeatherForecastEndpoint.GetWeatherForecast");

            var forecast = await weatherForecastService.GetWeatherForecastAsync();

            activity?.SetTag("weather.forecast", "success");
            return forecast;
        })
    .WithName("GetWeatherForecast")
    .WithOpenApi();

app.Run();