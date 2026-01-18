using FastEndpoints;
using GymBuddy.Api.Features.WorkoutPlans.Endpoints;
using GymBuddy.Api.IntegrationTests.Common;
using GymBuddy.Api.IntegrationTests.Common.Factories;
using System.Net;

namespace GymBuddy.Api.IntegrationTests.Endpoints.WorkoutPlans;

public class GetWorkoutPlanEndpointTests : IntegrationTestBase
{
    [Test]
    public async Task GetWorkoutPlan_WithValidId_ShouldReturnWorkoutPlan()
    {
        // Arrange
        var trainer = UserFactory.GenerateTrainer();
        await AddAsync(trainer);

        var plan = WorkoutPlanFactory.Generate(trainer.Id);
        await AddAsync(plan);

        var client = GetAnonymousClient();

        // Act
        var result = await client.GETAsync<GetWorkoutPlanEndpoint, GetWorkoutPlanRequest, WorkoutPlanResponse>(
            new GetWorkoutPlanRequest(plan.Id.Value));

        // Assert
        await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        await Assert.That(result.Result).IsNotNull();
        await Assert.That(result.Result!.Id).IsEqualTo(plan.Id.Value);
        await Assert.That(result.Result.Name).IsEqualTo(plan.Name);
        await Assert.That(result.Result.Description).IsEqualTo(plan.Description);
        await Assert.That(result.Result.TrainerId).IsEqualTo(trainer.Id.Value);
    }

    [Test]
    public async Task GetWorkoutPlan_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var client = GetAnonymousClient();

        // Act
        var result = await client.GETAsync<GetWorkoutPlanEndpoint, GetWorkoutPlanRequest, WorkoutPlanResponse>(
            new GetWorkoutPlanRequest(nonExistentId));

        // Assert
        await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task GetWorkoutPlan_WithExercises_ShouldIncludeExercises()
    {
        // Arrange
        var trainer = UserFactory.GenerateTrainer();
        await AddAsync(trainer);

        var exercise1 = ExerciseFactory.GenerateRepsAndWeight();
        var exercise2 = ExerciseFactory.GenerateTimeBased();
        await AddRangeAsync([exercise1, exercise2]);

        var plan = WorkoutPlanFactory.Generate(trainer.Id);
        plan.AddExercise(exercise1, sets: 3, reps: 10, weight: new Domain.Common.Weight(50));
        plan.AddExercise(exercise2, sets: 3, duration: new Domain.Common.Duration(60));
        await AddAsync(plan);

        var client = GetAnonymousClient();

        // Act
        var result = await client.GETAsync<GetWorkoutPlanEndpoint, GetWorkoutPlanRequest, WorkoutPlanResponse>(
            new GetWorkoutPlanRequest(plan.Id.Value));

        // Assert
        await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        await Assert.That(result.Result).IsNotNull();
        await Assert.That(result.Result!.Exercises.Count).IsEqualTo(2);
        
        var ex1 = result.Result.Exercises[0];
        await Assert.That(ex1.ExerciseId).IsEqualTo(exercise1.Id.Value);
        await Assert.That(ex1.ExerciseName).IsEqualTo(exercise1.Name);
        await Assert.That(ex1.Sets).IsEqualTo(3);
        await Assert.That(ex1.Reps).IsEqualTo(10);
        await Assert.That(ex1.Weight).IsNotNull();
        await Assert.That(ex1.Weight!.Value).IsEqualTo(50);

        var ex2 = result.Result.Exercises[1];
        await Assert.That(ex2.ExerciseId).IsEqualTo(exercise2.Id.Value);
        await Assert.That(ex2.Duration).IsNotNull();
        await Assert.That(ex2.Duration!.Seconds).IsEqualTo(60);
    }

    [Test]
    public async Task GetWorkoutPlan_ExercisesOrderedCorrectly_ShouldReturnInOrder()
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

        var client = GetAnonymousClient();

        // Act
        var result = await client.GETAsync<GetWorkoutPlanEndpoint, GetWorkoutPlanRequest, WorkoutPlanResponse>(
            new GetWorkoutPlanRequest(plan.Id.Value));

        // Assert
        await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        await Assert.That(result.Result).IsNotNull();
        await Assert.That(result.Result!.Exercises.Count).IsEqualTo(3);
        await Assert.That(result.Result.Exercises[0].Order).IsEqualTo(1);
        await Assert.That(result.Result.Exercises[1].Order).IsEqualTo(2);
        await Assert.That(result.Result.Exercises[2].Order).IsEqualTo(3);
    }
}
