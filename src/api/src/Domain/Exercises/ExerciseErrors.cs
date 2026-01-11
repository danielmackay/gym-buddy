namespace GymBuddy.Domain.Exercises;

public static class ExerciseErrors
{
    public static readonly Error NotFound = Error.NotFound(
        "Exercise.NotFound",
        "Exercise is not found");

    public static readonly Error NoMuscleGroups = Error.Validation(
        "Exercise.NoMuscleGroups",
        "Exercise must target at least one muscle group");

    public static readonly Error DuplicateName = Error.Conflict(
        "Exercise.DuplicateName",
        "An exercise with this name already exists");
}
