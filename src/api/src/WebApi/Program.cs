using System.Reflection;
using FastEndpoints.Swagger;
using GymBuddy.Api.Host.Extensions;
using GymBuddy.Api.Host;

var appAssembly = Assembly.GetExecutingAssembly();
var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddCustomProblemDetails();

builder.AddWebApi();
builder.AddApplication();
builder.AddInfrastructure();

// Add CORS services
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        var allowedOrigins = builder.Configuration
            .GetSection("CorsSettings:AllowedOrigins")
            .Get<string[]>();

        // In production, fail fast if CORS origins are not configured
        if (!builder.Environment.IsDevelopment() && (allowedOrigins == null || allowedOrigins.Length == 0))
        {
            throw new InvalidOperationException(
                "CORS allowed origins must be explicitly configured in production. " +
                "Set CorsSettings:AllowedOrigins in appsettings.json or environment variables.");
        }

        // Fallback to localhost for development only
        allowedOrigins ??= ["http://localhost:3000"];

        policy.WithOrigins(allowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader();
        // Note: .AllowCredentials() will be added when Auth0 is implemented
    });
});

builder.Services.ConfigureFeatures(builder.Configuration, appAssembly);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Use CORS before routing/endpoints
app.UseCors("AllowFrontend");

app.UseCustomFastEndpoints();
app.UseSwaggerGen();
app.UseEventualConsistencyMiddleware();

app.MapDefaultEndpoints();
app.UseExceptionHandler();

app.Run();

namespace GymBuddy.Api
{
    public partial class Program;
}