using GymBuddy.Api.Common.Interfaces;

namespace GymBuddy.Api.Features.Exercises;

public sealed class ExercisesFeature : IFeature
{
    public static void ConfigureServices(IServiceCollection services, IConfiguration config)
    {
        // TODO: Add feature-specific services here if needed
    }
}

public class ExercisesGroup : Group
{
    public ExercisesGroup()
    {
        // NOTE: The prefix is used as the tag and group name
        base.Configure("exercises", ep => ep.Description(x => x.ProducesProblemDetails(500)));
    }
}
