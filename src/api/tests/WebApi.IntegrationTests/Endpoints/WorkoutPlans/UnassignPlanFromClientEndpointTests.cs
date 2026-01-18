using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using GymBuddy.Domain.Users;
using GymBuddy.Api.Features.WorkoutPlans.Endpoints;
using GymBuddy.Api.IntegrationTests.Common;
using GymBuddy.Api.IntegrationTests.Common.Factories;
using System.Net;

namespace GymBuddy.Api.IntegrationTests.Endpoints.WorkoutPlans;

public class UnassignPlanFromClientEndpointTests : IntegrationTestBase
{
    [Test]
    public async Task UnassignPlanFromClient_WithValidIds_ShouldUnassignPlan()
    {
        // Arrange
        var trainer = UserFactory.GenerateTrainer();
        var client = UserFactory.GenerateClient();
        await AddAsync(trainer);

        var plan = WorkoutPlanFactory.Generate(trainer.Id);
        await AddAsync(plan);

        // Assign the plan first
        client.AssignWorkoutPlan(plan.Id);
        await AddAsync(client);

        var request = new UnassignPlanFromClientRequest(
            ClientId: client.Id.Value,
            WorkoutPlanId: plan.Id.Value);
        var httpClient = GetAnonymousClient();

        // Act
        var result = await httpClient.POSTAsync<UnassignPlanFromClientEndpoint, UnassignPlanFromClientRequest>(request);

        // Assert
        await Assert.That(result.StatusCode).IsEqualTo(HttpStatusCode.NoContent);

        // Verify the plan was unassigned from the client
        var updatedClient = await GetQueryable<User>()
            .FirstOrDefaultAsync(u => u.Id == client.Id, CancellationToken);

        await Assert.That(updatedClient).IsNotNull();
        await Assert.That(updatedClient!.AssignedWorkoutPlanIds.Count).IsEqualTo(0);
    }

    [Test]
    public async Task UnassignPlanFromClient_WithMultiplePlans_ShouldOnlyUnassignSpecified()
    {
        // Arrange
        var trainer = UserFactory.GenerateTrainer();
        var client = UserFactory.GenerateClient();
        await AddAsync(trainer);

        var plan1 = WorkoutPlanFactory.Generate(trainer.Id);
        var plan2 = WorkoutPlanFactory.Generate(trainer.Id);
        await AddRangeAsync([plan1, plan2]);

        // Assign both plans
        client.AssignWorkoutPlan(plan1.Id);
        client.AssignWorkoutPlan(plan2.Id);
        await AddAsync(client);

        var request = new UnassignPlanFromClientRequest(
            ClientId: client.Id.Value,
            WorkoutPlanId: plan1.Id.Value);
        var httpClient = GetAnonymousClient();

        // Act
        var result = await httpClient.POSTAsync<UnassignPlanFromClientEndpoint, UnassignPlanFromClientRequest>(request);

        // Assert
        await Assert.That(result.StatusCode).IsEqualTo(HttpStatusCode.NoContent);

        // Verify only plan1 was unassigned
        var updatedClient = await GetQueryable<User>()
            .FirstOrDefaultAsync(u => u.Id == client.Id, CancellationToken);

        await Assert.That(updatedClient).IsNotNull();
        await Assert.That(updatedClient!.AssignedWorkoutPlanIds.Count).IsEqualTo(1);
        await Assert.That(updatedClient.AssignedWorkoutPlanIds[0]).IsEqualTo(plan2.Id);
    }

    [Test]
    public async Task UnassignPlanFromClient_WithInvalidClientId_ShouldReturnError()
    {
        // Arrange
        var trainer = UserFactory.GenerateTrainer();
        await AddAsync(trainer);

        var plan = WorkoutPlanFactory.Generate(trainer.Id);
        await AddAsync(plan);

        var request = new UnassignPlanFromClientRequest(
            ClientId: Guid.NewGuid(),
            WorkoutPlanId: plan.Id.Value);
        var httpClient = GetAnonymousClient();

        // Act
        var result = await httpClient.POSTAsync<UnassignPlanFromClientEndpoint, UnassignPlanFromClientRequest>(request);

        // Assert
        await Assert.That(result.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task UnassignPlanFromClient_WithInvalidPlanId_ShouldReturnError()
    {
        // Arrange
        var client = UserFactory.GenerateClient();
        await AddAsync(client);

        var request = new UnassignPlanFromClientRequest(
            ClientId: client.Id.Value,
            WorkoutPlanId: Guid.NewGuid());
        var httpClient = GetAnonymousClient();

        // Act
        var result = await httpClient.POSTAsync<UnassignPlanFromClientEndpoint, UnassignPlanFromClientRequest>(request);

        // Assert
        await Assert.That(result.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task UnassignPlanFromClient_PlanNotAssignedToClient_ShouldReturnError()
    {
        // Arrange
        var trainer = UserFactory.GenerateTrainer();
        var client = UserFactory.GenerateClient();
        await AddRangeAsync([trainer, client]);

        var plan = WorkoutPlanFactory.Generate(trainer.Id);
        await AddAsync(plan);

        // Don't assign the plan - try to unassign it directly
        var request = new UnassignPlanFromClientRequest(
            ClientId: client.Id.Value,
            WorkoutPlanId: plan.Id.Value);
        var httpClient = GetAnonymousClient();

        // Act
        var result = await httpClient.POSTAsync<UnassignPlanFromClientEndpoint, UnassignPlanFromClientRequest>(request);

        // Assert
        await Assert.That(result.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task UnassignPlanFromClient_WithEmptyClientId_ShouldReturnValidationError()
    {
        // Arrange
        var request = new UnassignPlanFromClientRequest(
            ClientId: Guid.Empty,
            WorkoutPlanId: Guid.NewGuid());
        var httpClient = GetAnonymousClient();

        // Act
        var result = await httpClient.POSTAsync<UnassignPlanFromClientEndpoint, UnassignPlanFromClientRequest>(request);

        // Assert
        await Assert.That(result.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task UnassignPlanFromClient_WithEmptyPlanId_ShouldReturnValidationError()
    {
        // Arrange
        var request = new UnassignPlanFromClientRequest(
            ClientId: Guid.NewGuid(),
            WorkoutPlanId: Guid.Empty);
        var httpClient = GetAnonymousClient();

        // Act
        var result = await httpClient.POSTAsync<UnassignPlanFromClientEndpoint, UnassignPlanFromClientRequest>(request);

        // Assert
        await Assert.That(result.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
    }
}
