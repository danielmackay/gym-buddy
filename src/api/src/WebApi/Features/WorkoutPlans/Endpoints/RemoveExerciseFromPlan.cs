using GymBuddy.Domain.WorkoutPlans;
using GymBuddy.Domain.Exercises;

namespace GymBuddy.Api.Features.WorkoutPlans.Endpoints;

public record RemoveExerciseFromPlanRequest(
    Guid WorkoutPlanId,
    Guid ExerciseId);

public class RemoveExerciseFromPlanEndpoint(ApplicationDbContext dbContext)
    : Endpoint<RemoveExerciseFromPlanRequest>
{
    public override void Configure()
    {
        Delete("{workoutPlanId}/exercises/{exerciseId}");
        Group<WorkoutPlansGroup>();
        Description(x => x.WithName("RemoveExerciseFromPlan")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound));
    }

    public override async Task HandleAsync(RemoveExerciseFromPlanRequest req, CancellationToken ct)
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

        var result = workoutPlan.RemoveExercise(exerciseId);

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

public class RemoveExerciseFromPlanSummary : Summary<RemoveExerciseFromPlanEndpoint>
{
    public RemoveExerciseFromPlanSummary()
    {
        Summary = "Remove an exercise from a workout plan";
        Description = "Removes an exercise from a workout plan and reorders the remaining exercises.";
    }
}
