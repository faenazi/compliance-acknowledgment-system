using Eap.Api.Extensions;
using Eap.Api.Middleware;
using Eap.Application;
using Eap.Infrastructure;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Serilog: structured logging, read from configuration so environments can override.
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext());

// Services
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEapSwagger();

builder.Services.AddCors(options =>
{
    options.AddPolicy("EapFrontend", policy =>
    {
        var origins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
                      ?? new[] { "http://localhost:3000" };

        policy.WithOrigins(origins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddRouting(options => options.LowercaseUrls = true);

var app = builder.Build();

// Pipeline
app.UseSerilogRequestLogging();
app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseEapSwagger();
}

app.UseHttpsRedirection();
app.UseCors("EapFrontend");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/health", () => Results.Ok(new
{
    status = "healthy",
    service = "Eap.Api",
    timestamp = DateTimeOffset.UtcNow
}))
.WithName("Health")
.WithTags("System");

app.Run();
