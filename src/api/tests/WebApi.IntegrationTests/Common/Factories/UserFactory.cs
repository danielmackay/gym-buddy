using GymBuddy.Domain.Users;

#pragma warning disable CA1707

namespace GymBuddy.Api.IntegrationTests.Common.Factories;

public static class UserFactory
{
    private static readonly Faker<User> UserFaker = new Faker<User>().CustomInstantiator(f =>
    {
        var user = User.Create(
            f.Person.FullName,
            f.Person.Email
        );

        return user;
    });

    public static User Generate() => UserFaker.Generate();

    public static IReadOnlyList<User> Generate(int count) => UserFaker.Generate(count);

    public static User GenerateTrainer()
    {
        var user = Generate();
        user.AddRole(UserRole.Trainer);
        return user;
    }

    public static User GenerateClient()
    {
        var user = Generate();
        user.AddRole(UserRole.Client);
        return user;
    }

    public static User GenerateAdmin()
    {
        var user = Generate();
        user.AddRole(UserRole.Admin);
        return user;
    }

    public static User GenerateClientWithTrainer(UserId trainerId)
    {
        var client = GenerateClient();
        client.AssignTrainer(trainerId);
        return client;
    }
}
