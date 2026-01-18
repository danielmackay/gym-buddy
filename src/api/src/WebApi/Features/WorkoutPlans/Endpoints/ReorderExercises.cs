using GymBuddy.Domain.WorkoutPlans;
using GymBuddy.Domain.Exercises;

namespace GymBuddy.Api.Features.WorkoutPlans.Endpoints;

public record ReorderExercisesRequest(
    Guid WorkoutPlanId,
    List<Guid> ExerciseIds);

public class ReorderExercisesEndpoint(ApplicationDbContext dbContext)
    : Endpoint<ReorderExercisesRequest>
{
    public override void Configure()
    {
        Put("{workoutPlanId}/exercises/reorder");
        Group<WorkoutPlansGroup>();
        Description(x => x.WithName("ReorderExercises")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound));
    }

    public override async Task HandleAsync(ReorderExercisesRequest req, CancellationToken ct)
    {
        var workoutPlanId = WorkoutPlanId.From(req.WorkoutPlanId);

        var workoutPlan = await dbContext.WorkoutPlans
            .Include(wp => wp.Exercises)
            .FirstOrDefaultAsync(wp => wp.Id == workoutPlanId, ct);

        if (workoutPlan is null)
        {
            AddError("Workout plan not found", "WorkoutPlan.NotFound");
            await Send.NotFoundAsync(ct);
            return;
        }

        var exerciseIds = req.ExerciseIds.Select(id => ExerciseId.From(id));

        var result = workoutPlan.ReorderExercises(exerciseIds);

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

public class ReorderExercisesRequestValidator : Validator<ReorderExercisesRequest>
{
    public ReorderExercisesRequestValidator()
    {
        RuleFor(v => v.WorkoutPlanId)
            .NotEmpty()
            .WithMessage("WorkoutPlanId is required");

        RuleFor(v => v.ExerciseIds)
            .NotEmpty()
            .WithMessage("ExerciseIds list cannot be empty");

        RuleForEach(v => v.ExerciseIds)
            .NotEmpty()
            .WithMessage("All exercise IDs must be valid GUIDs");
    }
}

public class ReorderExercisesSummary : Summary<ReorderExercisesEndpoint>
{
    public ReorderExercisesSummary()
    {
        Summary = "Reorder exercises in a workout plan";
        Description = "Reorders the exercises in a workout plan based on the provided list of exercise IDs in the desired order.";

        ExampleRequest = new ReorderExercisesRequest(
            WorkoutPlanId: Guid.Parse("00000000-0000-0000-0000-000000000000"),
            ExerciseIds: [
                Guid.Parse("00000000-0000-0000-0000-000000000002"),
                Guid.Parse("00000000-0000-0000-0000-000000000001"),
                Guid.Parse("00000000-0000-0000-0000-000000000003")
            ]);
    }
}
