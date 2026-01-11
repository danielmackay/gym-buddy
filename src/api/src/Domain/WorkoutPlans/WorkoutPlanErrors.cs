namespace GymBuddy.Domain.WorkoutPlans;

public static class WorkoutPlanErrors
{
    public static readonly Error NotFound = Error.NotFound(
        "WorkoutPlan.NotFound",
        "Workout plan is not found");

    public static readonly Error ExerciseNotFound = Error.NotFound(
        "WorkoutPlan.ExerciseNotFound",
        "Exercise not found in workout plan");

    public static readonly Error ExerciseAlreadyExists = Error.Conflict(
        "WorkoutPlan.ExerciseAlreadyExists",
        "Exercise already exists in workout plan");

    public static readonly Error InvalidSets = Error.Validation(
        "WorkoutPlan.InvalidSets",
        "Sets must be greater than zero");

    public static readonly Error InvalidReps = Error.Validation(
        "WorkoutPlan.InvalidReps",
        "Reps must be greater than zero for reps and weight exercises");

    public static readonly Error InvalidWeight = Error.Validation(
        "WorkoutPlan.InvalidWeight",
        "Weight must be zero or greater");

    public static readonly Error InvalidDuration = Error.Validation(
        "WorkoutPlan.InvalidDuration",
        "Duration must be greater than zero for time-based exercises");

    public static readonly Error MismatchedExerciseType = Error.Validation(
        "WorkoutPlan.MismatchedExerciseType",
        "Exercise type does not match the provided parameters");
}
