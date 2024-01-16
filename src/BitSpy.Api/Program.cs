var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapPost("/", async ctx =>
{
    var request = ctx.Request;

    Results.Ok();
});

app.Run();