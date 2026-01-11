namespace GymBuddy.Domain.Users;

public sealed class UserByIdSpec : SingleResultSpecification<User>
{
    public UserByIdSpec(UserId userId)
    {
        Query.Where(u => u.Id == userId);
    }
}
