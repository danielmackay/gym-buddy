using GymBuddy.Domain.Base;
using GymBuddy.Domain.Common;
using GymBuddy.Domain.Exercises;

namespace GymBuddy.Domain.WorkoutPlans;

[ValueObject<Guid>]
public readonly partial struct PlannedExerciseId;

public class PlannedExercise : Entity<PlannedExerciseId>
{
    public const int ExerciseNameMaxLength = 100;

    public ExerciseId ExerciseId { get; private set; }

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

    public int Sets
    {
        get;
        private set
        {
            ThrowIfLessThan(value, 1, nameof(Sets));
            field = value;
        }
    }

    // For RepsAndWeight exercises
    public int? Reps { get; private set; }
    public Weight? Weight { get; private set; }

    // For TimeBased exercises
    public Duration? Duration { get; private set; }

    public int Order { get; private set; }

    private PlannedExercise() { } // Needed for EF Core

    // NOTE: Internal so that PlannedExercises can only be created by the WorkoutPlan aggregate
    internal static ErrorOr<PlannedExercise> Create(
        ExerciseId exerciseId,
        string exerciseName,
        ExerciseType exerciseType,
        int sets,
        int order,
        int? reps = null,
        Weight? weight = null,
        Duration? duration = null)
    {
        if (sets < 1)
            return WorkoutPlanErrors.InvalidSets;

        if (exerciseType == ExerciseType.RepsAndWeight)
        {
            if (!reps.HasValue || reps < 1)
                return WorkoutPlanErrors.InvalidReps;
            // Weight validation is now handled by the Weight value object itself
        }
        else if (exerciseType == ExerciseType.TimeBased)
        {
            if (duration is null)
                return WorkoutPlanErrors.InvalidDuration;
            // Duration validation is now handled by the Duration value object itself
        }

        return new PlannedExercise
        {
            Id = PlannedExerciseId.From(Guid.CreateVersion7()),
            ExerciseId = exerciseId,
            ExerciseName = exerciseName,
            ExerciseType = exerciseType,
            Sets = sets,
            Reps = exerciseType == ExerciseType.RepsAndWeight ? reps : null,
            Weight = exerciseType == ExerciseType.RepsAndWeight ? weight : null,
            Duration = exerciseType == ExerciseType.TimeBased ? duration : null,
            Order = order
        };
    }

    internal void UpdateOrder(int order)
    {
        Order = order;
    }

    internal ErrorOr<Success> Update(
        int sets,
        int? reps = null,
        Weight? weight = null,
        Duration? duration = null)
    {
        if (sets < 1)
            return WorkoutPlanErrors.InvalidSets;

        if (ExerciseType == ExerciseType.RepsAndWeight)
        {
            if (!reps.HasValue || reps < 1)
                return WorkoutPlanErrors.InvalidReps;
            // Weight validation is now handled by the Weight value object itself
        }
        else if (ExerciseType == ExerciseType.TimeBased)
        {
            if (duration is null)
                return WorkoutPlanErrors.InvalidDuration;
            // Duration validation is now handled by the Duration value object itself
        }

        Sets = sets;
        Reps = ExerciseType == ExerciseType.RepsAndWeight ? reps : null;
        Weight = ExerciseType == ExerciseType.RepsAndWeight ? weight : null;
        Duration = ExerciseType == ExerciseType.TimeBased ? duration : null;

        return new Success();
    }
}
