using GymBuddy.Domain.Users;

namespace GymBuddy.Api.Features.Users.Endpoints;

public record UserResponse(
    Guid Id,
    string Name,
    string Email,
    List<UserRole> Roles,
    Guid? TrainerId,
    List<Guid> AssignedWorkoutPlanIds);

public class ListTrainersEndpoint(ApplicationDbContext dbContext)
    : EndpointWithoutRequest<List<UserResponse>>
{
    public override void Configure()
    {
        Get("/trainers");
        Group<UsersGroup>();
        Description(x => x.WithName("ListTrainers"));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        // Load all users and filter in memory since EF Core cannot translate Contains on JSON columns
        var allUsers = await dbContext.Users.ToListAsync(ct);
        
        var trainers = allUsers
            .Where(u => u.Roles.Contains(UserRole.Trainer))
            .Select(u => new UserResponse(
                u.Id.Value,
                u.Name,
                u.Email,
                u.Roles.ToList(),
                u.TrainerId.HasValue ? u.TrainerId.Value.Value : null,
                u.AssignedWorkoutPlanIds.Select(id => id.Value).ToList()))
            .ToList();

        await Send.OkAsync(trainers, ct);
    }
}

public class ListTrainersSummary : Summary<ListTrainersEndpoint>
{
    public ListTrainersSummary()
    {
        Summary = "List all trainers";
        Description = "Retrieves a list of all users with the Trainer role.";

        Response(200, "List of trainers retrieved successfully");
    }
}
