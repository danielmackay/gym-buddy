using GymBuddy.Domain.Base;
using GymBuddy.Domain.Common;
using GymBuddy.Domain.Exercises;
using GymBuddy.Domain.WorkoutPlans;

namespace GymBuddy.Domain.WorkoutSessions;

[ValueObject<Guid>]
public readonly partial struct SessionExerciseId;

public class SessionExercise : Entity<SessionExerciseId>
{
    public const int ExerciseNameMaxLength = 100;

    // Original exercise reference
    public ExerciseId ExerciseId { get; private set; }

    // Snapshot data
    public string ExerciseName
    {
        get;
        private set
        {
            ThrowIfNullOrWhiteSpace(value, nameof(ExerciseName));
            ThrowIfGreaterThan(value.Length, ExerciseNameMaxLength, nameof(ExerciseName));
            field = value;
        }
    } = null!;

    public ExerciseType ExerciseType { get; private set; }

    // Target values (from workout plan)
    public int TargetSets { get; private set; }
    public int? TargetReps { get; private set; }
    public Weight? TargetWeight { get; private set; }
    public Duration? TargetDuration { get; private set; }

    // Actual recorded values
    public int? ActualSets { get; private set; }
    public int? ActualReps { get; private set; }
    public Weight? ActualWeight { get; private set; }
    public Duration? ActualDuration { get; private set; }

    public DateTimeOffset? CompletedAt { get; private set; }
    public int Order { get; private set; }

    public bool IsCompleted => CompletedAt.HasValue;

    // Private constructor for EF Core
    private SessionExercise() { }

    internal static SessionExercise CreateFromPlannedExercise(PlannedExercise plannedExercise)
    {
        return new SessionExercise
        {
            Id = SessionExerciseId.From(Guid.CreateVersion7()),
            ExerciseId = plannedExercise.ExerciseId,
            ExerciseName = plannedExercise.ExerciseName,
            ExerciseType = plannedExercise.ExerciseType,
            TargetSets = plannedExercise.Sets,
            TargetReps = plannedExercise.Reps,
            TargetWeight = plannedExercise.Weight,
            TargetDuration = plannedExercise.Duration,
            Order = plannedExercise.Order
        };
    }

    internal ErrorOr<Success> RecordActuals(
        int actualSets,
        int? actualReps,
        Weight? actualWeight,
        Duration? actualDuration,
        TimeProvider timeProvider)
    {
        if (IsCompleted)
            return WorkoutSessionErrors.ExerciseAlreadyRecorded;

        if (actualSets < 1)
            return WorkoutSessionErrors.InvalidActualSets;

        if (ExerciseType == ExerciseType.RepsAndWeight)
        {
            if (!actualReps.HasValue || actualReps < 1)
                return WorkoutSessionErrors.InvalidActualReps;
            // Weight validation is now handled by the Weight value object itself
        }
        else if (ExerciseType == ExerciseType.TimeBased)
        {
            if (actualDuration is null)
                return WorkoutSessionErrors.InvalidActualDuration;
            // Duration validation is now handled by the Duration value object itself
        }

        ActualSets = actualSets;
        ActualReps = ExerciseType == ExerciseType.RepsAndWeight ? actualReps : null;
        ActualWeight = ExerciseType == ExerciseType.RepsAndWeight ? actualWeight : null;
        ActualDuration = ExerciseType == ExerciseType.TimeBased ? actualDuration : null;
        CompletedAt = timeProvider.GetUtcNow();

        return new Success();
    }
}
