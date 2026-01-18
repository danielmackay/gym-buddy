using GymBuddy.Domain.WorkoutPlans;
using GymBuddy.Domain.Users;

namespace GymBuddy.Api.Features.WorkoutPlans.Endpoints;

public record CreateWorkoutPlanRequest(
    string Name,
    string? Description = null);

public record CreateWorkoutPlanResponse(Guid Id);

public class CreateWorkoutPlanEndpoint(ApplicationDbContext dbContext)
    : Endpoint<CreateWorkoutPlanRequest, CreateWorkoutPlanResponse>
{
    public override void Configure()
    {
        Post("");
        Group<WorkoutPlansGroup>();
        Description(x => x.WithName("CreateWorkoutPlan")
            .Produces(StatusCodes.Status201Created));
    }

    public override async Task HandleAsync(CreateWorkoutPlanRequest req, CancellationToken ct)
    {
        // TODO: Get trainerId from authenticated user context
        // For now, we'll get the first trainer from the database for demonstration

        // TODO: Make querying users by role more efficient
        var users = await dbContext.Users.ToListAsync(ct);
        var trainer = users
            .FirstOrDefault(u => u.Roles.Contains(UserRole.Trainer));

        if (trainer is null)
        {
            AddError("No trainer found. Please create a trainer first.", "WorkoutPlan.NoTrainer");
            await Send.ErrorsAsync(cancellation: ct);
            return;
        }

        var workoutPlan = WorkoutPlan.Create(
            req.Name,
            trainer.Id,
            req.Description);

        dbContext.WorkoutPlans.Add(workoutPlan);
        await dbContext.SaveChangesAsync(ct);

        await Send.CreatedAtAsync<GetWorkoutPlanEndpoint>(
            new { id = workoutPlan.Id.Value },
            new CreateWorkoutPlanResponse(workoutPlan.Id.Value),
            cancellation: ct);
    }
}

public class CreateWorkoutPlanRequestValidator : Validator<CreateWorkoutPlanRequest>
{
    public CreateWorkoutPlanRequestValidator()
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

public class CreateWorkoutPlanSummary : Summary<CreateWorkoutPlanEndpoint>
{
    public CreateWorkoutPlanSummary()
    {
        Summary = "Create a new workout plan";
        Description = "Creates a new workout plan for the authenticated trainer. Returns the ID of the created workout plan.";

        ExampleRequest = new CreateWorkoutPlanRequest(
            Name: "Full Body Workout",
            Description: "A comprehensive full body workout targeting all major muscle groups");
    }
}
