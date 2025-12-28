using Portfolio.API.Extensions;
using Portfolio.API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<JsonBackedStateService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/favicon.ico", context =>
{
    context.Response.StatusCode = StatusCodes.Status204NoContent;
    return Task.CompletedTask;
});

app.AddPortfolioExtensions();

app.Run();