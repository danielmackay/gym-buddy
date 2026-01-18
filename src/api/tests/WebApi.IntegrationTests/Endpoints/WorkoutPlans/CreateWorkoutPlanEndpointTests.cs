using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using GymBuddy.Domain.WorkoutPlans;
using GymBuddy.Domain.Users;
using GymBuddy.Api.Features.WorkoutPlans.Endpoints;
using GymBuddy.Api.IntegrationTests.Common;
using GymBuddy.Api.IntegrationTests.Common.Factories;
using System.Net;

namespace GymBuddy.Api.IntegrationTests.Endpoints.WorkoutPlans;

public class CreateWorkoutPlanEndpointTests : IntegrationTestBase
{
    [Test]
    public async Task CreateWorkoutPlan_WithValidRequest_ShouldCreateWorkoutPlan()
    {
        // Arrange
        var trainer = UserFactory.GenerateTrainer();
        await AddAsync(trainer);

        var request = new CreateWorkoutPlanRequest(
            Name: "Full Body Workout",
            Description: "A comprehensive full body workout");
        var client = GetAnonymousClient();

        // Act
        var result = await client.POSTAsync<CreateWorkoutPlanEndpoint, CreateWorkoutPlanRequest, CreateWorkoutPlanResponse>(request);

        // Assert
        await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.Created);
        await Assert.That(result.Result).IsNotNull();
        await Assert.That(result.Result!.Id).IsNotEqualTo(Guid.Empty);

        // Verify the workout plan was created in the database
        var workoutPlanId = WorkoutPlanId.From(result.Result.Id);
        var workoutPlan = await GetQueryable<WorkoutPlan>().FirstOrDefaultAsync(wp => wp.Id == workoutPlanId, CancellationToken);

        await Assert.That(workoutPlan).IsNotNull();
        await Assert.That(workoutPlan!.Name).IsEqualTo(request.Name);
        await Assert.That(workoutPlan.Description).IsEqualTo(request.Description);
        await Assert.That(workoutPlan.TrainerId).IsEqualTo(trainer.Id);
    }

    [Test]
    public async Task CreateWorkoutPlan_WithNoDescription_ShouldSucceed()
    {
        // Arrange
        var trainer = UserFactory.GenerateTrainer();
        await AddAsync(trainer);

        var request = new CreateWorkoutPlanRequest(Name: "Simple Workout");
        var client = GetAnonymousClient();

        // Act
        var result = await client.POSTAsync<CreateWorkoutPlanEndpoint, CreateWorkoutPlanRequest, CreateWorkoutPlanResponse>(request);

        // Assert
        await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.Created);
        await Assert.That(result.Result).IsNotNull();

        // Verify the workout plan was created without a description
        var workoutPlanId = WorkoutPlanId.From(result.Result!.Id);
        var workoutPlan = await GetQueryable<WorkoutPlan>().FirstOrDefaultAsync(wp => wp.Id == workoutPlanId, CancellationToken);

        await Assert.That(workoutPlan).IsNotNull();
        await Assert.That(workoutPlan!.Description).IsNull();
    }

    [Test]
    public async Task CreateWorkoutPlan_WithNoTrainerInDatabase_ShouldReturnError()
    {
        // Arrange
        var request = new CreateWorkoutPlanRequest(
            Name: "Full Body Workout",
            Description: "A comprehensive full body workout");
        var client = GetAnonymousClient();

        // Act
        var result = await client.POSTAsync<CreateWorkoutPlanEndpoint, CreateWorkoutPlanRequest, CreateWorkoutPlanResponse>(request);

        // Assert
        await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);

        // Verify no workout plan was created
        var workoutPlanCount = await GetQueryable<WorkoutPlan>().CountAsync(CancellationToken);
        await Assert.That(workoutPlanCount).IsEqualTo(0);
    }

    [Test]
    public async Task CreateWorkoutPlan_WithEmptyName_ShouldReturnValidationError()
    {
        // Arrange
        var trainer = UserFactory.GenerateTrainer();
        await AddAsync(trainer);

        var request = new CreateWorkoutPlanRequest(
            Name: "",
            Description: "Description");
        var client = GetAnonymousClient();

        // Act
        var result = await client.POSTAsync<CreateWorkoutPlanEndpoint, CreateWorkoutPlanRequest, CreateWorkoutPlanResponse>(request);

        // Assert
        await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);

        // Verify no workout plan was created
        var workoutPlanCount = await GetQueryable<WorkoutPlan>().CountAsync(CancellationToken);
        await Assert.That(workoutPlanCount).IsEqualTo(0);
    }

    [Test]
    public async Task CreateWorkoutPlan_WithNameTooLong_ShouldReturnValidationError()
    {
        // Arrange
        var trainer = UserFactory.GenerateTrainer();
        await AddAsync(trainer);

        var longName = new string('A', WorkoutPlan.NameMaxLength + 1);
        var request = new CreateWorkoutPlanRequest(
            Name: longName,
            Description: "Description");
        var client = GetAnonymousClient();

        // Act
        var result = await client.POSTAsync<CreateWorkoutPlanEndpoint, CreateWorkoutPlanRequest, CreateWorkoutPlanResponse>(request);

        // Assert
        await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);

        // Verify no workout plan was created
        var workoutPlanCount = await GetQueryable<WorkoutPlan>().CountAsync(CancellationToken);
        await Assert.That(workoutPlanCount).IsEqualTo(0);
    }

    [Test]
    public async Task CreateWorkoutPlan_WithDescriptionTooLong_ShouldReturnValidationError()
    {
        // Arrange
        var trainer = UserFactory.GenerateTrainer();
        await AddAsync(trainer);

        var longDescription = new string('A', WorkoutPlan.DescriptionMaxLength + 1);
        var request = new CreateWorkoutPlanRequest(
            Name: "Valid Name",
            Description: longDescription);
        var client = GetAnonymousClient();

        // Act
        var result = await client.POSTAsync<CreateWorkoutPlanEndpoint, CreateWorkoutPlanRequest, CreateWorkoutPlanResponse>(request);

        // Assert
        await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);

        // Verify no workout plan was created
        var workoutPlanCount = await GetQueryable<WorkoutPlan>().CountAsync(CancellationToken);
        await Assert.That(workoutPlanCount).IsEqualTo(0);
    }

    [Test]
    public async Task CreateWorkoutPlan_WithMaxLengthValues_ShouldSucceed()
    {
        // Arrange
        var trainer = UserFactory.GenerateTrainer();
        await AddAsync(trainer);

        var maxLengthName = new string('A', WorkoutPlan.NameMaxLength);
        var maxLengthDescription = new string('B', WorkoutPlan.DescriptionMaxLength);
        var request = new CreateWorkoutPlanRequest(
            Name: maxLengthName,
            Description: maxLengthDescription);
        var client = GetAnonymousClient();

        // Act
        var result = await client.POSTAsync<CreateWorkoutPlanEndpoint, CreateWorkoutPlanRequest, CreateWorkoutPlanResponse>(request);

        // Assert
        await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.Created);
        await Assert.That(result.Result).IsNotNull();

        // Verify the workout plan was created
        var workoutPlanId = WorkoutPlanId.From(result.Result!.Id);
        var workoutPlan = await GetQueryable<WorkoutPlan>().FirstOrDefaultAsync(wp => wp.Id == workoutPlanId, CancellationToken);
        await Assert.That(workoutPlan).IsNotNull();
    }
}
