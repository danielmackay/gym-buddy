namespace GymBuddy.Domain.WorkoutSessions;

public static class WorkoutSessionErrors
{
    public static readonly Error NotFound = Error.NotFound(
        "WorkoutSession.NotFound",
        "Workout session is not found");

    public static readonly Error ExerciseNotFound = Error.NotFound(
        "WorkoutSession.ExerciseNotFound",
        "Exercise not found in workout session");

    public static readonly Error AlreadyCompleted = Error.Conflict(
        "WorkoutSession.AlreadyCompleted",
        "Workout session is already completed");

    public static readonly Error AlreadyAbandoned = Error.Conflict(
        "WorkoutSession.AlreadyAbandoned",
        "Workout session has already been abandoned");

    public static readonly Error SessionNotActive = Error.Conflict(
        "WorkoutSession.SessionNotActive",
        "Cannot modify a session that is not in progress");

    public static readonly Error ExerciseAlreadyRecorded = Error.Conflict(
        "WorkoutSession.ExerciseAlreadyRecorded",
        "Exercise has already been recorded");

    public static readonly Error InvalidActualSets = Error.Validation(
        "WorkoutSession.InvalidActualSets",
        "Actual sets must be greater than zero");

    public static readonly Error InvalidActualReps = Error.Validation(
        "WorkoutSession.InvalidActualReps",
        "Actual reps must be greater than zero for reps and weight exercises");

    public static readonly Error InvalidActualWeight = Error.Validation(
        "WorkoutSession.InvalidActualWeight",
        "Actual weight must be zero or greater");

    public static readonly Error InvalidActualDuration = Error.Validation(
        "WorkoutSession.InvalidActualDuration",
        "Actual duration must be greater than zero for time-based exercises");
}
