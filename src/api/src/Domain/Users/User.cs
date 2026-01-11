using GymBuddy.Domain.Base;
using GymBuddy.Domain.WorkoutPlans;

namespace GymBuddy.Domain.Users;

[ValueObject<Guid>]
public readonly partial struct UserId;

public class User : AggregateRoot<UserId>
{
    public const int NameMaxLength = 100;
    public const int EmailMaxLength = 256;

    private readonly List<UserRole> _roles = [];
    private readonly List<WorkoutPlanId> _assignedWorkoutPlanIds = [];

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

    public string Email
    {
        get;
        set
        {
            ThrowIfNullOrWhiteSpace(value, nameof(Email));
            ThrowIfGreaterThan(value.Length, EmailMaxLength, nameof(Email));
            field = value;
        }
    } = null!;

    public UserId? TrainerId { get; private set; }

    public IReadOnlyList<UserRole> Roles => _roles.AsReadOnly();
    public IReadOnlyList<WorkoutPlanId> AssignedWorkoutPlanIds => _assignedWorkoutPlanIds.AsReadOnly();

    private User() { } // Needed for EF Core

    public static User Create(string name, string email)
    {
        var user = new User
        {
            Id = UserId.From(Guid.CreateVersion7()),
            Name = name,
            Email = email
        };

        return user;
    }

    public ErrorOr<Success> AddRole(UserRole role)
    {
        if (_roles.Contains(role))
            return UserErrors.AlreadyHasRole;

        _roles.Add(role);
        return new Success();
    }

    public ErrorOr<Success> RemoveRole(UserRole role)
    {
        if (!_roles.Contains(role))
            return UserErrors.DoesNotHaveRole;

        _roles.Remove(role);

        // If removing Client role, clear trainer and assigned plans
        if (role == UserRole.Client)
        {
            TrainerId = null;
            _assignedWorkoutPlanIds.Clear();
        }

        return new Success();
    }

    public ErrorOr<Success> AssignTrainer(UserId trainerId)
    {
        if (!_roles.Contains(UserRole.Client))
            return UserErrors.NotAClient;

        if (Id == trainerId)
            return UserErrors.CannotAssignSelfAsTrainer;

        TrainerId = trainerId;
        return new Success();
    }

    public void UnassignTrainer()
    {
        TrainerId = null;
    }

    public ErrorOr<Success> AssignWorkoutPlan(WorkoutPlanId workoutPlanId)
    {
        if (!_roles.Contains(UserRole.Client))
            return UserErrors.NotAClient;

        if (_assignedWorkoutPlanIds.Contains(workoutPlanId))
            return UserErrors.WorkoutPlanAlreadyAssigned;

        _assignedWorkoutPlanIds.Add(workoutPlanId);
        return new Success();
    }

    public ErrorOr<Success> UnassignWorkoutPlan(WorkoutPlanId workoutPlanId)
    {
        if (!_assignedWorkoutPlanIds.Contains(workoutPlanId))
            return UserErrors.WorkoutPlanNotAssigned;

        _assignedWorkoutPlanIds.Remove(workoutPlanId);
        return new Success();
    }

    public bool HasRole(UserRole role) => _roles.Contains(role);
}
