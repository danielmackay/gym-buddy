using GymBuddy.Domain.Users;

namespace GymBuddy.Api.Features.Users.Endpoints;

public record ListClientsRequest
{
    // [QueryParam]
    public Guid TrainerId { get; init; }
}

public class ListClientsEndpoint(ApplicationDbContext dbContext)
    : Endpoint<ListClientsRequest, List<UserResponse>>
{
    public override void Configure()
    {
        Get("/clients");
        Group<UsersGroup>();
        Description(x => x.WithName("ListClients"));
        AllowAnonymous(); // TODO: Add authentication when Auth0 is integrated
    }

    public override async Task HandleAsync(ListClientsRequest req, CancellationToken ct)
    {
        var trainerId = UserId.From(req.TrainerId);
        
        // Load users for this trainer and filter in memory since EF Core cannot translate Contains on JSON columns
        var allClientsForTrainer = await dbContext.Users
            .Where(u => u.TrainerId == trainerId)
            .ToListAsync(ct);
        
        var clients = allClientsForTrainer
            .Where(u => u.Roles.Contains(UserRole.Client))
            .Select(u => new UserResponse(
                u.Id.Value,
                u.Name,
                u.Email,
                u.Roles.ToList(),
                u.TrainerId.HasValue ? u.TrainerId.Value.Value : null,
                u.AssignedWorkoutPlanIds.Select(id => id.Value).ToList()))
            .ToList();

        await Send.OkAsync(clients, ct);
    }
}

public class ListClientsRequestValidator : Validator<ListClientsRequest>
{
    public ListClientsRequestValidator()
    {
        RuleFor(v => v.TrainerId)
            .NotEmpty()
            .WithMessage("TrainerId is required");
    }
}

public class ListClientsSummary : Summary<ListClientsEndpoint>
{
    public ListClientsSummary()
    {
        Summary = "List clients for current trainer";
        Description = "Retrieves a list of all clients assigned to the specified trainer.";

        Response(200, "List of clients retrieved successfully");
    }
}