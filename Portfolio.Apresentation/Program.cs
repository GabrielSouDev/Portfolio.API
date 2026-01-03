using Microsoft.Extensions.DependencyInjection;
using Portfolio.API.Extensions;
using Portfolio.API.Models;
using Portfolio.API.Services;
using Portfolio.Apresentation.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<BackedStateService>();
builder.Services.AddHostedService<JsonPersistenceService>();

builder.Configuration.AddEnvironmentVariables();
builder.Services.Configure<AdminUser>(builder.Configuration.GetSection("AdminUser"));
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("JWT"));


builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var secret = builder.Configuration["Jwt:Secret"];
        var issuer = builder.Configuration["Jwt:Issuer"];
        var audience = builder.Configuration["Jwt:Audience"];

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret!)),
            ValidateIssuer = !string.IsNullOrWhiteSpace(issuer),
            ValidIssuer = issuer,
            ValidateAudience = !string.IsNullOrWhiteSpace(audience),
            ValidAudience = audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization(options =>
{
    // (Opcional) Policy para admins
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireClaim("role", "Admin"));
});


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

app.AddAuthExtensions();

app.AddPortfolioExtensions();

app.Run();