using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using GymBuddy.Domain.WorkoutPlans;
using GymBuddy.Api.Features.WorkoutPlans.Endpoints;
using GymBuddy.Api.IntegrationTests.Common;
using GymBuddy.Api.IntegrationTests.Common.Factories;
using System.Net;

namespace GymBuddy.Api.IntegrationTests.Endpoints.WorkoutPlans;

public class ReorderExercisesEndpointTests : IntegrationTestBase
{
    [Test]
    public async Task ReorderExercises_WithValidOrder_ShouldReorderExercises()
    {
        // Arrange
        var trainer = UserFactory.GenerateTrainer();
        await AddAsync(trainer);

        var exercise1 = ExerciseFactory.GenerateRepsAndWeight();
        var exercise2 = ExerciseFactory.GenerateRepsAndWeight();
        var exercise3 = ExerciseFactory.GenerateRepsAndWeight();
        await AddRangeAsync([exercise1, exercise2, exercise3]);

        var plan = WorkoutPlanFactory.Generate(trainer.Id);
        plan.AddExercise(exercise1, sets: 3);
        plan.AddExercise(exercise2, sets: 3);
        plan.AddExercise(exercise3, sets: 3);
        await AddAsync(plan);

        // Reorder: 3, 1, 2
        var request = new ReorderExercisesRequest(
            WorkoutPlanId: plan.Id.Value,
            ExerciseIds: [exercise3.Id.Value, exercise1.Id.Value, exercise2.Id.Value]);
        var client = GetAnonymousClient();

        // Act
        var result = await client.PUTAsync<ReorderExercisesEndpoint, ReorderExercisesRequest>(request);

        // Assert
        await Assert.That(result.StatusCode).IsEqualTo(HttpStatusCode.NoContent);

        // Verify the exercises were reordered
        var updatedPlan = await GetQueryable<WorkoutPlan>()
            .Include(wp => wp.Exercises)
            .FirstOrDefaultAsync(wp => wp.Id == plan.Id, CancellationToken);

        await Assert.That(updatedPlan).IsNotNull();
        await Assert.That(updatedPlan!.Exercises.Count).IsEqualTo(3);
        
        var orderedExercises = updatedPlan.Exercises.OrderBy(e => e.Order).ToList();
        await Assert.That(orderedExercises[0].ExerciseId).IsEqualTo(exercise3.Id);
        await Assert.That(orderedExercises[0].Order).IsEqualTo(1);
        await Assert.That(orderedExercises[1].ExerciseId).IsEqualTo(exercise1.Id);
        await Assert.That(orderedExercises[1].Order).IsEqualTo(2);
        await Assert.That(orderedExercises[2].ExerciseId).IsEqualTo(exercise2.Id);
        await Assert.That(orderedExercises[2].Order).IsEqualTo(3);
    }

    [Test]
    public async Task ReorderExercises_WithInvalidPlanId_ShouldReturnNotFound()
    {
        // Arrange
        var request = new ReorderExercisesRequest(
            WorkoutPlanId: Guid.NewGuid(),
            ExerciseIds: [Guid.NewGuid(), Guid.NewGuid()]);
        var client = GetAnonymousClient();

        // Act
        var result = await client.PUTAsync<ReorderExercisesEndpoint, ReorderExercisesRequest>(request);

        // Assert
        await Assert.That(result.StatusCode).IsEqualTo(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task ReorderExercises_WithEmptyList_ShouldReturnValidationError()
    {
        // Arrange
        var trainer = UserFactory.GenerateTrainer();
        await AddAsync(trainer);

        var plan = WorkoutPlanFactory.Generate(trainer.Id);
        await AddAsync(plan);

        var request = new ReorderExercisesRequest(
            WorkoutPlanId: plan.Id.Value,
            ExerciseIds: []);
        var client = GetAnonymousClient();

        // Act
        var result = await client.PUTAsync<ReorderExercisesEndpoint, ReorderExercisesRequest>(request);

        // Assert
        await Assert.That(result.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task ReorderExercises_WithMismatchedExerciseIds_ShouldReturnError()
    {
        // Arrange
        var trainer = UserFactory.GenerateTrainer();
        await AddAsync(trainer);

        var exercise1 = ExerciseFactory.GenerateRepsAndWeight();
        var exercise2 = ExerciseFactory.GenerateRepsAndWeight();
        await AddRangeAsync([exercise1, exercise2]);

        var plan = WorkoutPlanFactory.Generate(trainer.Id);
        plan.AddExercise(exercise1, sets: 3);
        plan.AddExercise(exercise2, sets: 3);
        await AddAsync(plan);

        // Try to reorder with a different exercise ID
        var request = new ReorderExercisesRequest(
            WorkoutPlanId: plan.Id.Value,
            ExerciseIds: [exercise1.Id.Value, Guid.NewGuid()]);
        var client = GetAnonymousClient();

        // Act
        var result = await client.PUTAsync<ReorderExercisesEndpoint, ReorderExercisesRequest>(request);

        // Assert
        await Assert.That(result.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task ReorderExercises_WithDuplicateIds_ShouldReturnError()
    {
        // Arrange
        var trainer = UserFactory.GenerateTrainer();
        await AddAsync(trainer);

        var exercise1 = ExerciseFactory.GenerateRepsAndWeight();
        var exercise2 = ExerciseFactory.GenerateRepsAndWeight();
        await AddRangeAsync([exercise1, exercise2]);

        var plan = WorkoutPlanFactory.Generate(trainer.Id);
        plan.AddExercise(exercise1, sets: 3);
        plan.AddExercise(exercise2, sets: 3);
        await AddAsync(plan);

        // Try to reorder with duplicate IDs
        var request = new ReorderExercisesRequest(
            WorkoutPlanId: plan.Id.Value,
            ExerciseIds: [exercise1.Id.Value, exercise1.Id.Value]);
        var client = GetAnonymousClient();

        // Act
        var result = await client.PUTAsync<ReorderExercisesEndpoint, ReorderExercisesRequest>(request);

        // Assert
        await Assert.That(result.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
    }
}
