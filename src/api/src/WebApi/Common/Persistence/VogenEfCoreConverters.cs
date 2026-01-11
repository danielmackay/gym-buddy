using GymBuddy.Domain.Heroes;
using GymBuddy.Domain.Teams;

namespace GymBuddy.Api.Common.Persistence;

// TODO: New strongly typed IDs should be registered here

[EfCoreConverter<HeroId>]
[EfCoreConverter<TeamId>]
[EfCoreConverter<MissionId>]
internal sealed partial class VogenEfCoreConverters;