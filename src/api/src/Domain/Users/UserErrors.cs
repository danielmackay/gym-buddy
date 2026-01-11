namespace GymBuddy.Domain.Users;

public static class UserErrors
{
    public static readonly Error NotFound = Error.NotFound(
        "User.NotFound",
        "User is not found");

    public static readonly Error NotAClient = Error.Validation(
        "User.NotAClient",
        "User must have the Client role to perform this action");

    public static readonly Error NotATrainer = Error.Validation(
        "User.NotATrainer",
        "User must have the Trainer role to perform this action");

    public static readonly Error AlreadyHasRole = Error.Conflict(
        "User.AlreadyHasRole",
        "User already has this role");

    public static readonly Error DoesNotHaveRole = Error.Conflict(
        "User.DoesNotHaveRole",
        "User does not have this role");

    public static readonly Error CannotAssignSelfAsTrainer = Error.Validation(
        "User.CannotAssignSelfAsTrainer",
        "A user cannot be their own trainer");

    public static readonly Error WorkoutPlanAlreadyAssigned = Error.Conflict(
        "User.WorkoutPlanAlreadyAssigned",
        "This workout plan is already assigned to the user");

    public static readonly Error WorkoutPlanNotAssigned = Error.Conflict(
        "User.WorkoutPlanNotAssigned",
        "This workout plan is not assigned to the user");
}
