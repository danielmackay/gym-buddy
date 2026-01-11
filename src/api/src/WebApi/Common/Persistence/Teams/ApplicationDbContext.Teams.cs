using GymBuddy.Api.Common.Domain.Teams;

// Preserve the namespace across partial classes
// ReSharper disable once CheckNamespace
namespace GymBuddy.Api.Common.Persistence;

public partial class ApplicationDbContext
{
    public DbSet<Team> Teams => AggregateRootSet<Team>();
}