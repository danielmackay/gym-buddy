using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using GymBuddy.Domain.WorkoutPlans;
using GymBuddy.Api.Features.WorkoutPlans.Endpoints;
using GymBuddy.Api.IntegrationTests.Common;
using GymBuddy.Api.IntegrationTests.Common.Factories;
using System.Net;

namespace GymBuddy.Api.IntegrationTests.Endpoints.WorkoutPlans;

public class RemoveExerciseFromPlanEndpointTests : IntegrationTestBase
{
    [Test]
    public async Task RemoveExerciseFromPlan_WithValidIds_ShouldRemoveExercise()
    {
        // Arrange
        var trainer = UserFactory.GenerateTrainer();
        await AddAsync(trainer);

        var exercise = ExerciseFactory.GenerateRepsAndWeight();
        await AddAsync(exercise);

        var plan = WorkoutPlanFactory.Generate(trainer.Id);
        plan.AddExercise(exercise, sets: 3, reps: 10);
        await AddAsync(plan);

        var request = new RemoveExerciseFromPlanRequest(
            WorkoutPlanId: plan.Id.Value,
            ExerciseId: exercise.Id.Value);
        var client = GetAnonymousClient();

        // Act
        var result = await client.DELETEAsync<RemoveExerciseFromPlanEndpoint, RemoveExerciseFromPlanRequest>(request);

        // Assert
        await Assert.That(result.StatusCode).IsEqualTo(HttpStatusCode.NoContent);

        // Verify the exercise was removed
        var updatedPlan = await GetQueryable<WorkoutPlan>()
            .Include(wp => wp.Exercises)
            .FirstOrDefaultAsync(wp => wp.Id == plan.Id, CancellationToken);

        await Assert.That(updatedPlan).IsNotNull();
        await Assert.That(updatedPlan!.Exercises.Count).IsEqualTo(0);
    }

    [Test]
    public async Task RemoveExerciseFromPlan_WithMultipleExercises_ShouldReorderRemaining()
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

        var request = new RemoveExerciseFromPlanRequest(
            WorkoutPlanId: plan.Id.Value,
            ExerciseId: exercise2.Id.Value);
        var client = GetAnonymousClient();

        // Act
        var result = await client.DELETEAsync<RemoveExerciseFromPlanEndpoint, RemoveExerciseFromPlanRequest>(request);

        // Assert
        await Assert.That(result.StatusCode).IsEqualTo(HttpStatusCode.NoContent);

        // Verify the exercise was removed and others were reordered
        var updatedPlan = await GetQueryable<WorkoutPlan>()
            .Include(wp => wp.Exercises)
            .FirstOrDefaultAsync(wp => wp.Id == plan.Id, CancellationToken);

        await Assert.That(updatedPlan).IsNotNull();
        await Assert.That(updatedPlan!.Exercises.Count).IsEqualTo(2);
        
        var orderedExercises = updatedPlan.Exercises.OrderBy(e => e.Order).ToList();
        await Assert.That(orderedExercises[0].ExerciseId).IsEqualTo(exercise1.Id);
        await Assert.That(orderedExercises[0].Order).IsEqualTo(1);
        await Assert.That(orderedExercises[1].ExerciseId).IsEqualTo(exercise3.Id);
        await Assert.That(orderedExercises[1].Order).IsEqualTo(2);
    }

    [Test]
    public async Task RemoveExerciseFromPlan_WithInvalidPlanId_ShouldReturnNotFound()
    {
        // Arrange
        var exercise = ExerciseFactory.GenerateRepsAndWeight();
        await AddAsync(exercise);

        var request = new RemoveExerciseFromPlanRequest(
            WorkoutPlanId: Guid.NewGuid(),
            ExerciseId: exercise.Id.Value);
        var client = GetAnonymousClient();

        // Act
        var result = await client.DELETEAsync<RemoveExerciseFromPlanEndpoint, RemoveExerciseFromPlanRequest>(request);

        // Assert
        await Assert.That(result.StatusCode).IsEqualTo(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task RemoveExerciseFromPlan_WithNonExistentExercise_ShouldReturnError()
    {
        // Arrange
        var trainer = UserFactory.GenerateTrainer();
        await AddAsync(trainer);

        var plan = WorkoutPlanFactory.Generate(trainer.Id);
        await AddAsync(plan);

        var request = new RemoveExerciseFromPlanRequest(
            WorkoutPlanId: plan.Id.Value,
            ExerciseId: Guid.NewGuid());
        var client = GetAnonymousClient();

        // Act
        var result = await client.DELETEAsync<RemoveExerciseFromPlanEndpoint, RemoveExerciseFromPlanRequest>(request);

        // Assert
        await Assert.That(result.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
    }
}
