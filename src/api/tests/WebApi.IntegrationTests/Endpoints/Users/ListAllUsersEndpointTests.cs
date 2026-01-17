using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using GymBuddy.Domain.Users;
using GymBuddy.Api.Features.Users.Endpoints;
using GymBuddy.Api.IntegrationTests.Common;
using GymBuddy.Api.IntegrationTests.Common.Factories;
using System.Net;

namespace GymBuddy.Api.IntegrationTests.Endpoints.Users;

public class ListAllUsersEndpointTests : IntegrationTestBase
{
    [Test]
    public async Task ListAllUsers_WithNoUsers_ShouldReturnEmptyList()
    {
        // Arrange
        var client = GetAnonymousClient();

        // Act
        var result = await client.GETAsync<ListAllUsersEndpoint, List<UserResponse>>();

        // Assert
        await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        await Assert.That(result.Result).IsNotNull();
        await Assert.That(result.Result!).IsEmpty();
    }

    [Test]
    public async Task ListAllUsers_WithOneUser_ShouldReturnUser()
    {
        // Arrange
        var user = UserFactory.GenerateTrainer();
        await AddAsync(user);

        var client = GetAnonymousClient();

        // Act
        var result = await client.GETAsync<ListAllUsersEndpoint, List<UserResponse>>();

        // Assert
        await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        await Assert.That(result.Result).IsNotNull();
        await Assert.That(result.Result!).HasCount().EqualTo(1);
        
        var returnedUser = result.Result.First();
        await Assert.That(returnedUser.Id).IsEqualTo(user.Id.Value);
        await Assert.That(returnedUser.Name).IsEqualTo(user.Name);
        await Assert.That(returnedUser.Email).IsEqualTo(user.Email);
        await Assert.That(returnedUser.Roles).Contains(UserRole.Trainer);
    }

    [Test]
    public async Task ListAllUsers_WithMultipleUsers_ShouldReturnAllUsers()
    {
        // Arrange
        var trainer1 = UserFactory.GenerateTrainer();
        var trainer2 = UserFactory.GenerateTrainer();
        var client1 = UserFactory.GenerateClientWithTrainer(trainer1.Id);
        var client2 = UserFactory.GenerateClientWithTrainer(trainer2.Id);
        var admin = UserFactory.GenerateAdmin();
        await AddRangeAsync(new[] { trainer1, trainer2, client1, client2, admin });

        var client = GetAnonymousClient();

        // Act
        var result = await client.GETAsync<ListAllUsersEndpoint, List<UserResponse>>();

        // Assert
        await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        await Assert.That(result.Result).IsNotNull();
        await Assert.That(result.Result!).HasCount().EqualTo(5);
        
        var userIds = result.Result.Select(u => u.Id).ToList();
        await Assert.That(userIds).Contains(trainer1.Id.Value);
        await Assert.That(userIds).Contains(trainer2.Id.Value);
        await Assert.That(userIds).Contains(client1.Id.Value);
        await Assert.That(userIds).Contains(client2.Id.Value);
        await Assert.That(userIds).Contains(admin.Id.Value);
    }

    [Test]
    public async Task ListAllUsers_ShouldIncludeTrainers()
    {
        // Arrange
        var trainer1 = UserFactory.GenerateTrainer();
        var trainer2 = UserFactory.GenerateTrainer();
        await AddRangeAsync(new[] { trainer1, trainer2 });

        var client = GetAnonymousClient();

        // Act
        var result = await client.GETAsync<ListAllUsersEndpoint, List<UserResponse>>();

        // Assert
        await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        await Assert.That(result.Result).IsNotNull();
        await Assert.That(result.Result!).HasCount().EqualTo(2);
        
        var trainers = result.Result.Where(u => u.Roles.Contains(UserRole.Trainer)).ToList();
        await Assert.That(trainers).HasCount().EqualTo(2);
    }

    [Test]
    public async Task ListAllUsers_ShouldIncludeClients()
    {
        // Arrange
        var trainer = UserFactory.GenerateTrainer();
        var client1 = UserFactory.GenerateClientWithTrainer(trainer.Id);
        var client2 = UserFactory.GenerateClientWithTrainer(trainer.Id);
        await AddRangeAsync(new[] { trainer, client1, client2 });

        var client = GetAnonymousClient();

        // Act
        var result = await client.GETAsync<ListAllUsersEndpoint, List<UserResponse>>();

        // Assert
        await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        await Assert.That(result.Result).IsNotNull();
        await Assert.That(result.Result!).HasCount().EqualTo(3);
        
        var clients = result.Result.Where(u => u.Roles.Contains(UserRole.Client)).ToList();
        await Assert.That(clients).HasCount().EqualTo(2);
    }

    [Test]
    public async Task ListAllUsers_ShouldIncludeAdmins()
    {
        // Arrange
        var admin1 = UserFactory.GenerateAdmin();
        var admin2 = UserFactory.GenerateAdmin();
        await AddRangeAsync(new[] { admin1, admin2 });

        var client = GetAnonymousClient();

        // Act
        var result = await client.GETAsync<ListAllUsersEndpoint, List<UserResponse>>();

        // Assert
        await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        await Assert.That(result.Result).IsNotNull();
        await Assert.That(result.Result!).HasCount().EqualTo(2);
        
        var admins = result.Result.Where(u => u.Roles.Contains(UserRole.Admin)).ToList();
        await Assert.That(admins).HasCount().EqualTo(2);
    }

    [Test]
    public async Task ListAllUsers_ShouldIncludeTrainerIdsForClients()
    {
        // Arrange
        var trainer = UserFactory.GenerateTrainer();
        var client = UserFactory.GenerateClientWithTrainer(trainer.Id);
        await AddRangeAsync(new[] { trainer, client });

        var httpClient = GetAnonymousClient();

        // Act
        var result = await httpClient.GETAsync<ListAllUsersEndpoint, List<UserResponse>>();

        // Assert
        await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        await Assert.That(result.Result).IsNotNull();
        
        var clientUser = result.Result!.First(u => u.Id == client.Id.Value);
        await Assert.That(clientUser.TrainerId).IsNotNull();
        await Assert.That(clientUser.TrainerId).IsEqualTo(trainer.Id.Value);
    }

    [Test]
    public async Task ListAllUsers_ShouldNotIncludeTrainerIdsForTrainers()
    {
        // Arrange
        var trainer = UserFactory.GenerateTrainer();
        await AddAsync(trainer);

        var client = GetAnonymousClient();

        // Act
        var result = await client.GETAsync<ListAllUsersEndpoint, List<UserResponse>>();

        // Assert
        await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        await Assert.That(result.Result).IsNotNull();
        
        var trainerUser = result.Result!.First(u => u.Id == trainer.Id.Value);
        await Assert.That(trainerUser.TrainerId).IsNull();
    }

    [Test]
    public async Task ListAllUsers_WithLargeNumberOfUsers_ShouldReturnAllUsers()
    {
        // Arrange
        var users = new List<User>();
        for (int i = 0; i < 50; i++)
        {
            if (i % 3 == 0)
                users.Add(UserFactory.GenerateTrainer());
            else if (i % 3 == 1)
                users.Add(UserFactory.GenerateClient());
            else
                users.Add(UserFactory.GenerateAdmin());
        }
        await AddRangeAsync(users);

        var client = GetAnonymousClient();

        // Act
        var result = await client.GETAsync<ListAllUsersEndpoint, List<UserResponse>>();

        // Assert
        await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        await Assert.That(result.Result).IsNotNull();
        await Assert.That(result.Result!).HasCount().EqualTo(50);
    }
}
