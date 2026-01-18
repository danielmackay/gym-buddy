using FastEndpoints;
using GymBuddy.Api.Features.WorkoutPlans.Endpoints;
using GymBuddy.Api.IntegrationTests.Common;
using GymBuddy.Api.IntegrationTests.Common.Factories;
using System.Net;

namespace GymBuddy.Api.IntegrationTests.Endpoints.WorkoutPlans;

public class ListWorkoutPlansEndpointTests : IntegrationTestBase
{
    [Test]
    public async Task ListWorkoutPlans_WithNoPlans_ShouldReturnEmptyList()
    {
        // Arrange
        var client = GetAnonymousClient();

        // Act
        var result = await client.GETAsync<ListWorkoutPlansEndpoint, ListWorkoutPlansRequest, List<WorkoutPlanResponse>>(
            new ListWorkoutPlansRequest(TrainerId: null));

        // Assert
        await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        await Assert.That(result.Result).IsNotNull();
        await Assert.That(result.Result!.Count).IsEqualTo(0);
    }

    [Test]
    public async Task ListWorkoutPlans_WithMultiplePlans_ShouldReturnAllPlans()
    {
        // Arrange
        var trainer = UserFactory.GenerateTrainer();
        await AddAsync(trainer);

        var plan1 = WorkoutPlanFactory.Generate(trainer.Id);
        var plan2 = WorkoutPlanFactory.Generate(trainer.Id);
        var plan3 = WorkoutPlanFactory.Generate(trainer.Id);
        await AddRangeAsync([plan1, plan2, plan3]);

        var client = GetAnonymousClient();

        // Act
        var result = await client.GETAsync<ListWorkoutPlansEndpoint, ListWorkoutPlansRequest, List<WorkoutPlanResponse>>(
            new ListWorkoutPlansRequest(TrainerId: null));

        // Assert
        await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        await Assert.That(result.Result).IsNotNull();
        await Assert.That(result.Result!.Count).IsEqualTo(3);
    }

    [Test]
    public async Task ListWorkoutPlans_FilteredByTrainer_ShouldReturnOnlyTrainerPlans()
    {
        // Arrange
        var trainer1 = UserFactory.GenerateTrainer();
        var trainer2 = UserFactory.GenerateTrainer();
        await AddRangeAsync([trainer1, trainer2]);

        var plan1 = WorkoutPlanFactory.Generate(trainer1.Id);
        var plan2 = WorkoutPlanFactory.Generate(trainer1.Id);
        var plan3 = WorkoutPlanFactory.Generate(trainer2.Id);
        await AddRangeAsync([plan1, plan2, plan3]);

        var client = GetAnonymousClient();

        // Act
        var result = await client.GETAsync<ListWorkoutPlansEndpoint, ListWorkoutPlansRequest, List<WorkoutPlanResponse>>(
            new ListWorkoutPlansRequest(TrainerId: trainer1.Id.Value));

        // Assert
        await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        await Assert.That(result.Result).IsNotNull();
        await Assert.That(result.Result!.Count).IsEqualTo(2);
        await Assert.That(result.Result.All(p => p.TrainerId == trainer1.Id.Value)).IsTrue();
    }

    [Test]
    public async Task ListWorkoutPlans_WithExercises_ShouldIncludeExercises()
    {
        // Arrange
        var trainer = UserFactory.GenerateTrainer();
        await AddAsync(trainer);

        var exercise1 = ExerciseFactory.GenerateRepsAndWeight();
        var exercise2 = ExerciseFactory.GenerateTimeBased();
        await AddRangeAsync([exercise1, exercise2]);

        var plan = WorkoutPlanFactory.Generate(trainer.Id);
        plan.AddExercise(exercise1, sets: 3, reps: 10);
        plan.AddExercise(exercise2, sets: 3, duration: new Domain.Common.Duration(60));
        await AddAsync(plan);

        var client = GetAnonymousClient();

        // Act
        var result = await client.GETAsync<ListWorkoutPlansEndpoint, ListWorkoutPlansRequest, List<WorkoutPlanResponse>>(
            new ListWorkoutPlansRequest(TrainerId: null));

        // Assert
        await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        await Assert.That(result.Result).IsNotNull();
        await Assert.That(result.Result!.Count).IsEqualTo(1);
        
        var returnedPlan = result.Result.First();
        await Assert.That(returnedPlan.Exercises.Count).IsEqualTo(2);
        await Assert.That(returnedPlan.Exercises[0].ExerciseName).IsEqualTo(exercise1.Name);
        await Assert.That(returnedPlan.Exercises[0].Sets).IsEqualTo(3);
        await Assert.That(returnedPlan.Exercises[0].Reps).IsEqualTo(10);
        await Assert.That(returnedPlan.Exercises[1].ExerciseName).IsEqualTo(exercise2.Name);
        await Assert.That(returnedPlan.Exercises[1].Duration).IsNotNull();
        await Assert.That(returnedPlan.Exercises[1].Duration!.Seconds).IsEqualTo(60);
    }
}
