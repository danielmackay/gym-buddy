using GymBuddy.Api.Common.Interfaces;

namespace GymBuddy.Api.Features.Users;

public sealed class UsersFeature : IFeature
{
    public static void ConfigureServices(IServiceCollection services, IConfiguration config)
    {
        // TODO: Add feature-specific services here if needed
    }
}

public class UsersGroup : Group
{
    public UsersGroup()
    {
        // NOTE: The prefix is used as the tag and group name
        base.Configure("users", ep => ep.Description(x => x.ProducesProblemDetails(500)));
    }
}
