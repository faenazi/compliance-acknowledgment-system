using Eap.Api.DependencyInjection;
using Eap.Api.Endpoints;
using Eap.Api.Middleware;
using Eap.Application;
using Eap.Infrastructure;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration);
});

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApiServices();

var app = builder.Build();

app.UseSerilogRequestLogging();
app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "EAP API v1");
    });
}

app.MapSystemEndpoints();
app.MapHealthChecks("/health");

app.Run();
