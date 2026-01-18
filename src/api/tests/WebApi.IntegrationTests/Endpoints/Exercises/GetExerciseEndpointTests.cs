using FastEndpoints;
using GymBuddy.Domain.Exercises;
using GymBuddy.Api.Features.Exercises.Endpoints;
using GymBuddy.Api.IntegrationTests.Common;
using GymBuddy.Api.IntegrationTests.Common.Factories;
using System.Net;

namespace GymBuddy.Api.IntegrationTests.Endpoints.Exercises;

public class GetExerciseEndpointTests : IntegrationTestBase
{
    [Test]
    public async Task GetExercise_WithExistingRepsAndWeightExercise_ShouldReturnExercise()
    {
        // Arrange
        var exercise = Exercise.Create(
            "Bench Press",
            ExerciseType.RepsAndWeight,
            [MuscleGroup.Chest, MuscleGroup.Triceps],
            "A compound chest exercise").Value;
        await AddAsync(exercise);

        var request = new GetExerciseRequest(exercise.Id.Value);
        var client = GetAnonymousClient();

        // Act
        var result = await client.GETAsync<GetExerciseEndpoint, GetExerciseRequest, ExerciseResponse>(request);

        // Assert
        await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        await Assert.That(result.Result).IsNotNull();
        await Assert.That(result.Result!.Id).IsEqualTo(exercise.Id.Value);
        await Assert.That(result.Result.Name).IsEqualTo("Bench Press");
        await Assert.That(result.Result.Description).IsEqualTo("A compound chest exercise");
        await Assert.That(result.Result.Type).IsEqualTo(ExerciseType.RepsAndWeight);
        await Assert.That(result.Result.MuscleGroups.Count).IsEqualTo(2);
        await Assert.That(result.Result.MuscleGroups).Contains(MuscleGroup.Chest);
        await Assert.That(result.Result.MuscleGroups).Contains(MuscleGroup.Triceps);
    }

    [Test]
    public async Task GetExercise_WithExistingTimeBasedExercise_ShouldReturnExercise()
    {
        // Arrange
        var exercise = Exercise.Create(
            "Plank",
            ExerciseType.TimeBased,
            [MuscleGroup.Abs, MuscleGroup.Obliques],
            "Core stability exercise").Value;
        await AddAsync(exercise);

        var request = new GetExerciseRequest(exercise.Id.Value);
        var client = GetAnonymousClient();

        // Act
        var result = await client.GETAsync<GetExerciseEndpoint, GetExerciseRequest, ExerciseResponse>(request);

        // Assert
        await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        await Assert.That(result.Result).IsNotNull();
        await Assert.That(result.Result!.Id).IsEqualTo(exercise.Id.Value);
        await Assert.That(result.Result.Name).IsEqualTo("Plank");
        await Assert.That(result.Result.Type).IsEqualTo(ExerciseType.TimeBased);
        await Assert.That(result.Result.MuscleGroups.Count).IsEqualTo(2);
    }

    [Test]
    public async Task GetExercise_WithExerciseWithNoDescription_ShouldReturnExerciseWithNullDescription()
    {
        // Arrange
        var exercise = Exercise.Create(
            "Squat",
            ExerciseType.RepsAndWeight,
            [MuscleGroup.Quadriceps, MuscleGroup.Glutes],
            null).Value;
        await AddAsync(exercise);

        var request = new GetExerciseRequest(exercise.Id.Value);
        var client = GetAnonymousClient();

        // Act
        var result = await client.GETAsync<GetExerciseEndpoint, GetExerciseRequest, ExerciseResponse>(request);

        // Assert
        await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        await Assert.That(result.Result).IsNotNull();
        await Assert.That(result.Result!.Description).IsNull();
    }

    [Test]
    public async Task GetExercise_WithNonExistentId_ShouldReturnNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var request = new GetExerciseRequest(nonExistentId);
        var client = GetAnonymousClient();

        // Act
        var result = await client.GETAsync<GetExerciseEndpoint, GetExerciseRequest, ExerciseResponse>(request);

        // Assert
        await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task GetExercise_WithEmptyId_ShouldReturnNotFound()
    {
        // Arrange
        var request = new GetExerciseRequest(Guid.Empty);
        var client = GetAnonymousClient();

        // Act
        var result = await client.GETAsync<GetExerciseEndpoint, GetExerciseRequest, ExerciseResponse>(request);

        // Assert
        await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task GetExercise_WithMultipleMuscleGroups_ShouldReturnAllMuscleGroups()
    {
        // Arrange
        var exercise = Exercise.Create(
            "Deadlift",
            ExerciseType.RepsAndWeight,
            [MuscleGroup.Back, MuscleGroup.Hamstrings, MuscleGroup.Glutes, MuscleGroup.Forearms],
            "Full body compound movement").Value;
        await AddAsync(exercise);

        var request = new GetExerciseRequest(exercise.Id.Value);
        var client = GetAnonymousClient();

        // Act
        var result = await client.GETAsync<GetExerciseEndpoint, GetExerciseRequest, ExerciseResponse>(request);

        // Assert
        await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        await Assert.That(result.Result).IsNotNull();
        await Assert.That(result.Result!.MuscleGroups.Count).IsEqualTo(4);
        await Assert.That(result.Result.MuscleGroups).Contains(MuscleGroup.Back);
        await Assert.That(result.Result.MuscleGroups).Contains(MuscleGroup.Hamstrings);
        await Assert.That(result.Result.MuscleGroups).Contains(MuscleGroup.Glutes);
        await Assert.That(result.Result.MuscleGroups).Contains(MuscleGroup.Forearms);
    }
}
