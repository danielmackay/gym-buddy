using GymBuddy.Api.Common.Interfaces;

namespace GymBuddy.Api.Features.WorkoutPlans;

public sealed class WorkoutPlansFeature : IFeature
{
    public static void ConfigureServices(IServiceCollection services, IConfiguration config)
    {
        // TODO: Add feature-specific services here if needed
    }
}

public class WorkoutPlansGroup : Group
{
    public WorkoutPlansGroup()
    {
        // NOTE: The prefix is used as the tag and group name
        base.Configure("workout-plans", ep => ep.Description(x => x.ProducesProblemDetails(500)));
    }
}
