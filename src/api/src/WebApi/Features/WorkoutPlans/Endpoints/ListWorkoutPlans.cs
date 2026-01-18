using GymBuddy.Domain.WorkoutPlans;
using GymBuddy.Domain.Exercises;
using GymBuddy.Domain.Common;
using GymBuddy.Domain.Users;

namespace GymBuddy.Api.Features.WorkoutPlans.Endpoints;

public record ListWorkoutPlansRequest(
    [property: QueryParam]
    Guid? TrainerId);

public record WorkoutPlanResponse(
    Guid Id,
    string Name,
    string? Description,
    Guid TrainerId,
    List<PlannedExerciseResponse> Exercises);

public record PlannedExerciseResponse(
    Guid Id,
    Guid ExerciseId,
    string ExerciseName,
    ExerciseType ExerciseType,
    int Sets,
    int? Reps,
    WeightResponse? Weight,
    DurationResponse? Duration,
    int Order);

public record WeightResponse(decimal Value, int Unit);  // WeightUnit enum as int
public record DurationResponse(int Seconds);

public class ListWorkoutPlansEndpoint(ApplicationDbContext dbContext)
    : Endpoint<ListWorkoutPlansRequest, List<WorkoutPlanResponse>>
{
    public override void Configure()
    {
        Get("");
        Group<WorkoutPlansGroup>();
        Description(x => x.WithName("ListWorkoutPlans")
            .Produces<List<WorkoutPlanResponse>>());
    }

    public override async Task HandleAsync(ListWorkoutPlansRequest req, CancellationToken ct)
    {
        // Filter by trainer ID if provided (for future Auth0 integration)
        var query = dbContext.WorkoutPlans
            .Include(wp => wp.Exercises)
            .AsQueryable();

        if (req.TrainerId.HasValue)
        {
            var trainerId = UserId.From(req.TrainerId.Value);
            query = query.Where(wp => wp.TrainerId == trainerId);
        }

        var workoutPlans = await query.ToListAsync(ct);

        var response = workoutPlans.Select(wp => MapToResponse(wp)).ToList();

        await Send.OkAsync(response, ct);
    }

    private static WorkoutPlanResponse MapToResponse(WorkoutPlan workoutPlan)
    {
        return new WorkoutPlanResponse(
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
    }
}

public class ListWorkoutPlansSummary : Summary<ListWorkoutPlansEndpoint>
{
    public ListWorkoutPlansSummary()
    {
        Summary = "List all workout plans";
        Description = "Returns a list of all workout plans for the authenticated trainer.";
    }
}
