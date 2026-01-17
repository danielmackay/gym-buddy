using GymBuddy.Domain.Users;

namespace GymBuddy.Api.Features.Users.Endpoints;

public record GetUserRequest(Guid Id);

public class GetUserEndpoint(ApplicationDbContext dbContext)
    : Endpoint<GetUserRequest, UserResponse>
{
    public override void Configure()
    {
        Get("/{id}");
        Group<UsersGroup>();
        Description(x => x.WithName("GetUser")
            .Produces(StatusCodes.Status404NotFound));
    }

    public override async Task HandleAsync(GetUserRequest req, CancellationToken ct)
    {
        var userId = UserId.From(req.Id);
        var user = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == userId, ct);

        if (user is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var response = new UserResponse(
            user.Id.Value,
            user.Name,
            user.Email,
            user.Roles.ToList(),
            user.TrainerId.HasValue ? user.TrainerId.Value.Value : null,
            user.AssignedWorkoutPlanIds.Select(id => id.Value).ToList());

        await Send.OkAsync(response, ct);
    }
}

public class GetUserSummary : Summary<GetUserEndpoint>
{
    public GetUserSummary()
    {
        Summary = "Get user by ID";
        Description = "Retrieves a single user by their unique identifier.";

        Response(200, "User retrieved successfully");
        Response(404, "User not found");
    }
}
