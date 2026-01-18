using GymBuddy.Domain.WorkoutPlans;
using GymBuddy.Domain.Users;

namespace GymBuddy.Api.Features.WorkoutPlans.Endpoints;

public record UnassignPlanFromClientRequest(
    Guid ClientId,
    Guid WorkoutPlanId);

public class UnassignPlanFromClientEndpoint(ApplicationDbContext dbContext)
    : Endpoint<UnassignPlanFromClientRequest>
{
    public override void Configure()
    {
        Post("unassign");
        Group<WorkoutPlansGroup>();
        Description(x => x.WithName("UnassignPlanFromClient")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest));
    }

    public override async Task HandleAsync(UnassignPlanFromClientRequest req, CancellationToken ct)
    {
        // Find the client user
        var client = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == UserId.From(req.ClientId), ct);

        if (client is null)
        {
            AddError($"Client with ID {req.ClientId} not found.", "User.NotFound");
            await Send.ErrorsAsync(cancellation: ct);
            return;
        }

        // Ensure the workout plan exists
        var workoutPlanExists = await dbContext.WorkoutPlans
            .AnyAsync(wp => wp.Id == WorkoutPlanId.From(req.WorkoutPlanId), ct);

        if (!workoutPlanExists)
        {
            AddError($"Workout plan with ID {req.WorkoutPlanId} not found.", "WorkoutPlan.NotFound");
            await Send.ErrorsAsync(cancellation: ct);
            return;
        }
        // Unassign the workout plan from the client
        var result = client.UnassignWorkoutPlan(WorkoutPlanId.From(req.WorkoutPlanId));

        if (result.IsError)
        {
            result.Errors.ForEach(e => AddError(e.Description, e.Code));
            await Send.ErrorsAsync(cancellation: ct);
            return;
        }

        await dbContext.SaveChangesAsync(ct);

        await Send.NoContentAsync(ct);
    }
}

public class UnassignPlanFromClientRequestValidator : Validator<UnassignPlanFromClientRequest>
{
    public UnassignPlanFromClientRequestValidator()
    {
        RuleFor(v => v.ClientId)
            .NotEmpty()
            .WithMessage("ClientId is required");

        RuleFor(v => v.WorkoutPlanId)
            .NotEmpty()
            .WithMessage("WorkoutPlanId is required");
    }
}

public class UnassignPlanFromClientSummary : Summary<UnassignPlanFromClientEndpoint>
{
    public UnassignPlanFromClientSummary()
    {
        Summary = "Unassign a workout plan from a client";
        Description = "Removes a workout plan assignment from a client. The client must exist and the plan must be currently assigned.";

        ExampleRequest = new UnassignPlanFromClientRequest(
            ClientId: Guid.Parse("00000000-0000-0000-0000-000000000001"),
            WorkoutPlanId: Guid.Parse("00000000-0000-0000-0000-000000000002"));
    }
}
