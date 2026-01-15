using GymBuddy.Domain.Users;
using GymBuddy.Domain.WorkoutPlans;
using GymBuddy.Domain.WorkoutSessions;

#pragma warning disable CA1707

namespace GymBuddy.Api.IntegrationTests.Common.Factories;

public static class WorkoutSessionFactory
{
    public static WorkoutSession Generate(UserId clientId, WorkoutPlan workoutPlan, TimeProvider? timeProvider = null)
    {
        timeProvider ??= TimeProvider.System;

        var session = WorkoutSession.Start(
            clientId,
            workoutPlan,
            timeProvider
        );

        return session;
    }
}
