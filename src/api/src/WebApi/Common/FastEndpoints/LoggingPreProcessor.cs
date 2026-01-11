using GymBuddy.Api.Common.Interfaces;

namespace GymBuddy.Api.Common.FastEndpoints;

public class LoggingPreProcessor : IGlobalPreProcessor
{
    private readonly ILogger _logger;

    public LoggingPreProcessor(ILogger<LoggingPreProcessor> logger)
    {
        _logger = logger;
    }

    public async Task PreProcessAsync(IPreProcessorContext context, CancellationToken ct)
    {
        var currentUserService = context.HttpContext.RequestServices.GetRequiredService<ICurrentUserService>();

        var requestName = context.Request?.GetType().Name;
        var userId = currentUserService.UserId ?? string.Empty;

        _logger.LogInformation("WebApi Request: {Name} {@UserId} {@Request}",
            requestName, userId, context.Request);

        await Task.CompletedTask;
    }
}