using GymBuddy.Domain.Base;
using GymBuddy.Domain.Exercises;
using GymBuddy.Domain.Users;
using GymBuddy.Domain.WorkoutPlans;

namespace GymBuddy.Domain.WorkoutSessions;

[ValueObject<Guid>]
public readonly partial struct WorkoutSessionId;

public class WorkoutSession : AggregateRoot<WorkoutSessionId>
{
    public const int WorkoutPlanNameMaxLength = 100;

    private readonly List<SessionExercise> _exercises = [];

    public UserId ClientId { get; private set; }
    public WorkoutPlanId WorkoutPlanId { get; private set; }

    public string WorkoutPlanName
    {
        get;
        private set
        {
            ThrowIfNullOrWhiteSpace(value, nameof(WorkoutPlanName));
            ThrowIfGreaterThan(value.Length, WorkoutPlanNameMaxLength, nameof(WorkoutPlanName));
            field = value;
        }
    } = null!;

    public DateTimeOffset StartedAt { get; private set; }
    public DateTimeOffset? CompletedAt { get; private set; }
    public SessionStatus Status { get; private set; }

    public IReadOnlyList<SessionExercise> Exercises => _exercises.AsReadOnly();

    private WorkoutSession() { } // Needed for EF Core

    public static WorkoutSession Start(
        UserId clientId,
        WorkoutPlan workoutPlan,
        TimeProvider timeProvider)
    {
        var session = new WorkoutSession
        {
            Id = WorkoutSessionId.From(Guid.CreateVersion7()),
            ClientId = clientId,
            WorkoutPlanId = workoutPlan.Id,
            WorkoutPlanName = workoutPlan.Name,
            StartedAt = timeProvider.GetUtcNow(),
            Status = SessionStatus.InProgress
        };

        // Create snapshot of all exercises from the workout plan
        foreach (var plannedExercise in workoutPlan.Exercises.OrderBy(e => e.Order))
        {
            var sessionExercise = SessionExercise.CreateFromPlannedExercise(plannedExercise);
            session._exercises.Add(sessionExercise);
        }

        return session;
    }

    public ErrorOr<Success> CompleteExercise(
        ExerciseId exerciseId,
        int actualSets,
        TimeProvider timeProvider,
        int? actualReps = null,
        decimal? actualWeight = null,
        int? actualDurationSeconds = null)
    {
        if (Status != SessionStatus.InProgress)
            return WorkoutSessionErrors.SessionNotActive;

        var exercise = _exercises.FirstOrDefault(e => e.ExerciseId == exerciseId);

        if (exercise is null)
            return WorkoutSessionErrors.ExerciseNotFound;

        return exercise.RecordActuals(
            actualSets,
            actualReps,
            actualWeight,
            actualDurationSeconds,
            timeProvider);
    }

    public ErrorOr<Success> Complete(TimeProvider timeProvider)
    {
        if (Status == SessionStatus.Completed)
            return WorkoutSessionErrors.AlreadyCompleted;

        if (Status == SessionStatus.Abandoned)
            return WorkoutSessionErrors.AlreadyAbandoned;

        Status = SessionStatus.Completed;
        CompletedAt = timeProvider.GetUtcNow();

        return new Success();
    }

    public ErrorOr<Success> Abandon(TimeProvider timeProvider)
    {
        if (Status == SessionStatus.Completed)
            return WorkoutSessionErrors.AlreadyCompleted;

        if (Status == SessionStatus.Abandoned)
            return WorkoutSessionErrors.AlreadyAbandoned;

        Status = SessionStatus.Abandoned;
        CompletedAt = timeProvider.GetUtcNow();

        return new Success();
    }

    public int GetCompletedExerciseCount() => _exercises.Count(e => e.IsCompleted);
    public int GetTotalExerciseCount() => _exercises.Count;
    public bool AreAllExercisesCompleted() => _exercises.All(e => e.IsCompleted);
}