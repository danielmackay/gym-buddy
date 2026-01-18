using GymBuddy.Domain.WorkoutPlans;
using GymBuddy.Domain.Exercises;
using GymBuddy.Domain.Common;

namespace GymBuddy.Api.Features.WorkoutPlans.Endpoints;

public record AddExerciseToPlanRequest(
    Guid WorkoutPlanId,
    Guid ExerciseId,
    int Sets,
    int? Reps = null,
    WeightRequest? Weight = null,
    DurationRequest? Duration = null);

public record WeightRequest(decimal Value, int Unit = 1); // WeightUnit enum, defaults to Kilograms
public record DurationRequest(int Seconds);

public class AddExerciseToPlanEndpoint(ApplicationDbContext dbContext)
    : Endpoint<AddExerciseToPlanRequest>
{
    public override void Configure()
    {
        Post("{workoutPlanId}/exercises");
        Group<WorkoutPlansGroup>();
        Description(x => x.WithName("AddExerciseToPlan")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound));
    }

    public override async Task HandleAsync(AddExerciseToPlanRequest req, CancellationToken ct)
    {
        var workoutPlanId = WorkoutPlanId.From(req.WorkoutPlanId);
        var exerciseId = ExerciseId.From(req.ExerciseId);

        var workoutPlan = await dbContext.WorkoutPlans
            .Include(wp => wp.Exercises)
            .FirstOrDefaultAsync(wp => wp.Id == workoutPlanId, ct);

        if (workoutPlan is null)
        {
            AddError("Workout plan not found", "WorkoutPlan.NotFound");
            await Send.NotFoundAsync(ct);
            return;
        }

        var exercise = await dbContext.Exercises
            .FirstOrDefaultAsync(e => e.Id == exerciseId, ct);

        if (exercise is null)
        {
            AddError("Exercise not found", "Exercise.NotFound");
            await Send.NotFoundAsync(ct);
            return;
        }

        // Convert request DTOs to domain value objects
        Weight? weight = req.Weight != null 
            ? new Weight(req.Weight.Value, (WeightUnit)req.Weight.Unit) 
            : null;

        Duration? duration = req.Duration != null 
            ? new Duration(req.Duration.Seconds) 
            : null;

        var result = workoutPlan.AddExercise(
            exercise,
            req.Sets,
            req.Reps,
            weight,
            duration);

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

public class AddExerciseToPlanRequestValidator : Validator<AddExerciseToPlanRequest>
{
    public AddExerciseToPlanRequestValidator()
    {
        RuleFor(v => v.WorkoutPlanId)
            .NotEmpty()
            .WithMessage("WorkoutPlanId is required");

        RuleFor(v => v.ExerciseId)
            .NotEmpty()
            .WithMessage("ExerciseId is required");

        RuleFor(v => v.Sets)
            .GreaterThan(0)
            .WithMessage("Sets must be greater than zero");

        When(v => v.Reps.HasValue, () =>
        {
            RuleFor(v => v.Reps!.Value)
                .GreaterThan(0)
                .WithMessage("Reps must be greater than zero when provided");
        });

        When(v => v.Weight != null, () =>
        {
            RuleFor(v => v.Weight!.Value)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Weight must be zero or greater");

            RuleFor(v => v.Weight!.Unit)
                .IsInEnum()
                .WithMessage("Weight unit must be valid (1 = Kilograms, 2 = Pounds)");
        });

        When(v => v.Duration != null, () =>
        {
            RuleFor(v => v.Duration!.Seconds)
                .GreaterThan(0)
                .WithMessage("Duration must be greater than zero");
        });
    }
}

public class AddExerciseToPlanSummary : Summary<AddExerciseToPlanEndpoint>
{
    public AddExerciseToPlanSummary()
    {
        Summary = "Add an exercise to a workout plan";
        Description = "Adds an exercise to a workout plan with specified sets, reps/weight or duration depending on exercise type.";

        ExampleRequest = new AddExerciseToPlanRequest(
            WorkoutPlanId: Guid.Parse("00000000-0000-0000-0000-000000000000"),
            ExerciseId: Guid.Parse("00000000-0000-0000-0000-000000000001"),
            Sets: 3,
            Reps: 10,
            Weight: new WeightRequest(20, 1));  // 20kg
    }
}
