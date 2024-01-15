var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapPost("/test", async ctx =>
{
    var request = ctx.Request;
    using var reader = new StreamReader(request.Body);
    var body = await reader.ReadToEndAsync();
    Console.WriteLine("Received telemetry:");
    Console.WriteLine(body);

    // body contains the telemetry data sent by the OpenTelemetry exporter
    // You can now process this data as needed

    Results.Ok();
});

app.Run();