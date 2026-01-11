using GymBuddy.Domain.Users;

namespace GymBuddy.Domain.WorkoutSessions;

public sealed class WorkoutSessionByIdSpec : SingleResultSpecification<WorkoutSession>
{
    public WorkoutSessionByIdSpec(WorkoutSessionId sessionId)
    {
        Query.Where(s => s.Id == sessionId);
    }
}

public sealed class WorkoutSessionsByClientIdSpec : Specification<WorkoutSession>
{
    public WorkoutSessionsByClientIdSpec(UserId clientId)
    {
        Query.Where(s => s.ClientId == clientId)
            .OrderByDescending(s => s.StartedAt);
    }
}

public sealed class ActiveWorkoutSessionByClientIdSpec : SingleResultSpecification<WorkoutSession>
{
    public ActiveWorkoutSessionByClientIdSpec(UserId clientId)
    {
        Query.Where(s => s.ClientId == clientId && s.Status == SessionStatus.InProgress)
            .OrderByDescending(s => s.StartedAt);
    }
}
