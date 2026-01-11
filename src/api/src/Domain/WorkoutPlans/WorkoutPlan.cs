using GymBuddy.Domain.Base;
using GymBuddy.Domain.Exercises;
using GymBuddy.Domain.Users;

namespace GymBuddy.Domain.WorkoutPlans;

[ValueObject<Guid>]
public readonly partial struct WorkoutPlanId;

public class WorkoutPlan : AggregateRoot<WorkoutPlanId>
{
    public const int NameMaxLength = 100;
    public const int DescriptionMaxLength = 500;

    private readonly List<PlannedExercise> _exercises = [];

    public string Name
    {
        get;
        set
        {
            ThrowIfNullOrWhiteSpace(value, nameof(Name));
            ThrowIfGreaterThan(value.Length, NameMaxLength, nameof(Name));
            field = value;
        }
    } = null!;

    public string? Description
    {
        get;
        set
        {
            if (value is not null)
                ThrowIfGreaterThan(value.Length, DescriptionMaxLength, nameof(Description));
            field = value;
        }
    }

    public UserId TrainerId { get; private set; }

    public IReadOnlyList<PlannedExercise> Exercises => _exercises.AsReadOnly();

    private WorkoutPlan() { } // Needed for EF Core

    public static WorkoutPlan Create(string name, UserId trainerId, string? description = null)
    {
        var workoutPlan = new WorkoutPlan
        {
            Id = WorkoutPlanId.From(Guid.CreateVersion7()),
            Name = name,
            TrainerId = trainerId,
            Description = description
        };

        return workoutPlan;
    }

    public ErrorOr<Success> AddExercise(
        Exercise exercise,
        int sets,
        int? reps = null,
        decimal? weight = null,
        int? durationSeconds = null)
    {
        if (_exercises.Any(e => e.ExerciseId == exercise.Id))
            return WorkoutPlanErrors.ExerciseAlreadyExists;

        var order = _exercises.Count + 1;

        var plannedExerciseResult = PlannedExercise.Create(
            exercise.Id,
            exercise.Name,
            exercise.Type,
            sets,
            order,
            reps,
            weight,
            durationSeconds);

        if (plannedExerciseResult.IsError)
            return plannedExerciseResult.Errors;

        _exercises.Add(plannedExerciseResult.Value);
        return new Success();
    }

    public ErrorOr<Success> RemoveExercise(ExerciseId exerciseId)
    {
        var exercise = _exercises.FirstOrDefault(e => e.ExerciseId == exerciseId);

        if (exercise is null)
            return WorkoutPlanErrors.ExerciseNotFound;

        _exercises.Remove(exercise);
        ReorderExercises();

        return new Success();
    }

    public ErrorOr<Success> UpdateExercise(
        ExerciseId exerciseId,
        int sets,
        int? reps = null,
        decimal? weight = null,
        int? durationSeconds = null)
    {
        var existingExercise = _exercises.FirstOrDefault(e => e.ExerciseId == exerciseId);

        if (existingExercise is null)
            return WorkoutPlanErrors.ExerciseNotFound;

        return existingExercise.Update(sets, reps, weight, durationSeconds);
    }

    public ErrorOr<Success> ReorderExercises(IEnumerable<ExerciseId> newOrder)
    {
        var orderList = newOrder.ToList();

        // Validate all exercises exist
        foreach (var exerciseId in orderList)
        {
            if (!_exercises.Any(e => e.ExerciseId == exerciseId))
                return WorkoutPlanErrors.ExerciseNotFound;
        }

        // Reorder based on new sequence
        var reorderedExercises = new List<PlannedExercise>();
        var order = 1;

        foreach (var exerciseId in orderList)
        {
            var exercise = _exercises.First(e => e.ExerciseId == exerciseId);
            exercise.UpdateOrder(order);
            reorderedExercises.Add(exercise);
            order++;
        }

        _exercises.Clear();
        _exercises.AddRange(reorderedExercises);

        return new Success();
    }

    private void ReorderExercises()
    {
        var order = 1;
        foreach (var exercise in _exercises)
        {
            exercise.UpdateOrder(order);
            order++;
        }
    }
}