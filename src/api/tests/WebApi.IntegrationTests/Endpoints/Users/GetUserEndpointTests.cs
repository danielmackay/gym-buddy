using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using GymBuddy.Domain.Users;
using GymBuddy.Api.Features.Users.Endpoints;
using GymBuddy.Api.IntegrationTests.Common;
using GymBuddy.Api.IntegrationTests.Common.Factories;
using System.Net;

namespace GymBuddy.Api.IntegrationTests.Endpoints.Users;

public class GetUserEndpointTests : IntegrationTestBase
{
    [Test]
    public async Task GetUser_WithExistingTrainer_ShouldReturnUser()
    {
        // Arrange
        var trainer = UserFactory.GenerateTrainer();
        await AddAsync(trainer);

        var request = new GetUserRequest(trainer.Id.Value);
        var client = GetAnonymousClient();

        // Act
        var result = await client.GETAsync<GetUserEndpoint, GetUserRequest, UserResponse>(request);

        // Assert
        await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        await Assert.That(result.Result).IsNotNull();
        await Assert.That(result.Result!.Id).IsEqualTo(trainer.Id.Value);
        await Assert.That(result.Result.Name).IsEqualTo(trainer.Name);
        await Assert.That(result.Result.Email).IsEqualTo(trainer.Email);
        await Assert.That(result.Result.Roles).Contains(UserRole.Trainer);
        await Assert.That(result.Result.TrainerId).IsNull();
        await Assert.That(result.Result.AssignedWorkoutPlanIds).IsEmpty();
    }

    [Test]
    public async Task GetUser_WithExistingClient_ShouldReturnUserWithTrainer()
    {
        // Arrange
        var trainer = UserFactory.GenerateTrainer();
        var client = UserFactory.GenerateClientWithTrainer(trainer.Id);
        await AddRangeAsync(new[] { trainer, client });

        var request = new GetUserRequest(client.Id.Value);
        var httpClient = GetAnonymousClient();

        // Act
        var result = await httpClient.GETAsync<GetUserEndpoint, GetUserRequest, UserResponse>(request);

        // Assert
        await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        await Assert.That(result.Result).IsNotNull();
        await Assert.That(result.Result!.Id).IsEqualTo(client.Id.Value);
        await Assert.That(result.Result.Name).IsEqualTo(client.Name);
        await Assert.That(result.Result.Email).IsEqualTo(client.Email);
        await Assert.That(result.Result.Roles).Contains(UserRole.Client);
        await Assert.That(result.Result.TrainerId).IsNotNull();
        await Assert.That(result.Result.TrainerId).IsEqualTo(trainer.Id.Value);
        await Assert.That(result.Result.AssignedWorkoutPlanIds).IsEmpty();
    }

    [Test]
    public async Task GetUser_WithNonExistentId_ShouldReturnNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var request = new GetUserRequest(nonExistentId);
        var client = GetAnonymousClient();

        // Act
        var result = await client.GETAsync<GetUserEndpoint, GetUserRequest, UserResponse>(request);

        // Assert
        await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task GetUser_WithAdmin_ShouldReturnAdminUser()
    {
        // Arrange
        var admin = UserFactory.GenerateAdmin();
        await AddAsync(admin);

        var request = new GetUserRequest(admin.Id.Value);
        var client = GetAnonymousClient();

        // Act
        var result = await client.GETAsync<GetUserEndpoint, GetUserRequest, UserResponse>(request);

        // Assert
        await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        await Assert.That(result.Result).IsNotNull();
        await Assert.That(result.Result!.Id).IsEqualTo(admin.Id.Value);
        await Assert.That(result.Result.Roles).Contains(UserRole.Admin);
    }

    [Test]
    public async Task GetUser_WithEmptyId_ShouldReturnNotFound()
    {
        // Arrange
        var request = new GetUserRequest(Guid.Empty);
        var client = GetAnonymousClient();

        // Act
        var result = await client.GETAsync<GetUserEndpoint, GetUserRequest, UserResponse>(request);

        // Assert
        await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.NotFound);
    }
}
