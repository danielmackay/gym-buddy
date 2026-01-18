namespace GymBuddy.Api.Features.Users.Endpoints;

public class ListAllUsersEndpoint(ApplicationDbContext dbContext)
    : EndpointWithoutRequest<List<UserResponse>>
{
    public override void Configure()
    {
        Get("/");
        Group<UsersGroup>();
        Description(x => x.WithName("ListAllUsers"));
        AllowAnonymous(); // No auth in MVP
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        // Load all users from the database
        var allUsers = await dbContext.Users.ToListAsync(ct);
        
        var users = allUsers
            .Select(u => new UserResponse(
                u.Id.Value,
                u.Name,
                u.Email,
                u.Roles.ToList(),
                u.TrainerId.HasValue ? u.TrainerId.Value.Value : null,
                u.AssignedWorkoutPlanIds.Select(id => id.Value).ToList()))
            .ToList();

        await Send.OkAsync(users, ct);
    }
}

public class ListAllUsersSummary : Summary<ListAllUsersEndpoint>
{
    public ListAllUsersSummary()
    {
        Summary = "List all users";
        Description = "Retrieves a list of all users in the system (for user selection screen in MVP).";

        Response(200, "List of users retrieved successfully");
    }
}
