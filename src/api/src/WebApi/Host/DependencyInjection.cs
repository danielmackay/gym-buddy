using FastEndpoints.Swagger;
using GymBuddy.Api.Common.Interfaces;
using GymBuddy.Api.Common.Services;

namespace GymBuddy.Api.Host;

public static class DependencyInjection
{
    public static void AddWebApi(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;
        
        services.AddHttpContextAccessor();
        
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        
        services.AddOpenApi();

        services.AddFastEndpoints();

        builder.Services.SwaggerDocument();
    }
    
    public static void AddApplication(this IHostApplicationBuilder builder)
    {
        var applicationAssembly = typeof(DependencyInjection).Assembly;
        var services = builder.Services;
        
        services.AddValidatorsFromAssembly(applicationAssembly, includeInternalTypes: true);
    }
}