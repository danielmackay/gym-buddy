using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using GymBuddy.Domain.Users;
using GymBuddy.Api.Features.WorkoutPlans.Endpoints;
using GymBuddy.Api.IntegrationTests.Common;
using GymBuddy.Api.IntegrationTests.Common.Factories;
using System.Net;

namespace GymBuddy.Api.IntegrationTests.Endpoints.WorkoutPlans;

public class AssignPlanToClientEndpointTests : IntegrationTestBase
{
    [Test]
    public async Task AssignPlanToClient_WithValidIds_ShouldAssignPlan()
    {
        // Arrange
        var trainer = UserFactory.GenerateTrainer();
        var client = UserFactory.GenerateClient();
        await AddRangeAsync([trainer, client]);

        var plan = WorkoutPlanFactory.Generate(trainer.Id);
        await AddAsync(plan);

        var request = new AssignPlanToClientRequest(
            ClientId: client.Id.Value,
            WorkoutPlanId: plan.Id.Value);
        var httpClient = GetAnonymousClient();

        // Act
        var result = await httpClient.POSTAsync<AssignPlanToClientEndpoint, AssignPlanToClientRequest>(request);

        // Assert
        await Assert.That(result.StatusCode).IsEqualTo(HttpStatusCode.NoContent);

        // Verify the plan was assigned to the client
        var updatedClient = await GetQueryable<User>()
            .FirstOrDefaultAsync(u => u.Id == client.Id, CancellationToken);

        await Assert.That(updatedClient).IsNotNull();
        await Assert.That(updatedClient!.AssignedWorkoutPlanIds.Count).IsEqualTo(1);
        await Assert.That(updatedClient.AssignedWorkoutPlanIds[0]).IsEqualTo(plan.Id);
    }

    [Test]
    public async Task AssignPlanToClient_MultipleToSameClient_ShouldAssignAllPlans()
    {
        // Arrange
        var trainer = UserFactory.GenerateTrainer();
        var client = UserFactory.GenerateClient();
        await AddRangeAsync([trainer, client]);

        var plan1 = WorkoutPlanFactory.Generate(trainer.Id);
        var plan2 = WorkoutPlanFactory.Generate(trainer.Id);
        await AddRangeAsync([plan1, plan2]);

        var httpClient = GetAnonymousClient();

        // Act - Assign first plan
        var request1 = new AssignPlanToClientRequest(
            ClientId: client.Id.Value,
            WorkoutPlanId: plan1.Id.Value);
        await httpClient.POSTAsync<AssignPlanToClientEndpoint, AssignPlanToClientRequest>(request1);

        // Assign second plan
        var request2 = new AssignPlanToClientRequest(
            ClientId: client.Id.Value,
            WorkoutPlanId: plan2.Id.Value);
        await httpClient.POSTAsync<AssignPlanToClientEndpoint, AssignPlanToClientRequest>(request2);

        // Assert
        var updatedClient = await GetQueryable<User>()
            .FirstOrDefaultAsync(u => u.Id == client.Id, CancellationToken);

        await Assert.That(updatedClient).IsNotNull();
        await Assert.That(updatedClient!.AssignedWorkoutPlanIds.Count).IsEqualTo(2);
        await Assert.That(updatedClient.AssignedWorkoutPlanIds).Contains(plan1.Id);
        await Assert.That(updatedClient.AssignedWorkoutPlanIds).Contains(plan2.Id);
    }

    [Test]
    public async Task AssignPlanToClient_WithInvalidClientId_ShouldReturnError()
    {
        // Arrange
        var trainer = UserFactory.GenerateTrainer();
        await AddAsync(trainer);

        var plan = WorkoutPlanFactory.Generate(trainer.Id);
        await AddAsync(plan);

        var request = new AssignPlanToClientRequest(
            ClientId: Guid.NewGuid(),
            WorkoutPlanId: plan.Id.Value);
        var httpClient = GetAnonymousClient();

        // Act
        var result = await httpClient.POSTAsync<AssignPlanToClientEndpoint, AssignPlanToClientRequest>(request);

        // Assert
        await Assert.That(result.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task AssignPlanToClient_WithInvalidPlanId_ShouldReturnError()
    {
        // Arrange
        var client = UserFactory.GenerateClient();
        await AddAsync(client);

        var request = new AssignPlanToClientRequest(
            ClientId: client.Id.Value,
            WorkoutPlanId: Guid.NewGuid());
        var httpClient = GetAnonymousClient();

        // Act
        var result = await httpClient.POSTAsync<AssignPlanToClientEndpoint, AssignPlanToClientRequest>(request);

        // Assert
        await Assert.That(result.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task AssignPlanToClient_AssigningSamePlanTwice_ShouldReturnError()
    {
        // Arrange
        var trainer = UserFactory.GenerateTrainer();
        var client = UserFactory.GenerateClient();
        await AddRangeAsync([trainer, client]);

        var plan = WorkoutPlanFactory.Generate(trainer.Id);
        await AddAsync(plan);

        var request = new AssignPlanToClientRequest(
            ClientId: client.Id.Value,
            WorkoutPlanId: plan.Id.Value);
        var httpClient = GetAnonymousClient();

        // Act - Assign the plan
        await httpClient.POSTAsync<AssignPlanToClientEndpoint, AssignPlanToClientRequest>(request);

        // Try to assign the same plan again
        var result = await httpClient.POSTAsync<AssignPlanToClientEndpoint, AssignPlanToClientRequest>(request);

        // Assert
        await Assert.That(result.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task AssignPlanToClient_WithEmptyClientId_ShouldReturnValidationError()
    {
        // Arrange
        var request = new AssignPlanToClientRequest(
            ClientId: Guid.Empty,
            WorkoutPlanId: Guid.NewGuid());
        var httpClient = GetAnonymousClient();

        // Act
        var result = await httpClient.POSTAsync<AssignPlanToClientEndpoint, AssignPlanToClientRequest>(request);

        // Assert
        await Assert.That(result.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task AssignPlanToClient_WithEmptyPlanId_ShouldReturnValidationError()
    {
        // Arrange
        var request = new AssignPlanToClientRequest(
            ClientId: Guid.NewGuid(),
            WorkoutPlanId: Guid.Empty);
        var httpClient = GetAnonymousClient();

        // Act
        var result = await httpClient.POSTAsync<AssignPlanToClientEndpoint, AssignPlanToClientRequest>(request);

        // Assert
        await Assert.That(result.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
    }
}
