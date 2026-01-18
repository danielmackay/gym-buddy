using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using GymBuddy.Domain.WorkoutPlans;
using GymBuddy.Api.Features.WorkoutPlans.Endpoints;
using GymBuddy.Api.IntegrationTests.Common;
using GymBuddy.Api.IntegrationTests.Common.Factories;
using GymBuddy.Domain.Common;
using System.Net;

namespace GymBuddy.Api.IntegrationTests.Endpoints.WorkoutPlans;

public class AddExerciseToPlanEndpointTests : IntegrationTestBase
{
    [Test]
    public async Task AddExerciseToPlan_WithRepsAndWeight_ShouldAddExercise()
    {
        // Arrange
        var trainer = UserFactory.GenerateTrainer();
        await AddAsync(trainer);

        var exercise = ExerciseFactory.GenerateRepsAndWeight();
        await AddAsync(exercise);

        var plan = WorkoutPlanFactory.Generate(trainer.Id);
        await AddAsync(plan);

        var request = new AddExerciseToPlanRequest(
            WorkoutPlanId: plan.Id.Value,
            ExerciseId: exercise.Id.Value,
            Sets: 3,
            Reps: 10,
            Weight: new WeightRequest(20, WeightUnit.Kilograms),
            Duration: null);
        var client = GetAnonymousClient();

        // Act
        var result = await client.POSTAsync<AddExerciseToPlanEndpoint, AddExerciseToPlanRequest>(request);

        // Assert
        await Assert.That(result.StatusCode).IsEqualTo(HttpStatusCode.NoContent);

        // Verify the exercise was added
        var updatedPlan = await GetQueryable<WorkoutPlan>()
            .Include(wp => wp.Exercises)
            .FirstOrDefaultAsync(wp => wp.Id == plan.Id, CancellationToken);

        await Assert.That(updatedPlan).IsNotNull();
        await Assert.That(updatedPlan!.Exercises.Count).IsEqualTo(1);
        
        var plannedExercise = updatedPlan.Exercises.First();
        await Assert.That(plannedExercise.ExerciseId).IsEqualTo(exercise.Id);
        await Assert.That(plannedExercise.Sets).IsEqualTo(3);
        await Assert.That(plannedExercise.Reps).IsEqualTo(10);
        await Assert.That(plannedExercise.Weight).IsNotNull();
        await Assert.That(plannedExercise.Weight!.Value).IsEqualTo(20);
        await Assert.That(plannedExercise.Weight.Unit).IsEqualTo(WeightUnit.Kilograms);
    }

    [Test]
    public async Task AddExerciseToPlan_WithTimeBased_ShouldAddExercise()
    {
        // Arrange
        var trainer = UserFactory.GenerateTrainer();
        await AddAsync(trainer);

        var exercise = ExerciseFactory.GenerateTimeBased();
        await AddAsync(exercise);

        var plan = WorkoutPlanFactory.Generate(trainer.Id);
        await AddAsync(plan);

        var request = new AddExerciseToPlanRequest(
            WorkoutPlanId: plan.Id.Value,
            ExerciseId: exercise.Id.Value,
            Sets: 3,
            Reps: null,
            Weight: null,
            Duration: new DurationRequest(60));
        var client = GetAnonymousClient();

        // Act
        var result = await client.POSTAsync<AddExerciseToPlanEndpoint, AddExerciseToPlanRequest>(request);

        // Assert
        await Assert.That(result.StatusCode).IsEqualTo(HttpStatusCode.NoContent);

        // Verify the exercise was added
        var updatedPlan = await GetQueryable<WorkoutPlan>()
            .Include(wp => wp.Exercises)
            .FirstOrDefaultAsync(wp => wp.Id == plan.Id, CancellationToken);

        await Assert.That(updatedPlan).IsNotNull();
        await Assert.That(updatedPlan!.Exercises.Count).IsEqualTo(1);
        
        var plannedExercise = updatedPlan.Exercises.First();
        await Assert.That(plannedExercise.Duration).IsNotNull();
        await Assert.That(plannedExercise.Duration!.Seconds).IsEqualTo(60);
    }

    [Test]
    public async Task AddExerciseToPlan_MultipleExercises_ShouldMaintainOrder()
    {
        // Arrange
        var trainer = UserFactory.GenerateTrainer();
        await AddAsync(trainer);

        var exercise1 = ExerciseFactory.GenerateRepsAndWeight();
        var exercise2 = ExerciseFactory.GenerateRepsAndWeight();
        await AddRangeAsync([exercise1, exercise2]);

        var plan = WorkoutPlanFactory.Generate(trainer.Id);
        await AddAsync(plan);

        var client = GetAnonymousClient();

        // Act - Add first exercise
        var request1 = new AddExerciseToPlanRequest(
            WorkoutPlanId: plan.Id.Value,
            ExerciseId: exercise1.Id.Value,
            Sets: 3,
            Reps: 10);
        await client.POSTAsync<AddExerciseToPlanEndpoint, AddExerciseToPlanRequest>(request1);

        // Add second exercise
        var request2 = new AddExerciseToPlanRequest(
            WorkoutPlanId: plan.Id.Value,
            ExerciseId: exercise2.Id.Value,
            Sets: 4,
            Reps: 12);
        await client.POSTAsync<AddExerciseToPlanEndpoint, AddExerciseToPlanRequest>(request2);

        // Assert
        var updatedPlan = await GetQueryable<WorkoutPlan>()
            .Include(wp => wp.Exercises)
            .FirstOrDefaultAsync(wp => wp.Id == plan.Id, CancellationToken);

        await Assert.That(updatedPlan).IsNotNull();
        await Assert.That(updatedPlan!.Exercises.Count).IsEqualTo(2);
        
        var orderedExercises = updatedPlan.Exercises.OrderBy(e => e.Order).ToList();
        await Assert.That(orderedExercises[0].Order).IsEqualTo(1);
        await Assert.That(orderedExercises[1].Order).IsEqualTo(2);
    }

    [Test]
    public async Task AddExerciseToPlan_WithInvalidPlanId_ShouldReturnNotFound()
    {
        // Arrange
        var exercise = ExerciseFactory.GenerateRepsAndWeight();
        await AddAsync(exercise);

        var request = new AddExerciseToPlanRequest(
            WorkoutPlanId: Guid.NewGuid(),
            ExerciseId: exercise.Id.Value,
            Sets: 3,
            Reps: 10);
        var client = GetAnonymousClient();

        // Act
        var result = await client.POSTAsync<AddExerciseToPlanEndpoint, AddExerciseToPlanRequest>(request);

        // Assert
        await Assert.That(result.StatusCode).IsEqualTo(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task AddExerciseToPlan_WithInvalidExerciseId_ShouldReturnNotFound()
    {
        // Arrange
        var trainer = UserFactory.GenerateTrainer();
        await AddAsync(trainer);

        var plan = WorkoutPlanFactory.Generate(trainer.Id);
        await AddAsync(plan);

        var request = new AddExerciseToPlanRequest(
            WorkoutPlanId: plan.Id.Value,
            ExerciseId: Guid.NewGuid(),
            Sets: 3,
            Reps: 10);
        var client = GetAnonymousClient();

        // Act
        var result = await client.POSTAsync<AddExerciseToPlanEndpoint, AddExerciseToPlanRequest>(request);

        // Assert
        await Assert.That(result.StatusCode).IsEqualTo(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task AddExerciseToPlan_WithZeroSets_ShouldReturnValidationError()
    {
        // Arrange
        var trainer = UserFactory.GenerateTrainer();
        await AddAsync(trainer);

        var exercise = ExerciseFactory.GenerateRepsAndWeight();
        await AddAsync(exercise);

        var plan = WorkoutPlanFactory.Generate(trainer.Id);
        await AddAsync(plan);

        var request = new AddExerciseToPlanRequest(
            WorkoutPlanId: plan.Id.Value,
            ExerciseId: exercise.Id.Value,
            Sets: 0,
            Reps: 10);
        var client = GetAnonymousClient();

        // Act
        var result = await client.POSTAsync<AddExerciseToPlanEndpoint, AddExerciseToPlanRequest>(request);

        // Assert
        await Assert.That(result.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task AddExerciseToPlan_WithNegativeWeight_ShouldReturnValidationError()
    {
        // Arrange
        var trainer = UserFactory.GenerateTrainer();
        await AddAsync(trainer);

        var exercise = ExerciseFactory.GenerateRepsAndWeight();
        await AddAsync(exercise);

        var plan = WorkoutPlanFactory.Generate(trainer.Id);
        await AddAsync(plan);

        var request = new AddExerciseToPlanRequest(
            WorkoutPlanId: plan.Id.Value,
            ExerciseId: exercise.Id.Value,
            Sets: 3,
            Reps: 10,
            Weight: new WeightRequest(-10, WeightUnit.Kilograms));
        var client = GetAnonymousClient();

        // Act
        var result = await client.POSTAsync<AddExerciseToPlanEndpoint, AddExerciseToPlanRequest>(request);

        // Assert
        await Assert.That(result.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
    }
}
