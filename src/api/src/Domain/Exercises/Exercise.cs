using GymBuddy.Domain.Base;

namespace GymBuddy.Domain.Exercises;

[ValueObject<Guid>]
public readonly partial struct ExerciseId;

public class Exercise : AggregateRoot<ExerciseId>
{
    public const int NameMaxLength = 100;
    public const int DescriptionMaxLength = 500;

    private readonly List<MuscleGroup> _muscleGroups = [];

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

    public ExerciseType Type { get; private set; }

    public IReadOnlyList<MuscleGroup> MuscleGroups => _muscleGroups.AsReadOnly();

    private Exercise() { } // Needed for EF Core

    public static ErrorOr<Exercise> Create(
        string name,
        ExerciseType type,
        IEnumerable<MuscleGroup> muscleGroups,
        string? description = null)
    {
        var muscleGroupList = muscleGroups.ToList();

        if (muscleGroupList.Count == 0)
            return ExerciseErrors.NoMuscleGroups;

        var exercise = new Exercise
        {
            Id = ExerciseId.From(Guid.CreateVersion7()),
            Name = name,
            Type = type,
            Description = description
        };

        foreach (var muscleGroup in muscleGroupList.Distinct())
        {
            exercise._muscleGroups.Add(muscleGroup);
        }

        return exercise;
    }

    public ErrorOr<Success> Update(
        string name,
        string? description,
        IEnumerable<MuscleGroup> muscleGroups)
    {
        var muscleGroupList = muscleGroups.ToList();

        if (muscleGroupList.Count == 0)
            return ExerciseErrors.NoMuscleGroups;

        Name = name;
        Description = description;

        _muscleGroups.Clear();
        foreach (var muscleGroup in muscleGroupList.Distinct())
        {
            _muscleGroups.Add(muscleGroup);
        }

        return new Success();
    }
}