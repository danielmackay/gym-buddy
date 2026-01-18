using GymBuddy.Domain.WorkoutPlans;
using GymBuddy.Domain.Users;

namespace GymBuddy.Api.Features.WorkoutPlans.Endpoints;

public record AssignPlanToClientRequest(
    Guid ClientId,
    Guid WorkoutPlanId);

public class AssignPlanToClientEndpoint(ApplicationDbContext dbContext)
    : Endpoint<AssignPlanToClientRequest>
{
    public override void Configure()
    {
        Post("assign");
        Group<WorkoutPlansGroup>();
        Description(x => x.WithName("AssignPlanToClient")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest));
    }

    public override async Task HandleAsync(AssignPlanToClientRequest req, CancellationToken ct)
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

        // Find the workout plan
        var workoutPlan = await dbContext.WorkoutPlans
            .FirstOrDefaultAsync(wp => wp.Id == WorkoutPlanId.From(req.WorkoutPlanId), ct);

        if (workoutPlan is null)
        {
            AddError($"Workout plan with ID {req.WorkoutPlanId} not found.", "WorkoutPlan.NotFound");
            await Send.ErrorsAsync(cancellation: ct);
            return;
        }

        // Assign the workout plan to the client
        var result = client.AssignWorkoutPlan(workoutPlan.Id);

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

public class AssignPlanToClientRequestValidator : Validator<AssignPlanToClientRequest>
{
    public AssignPlanToClientRequestValidator()
    {
        RuleFor(v => v.ClientId)
            .NotEmpty()
            .WithMessage("ClientId is required");

        RuleFor(v => v.WorkoutPlanId)
            .NotEmpty()
            .WithMessage("WorkoutPlanId is required");
    }
}

public class AssignPlanToClientSummary : Summary<AssignPlanToClientEndpoint>
{
    public AssignPlanToClientSummary()
    {
        Summary = "Assign a workout plan to a client";
        Description = "Assigns an existing workout plan to a client. The client must exist and must have the Client role.";

        ExampleRequest = new AssignPlanToClientRequest(
            ClientId: Guid.Parse("00000000-0000-0000-0000-000000000001"),
            WorkoutPlanId: Guid.Parse("00000000-0000-0000-0000-000000000002"));
    }
}
