using GymBuddy.Api.Common.Middleware;

namespace GymBuddy.Api.Host.Extensions;

public static class EventualConsistencyMiddlewareExt
{
    public static IApplicationBuilder UseEventualConsistencyMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<EventualConsistencyMiddleware>();
        return app;
    }
}