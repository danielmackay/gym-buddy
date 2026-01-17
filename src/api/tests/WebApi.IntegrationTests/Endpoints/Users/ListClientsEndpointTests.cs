using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using GymBuddy.Domain.Users;
using GymBuddy.Api.Features.Users.Endpoints;
using GymBuddy.Api.IntegrationTests.Common;
using GymBuddy.Api.IntegrationTests.Common.Factories;
using System.Net;

namespace GymBuddy.Api.IntegrationTests.Endpoints.Users;

public class ListClientsEndpointTests : IntegrationTestBase
{
    [Test]
    public async Task ListClients_WithNoClients_ShouldReturnEmptyList()
    {
        // Arrange
        var trainer = UserFactory.GenerateTrainer();
        await AddAsync(trainer);

        var request = new ListClientsRequest { TrainerId = trainer.Id.Value };
        var client = GetAnonymousClient();

        // Act
        var result = await client.GETAsync<ListClientsEndpoint, ListClientsRequest, List<UserResponse>>(request);

        // Assert
        await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        await Assert.That(result.Result).IsNotNull();
        await Assert.That(result.Result!).IsEmpty();
    }

    [Test]
    public async Task ListClients_WithOneClient_ShouldReturnClient()
    {
        // Arrange
        var trainer = UserFactory.GenerateTrainer();
        var clientUser = UserFactory.GenerateClientWithTrainer(trainer.Id);
        await AddRangeAsync(new[] { trainer, clientUser });

        var request = new ListClientsRequest { TrainerId = trainer.Id.Value };
        var client = GetAnonymousClient();

        // Act
        var result = await client.GETAsync<ListClientsEndpoint, ListClientsRequest, List<UserResponse>>(request);

        // Assert
        await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        await Assert.That(result.Result).IsNotNull();
        await Assert.That(result.Result!).HasCount().EqualTo(1);
        
        var returnedClient = result.Result.First();
        await Assert.That(returnedClient.Id).IsEqualTo(clientUser.Id.Value);
        await Assert.That(returnedClient.Name).IsEqualTo(clientUser.Name);
        await Assert.That(returnedClient.Email).IsEqualTo(clientUser.Email);
        await Assert.That(returnedClient.Roles).Contains(UserRole.Client);
        await Assert.That(returnedClient.TrainerId).IsEqualTo(trainer.Id.Value);
    }

    [Test]
    public async Task ListClients_WithMultipleClientsForSameTrainer_ShouldReturnAllClients()
    {
        // Arrange
        var trainer = UserFactory.GenerateTrainer();
        var client1 = UserFactory.GenerateClientWithTrainer(trainer.Id);
        var client2 = UserFactory.GenerateClientWithTrainer(trainer.Id);
        var client3 = UserFactory.GenerateClientWithTrainer(trainer.Id);
        await AddRangeAsync(new[] { trainer, client1, client2, client3 });

        var request = new ListClientsRequest { TrainerId = trainer.Id.Value };
        var client = GetAnonymousClient();

        // Act
        var result = await client.GETAsync<ListClientsEndpoint, ListClientsRequest, List<UserResponse>>(request);

        // Assert
        await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        await Assert.That(result.Result).IsNotNull();
        await Assert.That(result.Result!).HasCount().EqualTo(3);
        
        var clientIds = result.Result.Select(c => c.Id).ToList();
        await Assert.That(clientIds).Contains(client1.Id.Value);
        await Assert.That(clientIds).Contains(client2.Id.Value);
        await Assert.That(clientIds).Contains(client3.Id.Value);

        // All should have Client role and be assigned to the trainer
        foreach (var c in result.Result)
        {
            await Assert.That(c.Roles).Contains(UserRole.Client);
            await Assert.That(c.TrainerId).IsEqualTo(trainer.Id.Value);
        }
    }

    [Test]
    public async Task ListClients_WithMultipleTrainers_ShouldReturnOnlyClientsForSpecifiedTrainer()
    {
        // Arrange
        var trainer1 = UserFactory.GenerateTrainer();
        var trainer2 = UserFactory.GenerateTrainer();
        var client1ForTrainer1 = UserFactory.GenerateClientWithTrainer(trainer1.Id);
        var client2ForTrainer1 = UserFactory.GenerateClientWithTrainer(trainer1.Id);
        var client1ForTrainer2 = UserFactory.GenerateClientWithTrainer(trainer2.Id);
        var client2ForTrainer2 = UserFactory.GenerateClientWithTrainer(trainer2.Id);
        await AddRangeAsync(new[] { trainer1, trainer2, client1ForTrainer1, client2ForTrainer1, client1ForTrainer2, client2ForTrainer2 });

        var request = new ListClientsRequest { TrainerId = trainer1.Id.Value };
        var client = GetAnonymousClient();

        // Act
        var result = await client.GETAsync<ListClientsEndpoint, ListClientsRequest, List<UserResponse>>(request);

        // Assert
        await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        await Assert.That(result.Result).IsNotNull();
        await Assert.That(result.Result!).HasCount().EqualTo(2);
        
        var clientIds = result.Result.Select(c => c.Id).ToList();
        await Assert.That(clientIds).Contains(client1ForTrainer1.Id.Value);
        await Assert.That(clientIds).Contains(client2ForTrainer1.Id.Value);
        await Assert.That(clientIds).DoesNotContain(client1ForTrainer2.Id.Value);
        await Assert.That(clientIds).DoesNotContain(client2ForTrainer2.Id.Value);
    }

    [Test]
    public async Task ListClients_WithNonExistentTrainer_ShouldReturnEmptyList()
    {
        // Arrange
        var trainer = UserFactory.GenerateTrainer();
        var client = UserFactory.GenerateClientWithTrainer(trainer.Id);
        await AddRangeAsync(new[] { trainer, client });

        var nonExistentTrainerId = Guid.NewGuid();
        var request = new ListClientsRequest { TrainerId = nonExistentTrainerId };
        var httpClient = GetAnonymousClient();

        // Act
        var result = await httpClient.GETAsync<ListClientsEndpoint, ListClientsRequest, List<UserResponse>>(request);

        // Assert
        await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        await Assert.That(result.Result).IsNotNull();
        await Assert.That(result.Result!).IsEmpty();
    }

    [Test]
    public async Task ListClients_WithEmptyTrainerId_ShouldReturnValidationError()
    {
        // Arrange
        var request = new ListClientsRequest { TrainerId = Guid.Empty };
        var client = GetAnonymousClient();

        // Act
        var result = await client.GETAsync<ListClientsEndpoint, ListClientsRequest, List<UserResponse>>(request);

        // Assert
        await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task ListClients_ShouldNotIncludeTrainers()
    {
        // Arrange
        var trainer1 = UserFactory.GenerateTrainer();
        var trainer2 = UserFactory.GenerateTrainer();
        var client = UserFactory.GenerateClientWithTrainer(trainer1.Id);
        await AddRangeAsync(new[] { trainer1, trainer2, client });

        var request = new ListClientsRequest { TrainerId = trainer1.Id.Value };
        var httpClient = GetAnonymousClient();

        // Act
        var result = await httpClient.GETAsync<ListClientsEndpoint, ListClientsRequest, List<UserResponse>>(request);

        // Assert
        await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        await Assert.That(result.Result).IsNotNull();
        await Assert.That(result.Result!).HasCount().EqualTo(1);
        await Assert.That(result.Result.First().Id).IsEqualTo(client.Id.Value);
    }

    [Test]
    public async Task ListClients_ShouldNotIncludeAdmins()
    {
        // Arrange
        var trainer = UserFactory.GenerateTrainer();
        var client = UserFactory.GenerateClientWithTrainer(trainer.Id);
        var admin = UserFactory.GenerateAdmin();
        await AddRangeAsync(new[] { trainer, client, admin });

        var request = new ListClientsRequest { TrainerId = trainer.Id.Value };
        var httpClient = GetAnonymousClient();

        // Act
        var result = await httpClient.GETAsync<ListClientsEndpoint, ListClientsRequest, List<UserResponse>>(request);

        // Assert
        await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        await Assert.That(result.Result).IsNotNull();
        await Assert.That(result.Result!).HasCount().EqualTo(1);
        await Assert.That(result.Result.First().Id).IsEqualTo(client.Id.Value);
    }
}
