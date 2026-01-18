using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using GymBuddy.Domain.WorkoutPlans;
using GymBuddy.Api.Features.WorkoutPlans.Endpoints;
using GymBuddy.Api.IntegrationTests.Common;
using GymBuddy.Api.IntegrationTests.Common.Factories;
using System.Net;

namespace GymBuddy.Api.IntegrationTests.Endpoints.WorkoutPlans;

public class UpdateWorkoutPlanEndpointTests : IntegrationTestBase
{
    [Test]
    public async Task UpdateWorkoutPlan_WithValidRequest_ShouldUpdateWorkoutPlan()
    {
        // Arrange
        var trainer = UserFactory.GenerateTrainer();
        await AddAsync(trainer);

        var plan = WorkoutPlanFactory.Generate(trainer.Id);
        await AddAsync(plan);

        var request = new UpdateWorkoutPlanRequest(
            Id: plan.Id.Value,
            Name: "Updated Name",
            Description: "Updated Description");
        var client = GetAnonymousClient();

        // Act
        var result = await client.PUTAsync<UpdateWorkoutPlanEndpoint, UpdateWorkoutPlanRequest>(request);

        // Assert
        await Assert.That(result.StatusCode).IsEqualTo(HttpStatusCode.NoContent);

        // Verify the workout plan was updated in the database
        var updatedPlan = await GetQueryable<WorkoutPlan>().FirstOrDefaultAsync(wp => wp.Id == plan.Id, CancellationToken);
        await Assert.That(updatedPlan).IsNotNull();
        await Assert.That(updatedPlan!.Name).IsEqualTo("Updated Name");
        await Assert.That(updatedPlan.Description).IsEqualTo("Updated Description");
    }

    [Test]
    public async Task UpdateWorkoutPlan_WithNullDescription_ShouldClearDescription()
    {
        // Arrange
        var trainer = UserFactory.GenerateTrainer();
        await AddAsync(trainer);

        var plan = WorkoutPlanFactory.Generate(trainer.Id);
        await AddAsync(plan);

        var request = new UpdateWorkoutPlanRequest(
            Id: plan.Id.Value,
            Name: "Updated Name",
            Description: null);
        var client = GetAnonymousClient();

        // Act
        var result = await client.PUTAsync<UpdateWorkoutPlanEndpoint, UpdateWorkoutPlanRequest>(request);

        // Assert
        await Assert.That(result.StatusCode).IsEqualTo(HttpStatusCode.NoContent);

        var updatedPlan = await GetQueryable<WorkoutPlan>().FirstOrDefaultAsync(wp => wp.Id == plan.Id, CancellationToken);
        await Assert.That(updatedPlan).IsNotNull();
        await Assert.That(updatedPlan!.Description).IsNull();
    }

    [Test]
    public async Task UpdateWorkoutPlan_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var request = new UpdateWorkoutPlanRequest(
            Id: nonExistentId,
            Name: "Updated Name",
            Description: "Updated Description");
        var client = GetAnonymousClient();

        // Act
        var result = await client.PUTAsync<UpdateWorkoutPlanEndpoint, UpdateWorkoutPlanRequest>(request);

        // Assert
        await Assert.That(result.StatusCode).IsEqualTo(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task UpdateWorkoutPlan_WithEmptyName_ShouldReturnValidationError()
    {
        // Arrange
        var trainer = UserFactory.GenerateTrainer();
        await AddAsync(trainer);

        var plan = WorkoutPlanFactory.Generate(trainer.Id);
        await AddAsync(plan);

        var request = new UpdateWorkoutPlanRequest(
            Id: plan.Id.Value,
            Name: "",
            Description: "Description");
        var client = GetAnonymousClient();

        // Act
        var result = await client.PUTAsync<UpdateWorkoutPlanEndpoint, UpdateWorkoutPlanRequest>(request);

        // Assert
        await Assert.That(result.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);

        // Verify the workout plan was not updated
        var unchangedPlan = await GetQueryable<WorkoutPlan>().FirstOrDefaultAsync(wp => wp.Id == plan.Id, CancellationToken);
        await Assert.That(unchangedPlan).IsNotNull();
        await Assert.That(unchangedPlan!.Name).IsEqualTo(plan.Name);
    }

    [Test]
    public async Task UpdateWorkoutPlan_WithNameTooLong_ShouldReturnValidationError()
    {
        // Arrange
        var trainer = UserFactory.GenerateTrainer();
        await AddAsync(trainer);

        var plan = WorkoutPlanFactory.Generate(trainer.Id);
        await AddAsync(plan);

        var longName = new string('A', WorkoutPlan.NameMaxLength + 1);
        var request = new UpdateWorkoutPlanRequest(
            Id: plan.Id.Value,
            Name: longName,
            Description: "Description");
        var client = GetAnonymousClient();

        // Act
        var result = await client.PUTAsync<UpdateWorkoutPlanEndpoint, UpdateWorkoutPlanRequest>(request);

        // Assert
        await Assert.That(result.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task UpdateWorkoutPlan_WithDescriptionTooLong_ShouldReturnValidationError()
    {
        // Arrange
        var trainer = UserFactory.GenerateTrainer();
        await AddAsync(trainer);

        var plan = WorkoutPlanFactory.Generate(trainer.Id);
        await AddAsync(plan);

        var longDescription = new string('A', WorkoutPlan.DescriptionMaxLength + 1);
        var request = new UpdateWorkoutPlanRequest(
            Id: plan.Id.Value,
            Name: "Valid Name",
            Description: longDescription);
        var client = GetAnonymousClient();

        // Act
        var result = await client.PUTAsync<UpdateWorkoutPlanEndpoint, UpdateWorkoutPlanRequest>(request);

        // Assert
        await Assert.That(result.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task UpdateWorkoutPlan_WithMaxLengthValues_ShouldSucceed()
    {
        // Arrange
        var trainer = UserFactory.GenerateTrainer();
        await AddAsync(trainer);

        var plan = WorkoutPlanFactory.Generate(trainer.Id);
        await AddAsync(plan);

        var maxLengthName = new string('A', WorkoutPlan.NameMaxLength);
        var maxLengthDescription = new string('B', WorkoutPlan.DescriptionMaxLength);
        var request = new UpdateWorkoutPlanRequest(
            Id: plan.Id.Value,
            Name: maxLengthName,
            Description: maxLengthDescription);
        var client = GetAnonymousClient();

        // Act
        var result = await client.PUTAsync<UpdateWorkoutPlanEndpoint, UpdateWorkoutPlanRequest>(request);

        // Assert
        await Assert.That(result.StatusCode).IsEqualTo(HttpStatusCode.NoContent);

        var updatedPlan = await GetQueryable<WorkoutPlan>().FirstOrDefaultAsync(wp => wp.Id == plan.Id, CancellationToken);
        await Assert.That(updatedPlan).IsNotNull();
        await Assert.That(updatedPlan!.Name).IsEqualTo(maxLengthName);
        await Assert.That(updatedPlan.Description).IsEqualTo(maxLengthDescription);
    }
}
