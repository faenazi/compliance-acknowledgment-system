using Eap.Api.Authentication;
using Eap.Api.Extensions;
using Eap.Api.Middleware;
using Eap.Application;
using Eap.Application.Identity.Abstractions;
using Eap.Infrastructure;
using Eap.Infrastructure.Identity.Seeding;
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

builder.Services.AddHttpContextAccessor();

// Authentication (cookie) + access context adapters.
builder.Services.AddEapCookieAuthentication();
builder.Services.AddScoped<ICurrentUser, CurrentUserService>();
builder.Services.AddScoped<IAuthenticationSession, HttpAuthenticationSession>();

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

// Reference data seeding (idempotent).
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<IdentitySeeder>();
    await seeder.SeedAsync(CancellationToken.None);
}

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
