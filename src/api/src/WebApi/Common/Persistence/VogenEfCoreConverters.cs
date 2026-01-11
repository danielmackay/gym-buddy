using GymBuddy.Domain.Exercises;
using GymBuddy.Domain.Heroes;
using GymBuddy.Domain.Teams;
using GymBuddy.Domain.Users;
using GymBuddy.Domain.WorkoutPlans;
using GymBuddy.Domain.WorkoutSessions;

namespace GymBuddy.Api.Common.Persistence;

// TODO: New strongly typed IDs should be registered here

[EfCoreConverter<HeroId>]
[EfCoreConverter<TeamId>]
[EfCoreConverter<MissionId>]
[EfCoreConverter<UserId>]
[EfCoreConverter<ExerciseId>]
[EfCoreConverter<WorkoutPlanId>]
[EfCoreConverter<PlannedExerciseId>]
[EfCoreConverter<WorkoutSessionId>]
[EfCoreConverter<SessionExerciseId>]
internal sealed partial class VogenEfCoreConverters;