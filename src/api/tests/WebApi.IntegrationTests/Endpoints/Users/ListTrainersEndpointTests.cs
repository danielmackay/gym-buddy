using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using GymBuddy.Domain.Users;
using GymBuddy.Api.Features.Users.Endpoints;
using GymBuddy.Api.IntegrationTests.Common;
using GymBuddy.Api.IntegrationTests.Common.Factories;
using System.Net;

namespace GymBuddy.Api.IntegrationTests.Endpoints.Users;

public class ListTrainersEndpointTests : IntegrationTestBase
{
    [Test]
    public async Task ListTrainers_WithNoTrainers_ShouldReturnEmptyList()
    {
        // Arrange
        var client = GetAnonymousClient();

        // Act
        var result = await client.GETAsync<ListTrainersEndpoint, List<UserResponse>>();

        // Assert
        await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        await Assert.That(result.Result).IsNotNull();
        await Assert.That(result.Result!).IsEmpty();
    }

    [Test]
    public async Task ListTrainers_WithOneTrainer_ShouldReturnTrainer()
    {
        // Arrange
        var trainer = UserFactory.GenerateTrainer();
        await AddAsync(trainer);

        var client = GetAnonymousClient();

        // Act
        var result = await client.GETAsync<ListTrainersEndpoint, List<UserResponse>>();

        // Assert
        await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        await Assert.That(result.Result).IsNotNull();
        await Assert.That(result.Result!).HasCount().EqualTo(1);
        
        var returnedTrainer = result.Result.First();
        await Assert.That(returnedTrainer.Id).IsEqualTo(trainer.Id.Value);
        await Assert.That(returnedTrainer.Name).IsEqualTo(trainer.Name);
        await Assert.That(returnedTrainer.Email).IsEqualTo(trainer.Email);
        await Assert.That(returnedTrainer.Roles).Contains(UserRole.Trainer);
        await Assert.That(returnedTrainer.TrainerId).IsNull();
    }

    [Test]
    public async Task ListTrainers_WithMultipleTrainers_ShouldReturnAllTrainers()
    {
        // Arrange
        var trainers = UserFactory.GenerateTrainer();
        var trainer2 = UserFactory.GenerateTrainer();
        var trainer3 = UserFactory.GenerateTrainer();
        await AddRangeAsync(new[] { trainers, trainer2, trainer3 });

        var client = GetAnonymousClient();

        // Act
        var result = await client.GETAsync<ListTrainersEndpoint, List<UserResponse>>();

        // Assert
        await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        await Assert.That(result.Result).IsNotNull();
        await Assert.That(result.Result!).HasCount().EqualTo(3);
        
        var trainerIds = result.Result.Select(t => t.Id).ToList();
        await Assert.That(trainerIds).Contains(trainers.Id.Value);
        await Assert.That(trainerIds).Contains(trainer2.Id.Value);
        await Assert.That(trainerIds).Contains(trainer3.Id.Value);

        // All should have Trainer role
        foreach (var trainer in result.Result)
        {
            await Assert.That(trainer.Roles).Contains(UserRole.Trainer);
        }
    }

    [Test]
    public async Task ListTrainers_WithMixedUsers_ShouldReturnOnlyTrainers()
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
        var result = await client.GETAsync<ListTrainersEndpoint, List<UserResponse>>();

        // Assert
        await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        await Assert.That(result.Result).IsNotNull();
        await Assert.That(result.Result!).HasCount().EqualTo(2);
        
        var trainerIds = result.Result.Select(t => t.Id).ToList();
        await Assert.That(trainerIds).Contains(trainer1.Id.Value);
        await Assert.That(trainerIds).Contains(trainer2.Id.Value);
        await Assert.That(trainerIds).DoesNotContain(client1.Id.Value);
        await Assert.That(trainerIds).DoesNotContain(client2.Id.Value);
        await Assert.That(trainerIds).DoesNotContain(admin.Id.Value);
    }

    [Test]
    public async Task ListTrainers_WithClientsOnly_ShouldReturnEmptyList()
    {
        // Arrange
        var trainer = UserFactory.GenerateTrainer();
        var client1 = UserFactory.GenerateClientWithTrainer(trainer.Id);
        var client2 = UserFactory.GenerateClientWithTrainer(trainer.Id);
        await AddRangeAsync(new[] { trainer, client1, client2 });

        // Remove the trainer to leave only clients
        await RemoveAsync(trainer);

        var client = GetAnonymousClient();

        // Act
        var result = await client.GETAsync<ListTrainersEndpoint, List<UserResponse>>();

        // Assert
        await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        await Assert.That(result.Result).IsNotNull();
        await Assert.That(result.Result!).IsEmpty();
    }

    [Test]
    public async Task ListTrainers_WithAdminOnly_ShouldReturnEmptyList()
    {
        // Arrange
        var admin = UserFactory.GenerateAdmin();
        await AddAsync(admin);

        var client = GetAnonymousClient();

        // Act
        var result = await client.GETAsync<ListTrainersEndpoint, List<UserResponse>>();

        // Assert
        await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        await Assert.That(result.Result).IsNotNull();
        await Assert.That(result.Result!).IsEmpty();
    }
}
