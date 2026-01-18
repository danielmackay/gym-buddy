using GymBuddy.Domain.WorkoutPlans;

namespace GymBuddy.Api.Features.WorkoutPlans.Endpoints;

public record GetWorkoutPlanRequest(Guid Id);

public class GetWorkoutPlanEndpoint(ApplicationDbContext dbContext)
    : Endpoint<GetWorkoutPlanRequest, WorkoutPlanResponse>
{
    public override void Configure()
    {
        Get("{id}");
        Group<WorkoutPlansGroup>();
        Description(x => x.WithName("GetWorkoutPlan")
            .Produces<WorkoutPlanResponse>()
            .Produces(StatusCodes.Status404NotFound));
    }

    public override async Task HandleAsync(GetWorkoutPlanRequest req, CancellationToken ct)
    {
        var workoutPlanId = WorkoutPlanId.From(req.Id);

        var workoutPlan = await dbContext.WorkoutPlans
            .Include(wp => wp.Exercises)
            .FirstOrDefaultAsync(wp => wp.Id == workoutPlanId, ct);

        if (workoutPlan is null)
        {
            AddError("Workout plan not found", "WorkoutPlan.NotFound");
            await Send.NotFoundAsync(ct);
            return;
        }

        var response = new WorkoutPlanResponse(
            Id: workoutPlan.Id.Value,
            Name: workoutPlan.Name,
            Description: workoutPlan.Description,
            TrainerId: workoutPlan.TrainerId.Value,
            Exercises: workoutPlan.Exercises
                .OrderBy(e => e.Order)
                .Select(e => new PlannedExerciseResponse(
                    Id: e.Id.Value,
                    ExerciseId: e.ExerciseId.Value,
                    ExerciseName: e.ExerciseName,
                    ExerciseType: e.ExerciseType,
                    Sets: e.Sets,
                    Reps: e.Reps,
                    Weight: e.Weight != null ? new WeightResponse(e.Weight.Value, (int)e.Weight.Unit) : null,
                    Duration: e.Duration != null ? new DurationResponse(e.Duration.Seconds) : null,
                    Order: e.Order))
                .ToList());

        await Send.OkAsync(response, ct);
    }
}

public class GetWorkoutPlanSummary : Summary<GetWorkoutPlanEndpoint>
{
    public GetWorkoutPlanSummary()
    {
        Summary = "Get a workout plan by ID";
        Description = "Returns a single workout plan with all its exercises.";
    }
}
