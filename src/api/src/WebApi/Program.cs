using System.Reflection;
using FastEndpoints.Swagger;
using GymBuddy.Api.Host.Extensions;
using GymBuddy.Api.Host;

var appAssembly = Assembly.GetExecutingAssembly();
var builder = WebApplication.CreateBuilder(args);

// Default allowed origins for CORS (used as fallback)
var defaultAllowedOrigins = new[] { "http://localhost:3000" };

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
            .Get<string[]>() ?? defaultAllowedOrigins;

        policy.WithOrigins(allowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials(); // For future Auth0 integration
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