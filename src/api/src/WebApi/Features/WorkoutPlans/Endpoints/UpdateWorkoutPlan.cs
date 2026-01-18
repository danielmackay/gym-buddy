using GymBuddy.Domain.WorkoutPlans;

namespace GymBuddy.Api.Features.WorkoutPlans.Endpoints;

public record UpdateWorkoutPlanRequest(
    Guid Id,
    string Name,
    string? Description = null);

public class UpdateWorkoutPlanEndpoint(ApplicationDbContext dbContext)
    : Endpoint<UpdateWorkoutPlanRequest>
{
    public override void Configure()
    {
        Put("{id}");
        Group<WorkoutPlansGroup>();
        Description(x => x.WithName("UpdateWorkoutPlan")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound));
    }

    public override async Task HandleAsync(UpdateWorkoutPlanRequest req, CancellationToken ct)
    {
        var workoutPlanId = WorkoutPlanId.From(req.Id);

        var workoutPlan = await dbContext.WorkoutPlans
            .FirstOrDefaultAsync(wp => wp.Id == workoutPlanId, ct);

        if (workoutPlan is null)
        {
            AddError("Workout plan not found", "WorkoutPlan.NotFound");
            await Send.NotFoundAsync(ct);
            return;
        }

        workoutPlan.Name = req.Name;
        workoutPlan.Description = req.Description;

        await dbContext.SaveChangesAsync(ct);

        await Send.NoContentAsync(ct);
    }
}

public class UpdateWorkoutPlanRequestValidator : Validator<UpdateWorkoutPlanRequest>
{
    public UpdateWorkoutPlanRequestValidator()
    {
        RuleFor(v => v.Name)
            .NotEmpty()
            .WithMessage("Name is required")
            .MaximumLength(WorkoutPlan.NameMaxLength)
            .WithMessage($"Name must be {WorkoutPlan.NameMaxLength} characters or less");

        When(v => v.Description is not null, () =>
        {
            RuleFor(v => v.Description)
                .MaximumLength(WorkoutPlan.DescriptionMaxLength)
                .WithMessage($"Description must be {WorkoutPlan.DescriptionMaxLength} characters or less");
        });
    }
}

public class UpdateWorkoutPlanSummary : Summary<UpdateWorkoutPlanEndpoint>
{
    public UpdateWorkoutPlanSummary()
    {
        Summary = "Update a workout plan";
        Description = "Updates the name and description of an existing workout plan.";

        ExampleRequest = new UpdateWorkoutPlanRequest(
            Id: Guid.Parse("00000000-0000-0000-0000-000000000000"),
            Name: "Updated Full Body Workout",
            Description: "Updated description");
    }
}
