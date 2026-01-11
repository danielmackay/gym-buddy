using GymBuddy.Domain.Users;

// Preserve the namespace across partial classes
// ReSharper disable once CheckNamespace
namespace GymBuddy.Api.Common.Persistence;

public partial class ApplicationDbContext
{
    public DbSet<User> Users => AggregateRootSet<User>();
}
