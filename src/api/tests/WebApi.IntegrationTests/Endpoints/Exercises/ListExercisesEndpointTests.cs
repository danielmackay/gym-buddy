using FastEndpoints;
using GymBuddy.Domain.Exercises;
using GymBuddy.Api.Features.Exercises.Endpoints;
using GymBuddy.Api.IntegrationTests.Common;
using GymBuddy.Api.IntegrationTests.Common.Factories;
using System.Net;

namespace GymBuddy.Api.IntegrationTests.Endpoints.Exercises;

public class ListExercisesEndpointTests : IntegrationTestBase
{
    [Test]
    public async Task ListExercises_WithNoFilters_ShouldReturnAllExercises()
    {
        // Arrange
        var exercises = ExerciseFactory.Generate(5);
        await AddRangeAsync(exercises);

        var request = new ListExercisesRequest();
        var client = GetAnonymousClient();

        // Act
        var result = await client.GETAsync<ListExercisesEndpoint, ListExercisesRequest, List<ExerciseResponse>>(request);

        // Assert
        await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        await Assert.That(result.Result).IsNotNull();
        await Assert.That(result.Result!.Count).IsEqualTo(5);
    }

    [Test]
    public async Task ListExercises_FilterByMuscleGroup_ShouldReturnMatchingExercises()
    {
        // Arrange
        var chestExercise = Exercise.Create(
            "Bench Press",
            ExerciseType.RepsAndWeight,
            [MuscleGroup.Chest, MuscleGroup.Triceps],
            "Chest exercise").Value;

        var backExercise = Exercise.Create(
            "Pull Up",
            ExerciseType.RepsAndWeight,
            [MuscleGroup.Back, MuscleGroup.Biceps],
            "Back exercise").Value;

        var legExercise = Exercise.Create(
            "Squat",
            ExerciseType.RepsAndWeight,
            [MuscleGroup.Quadriceps, MuscleGroup.Glutes],
            "Leg exercise").Value;

        await AddRangeAsync(new[] { chestExercise, backExercise, legExercise });

        var request = new ListExercisesRequest(MuscleGroup: MuscleGroup.Chest);
        var client = GetAnonymousClient();

        // Act
        var result = await client.GETAsync<ListExercisesEndpoint, ListExercisesRequest, List<ExerciseResponse>>(request);

        // Assert
        await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        await Assert.That(result.Result).IsNotNull();
        await Assert.That(result.Result!.Count).IsEqualTo(1);
        await Assert.That(result.Result[0].Name).IsEqualTo("Bench Press");
        await Assert.That(result.Result[0].MuscleGroups).Contains(MuscleGroup.Chest);
    }

    [Test]
    public async Task ListExercises_FilterByType_ShouldReturnMatchingExercises()
    {
        // Arrange
        var repsExercises = ExerciseFactory.GenerateRepsAndWeight(3);
        var timeExercises = ExerciseFactory.GenerateTimeBased(2);
        await AddRangeAsync(repsExercises.Concat(timeExercises).ToList());

        var request = new ListExercisesRequest(Type: ExerciseType.TimeBased);
        var client = GetAnonymousClient();

        // Act
        var result = await client.GETAsync<ListExercisesEndpoint, ListExercisesRequest, List<ExerciseResponse>>(request);

        // Assert
        await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        await Assert.That(result.Result).IsNotNull();
        await Assert.That(result.Result!.Count).IsEqualTo(2);
        await Assert.That(result.Result.All(e => e.Type == ExerciseType.TimeBased)).IsTrue();
    }

    [Test]
    public async Task ListExercises_FilterByMuscleGroupAndType_ShouldReturnMatchingExercises()
    {
        // Arrange
        var chestRepsExercise = Exercise.Create(
            "Bench Press",
            ExerciseType.RepsAndWeight,
            [MuscleGroup.Chest],
            null).Value;

        var chestTimeExercise = Exercise.Create(
            "Chest Plank",
            ExerciseType.TimeBased,
            [MuscleGroup.Chest, MuscleGroup.Abs],
            null).Value;

        var backRepsExercise = Exercise.Create(
            "Deadlift",
            ExerciseType.RepsAndWeight,
            [MuscleGroup.Back],
            null).Value;

        await AddRangeAsync(new[] { chestRepsExercise, chestTimeExercise, backRepsExercise });

        var request = new ListExercisesRequest(
            MuscleGroup: MuscleGroup.Chest,
            Type: ExerciseType.RepsAndWeight);
        var client = GetAnonymousClient();

        // Act
        var result = await client.GETAsync<ListExercisesEndpoint, ListExercisesRequest, List<ExerciseResponse>>(request);

        // Assert
        await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        await Assert.That(result.Result).IsNotNull();
        await Assert.That(result.Result!.Count).IsEqualTo(1);
        await Assert.That(result.Result[0].Name).IsEqualTo("Bench Press");
        await Assert.That(result.Result[0].Type).IsEqualTo(ExerciseType.RepsAndWeight);
        await Assert.That(result.Result[0].MuscleGroups).Contains(MuscleGroup.Chest);
    }

    [Test]
    public async Task ListExercises_WithNoMatchingFilters_ShouldReturnEmptyList()
    {
        // Arrange
        var chestExercises = Exercise.Create(
            "Bench Press",
            ExerciseType.RepsAndWeight,
            [MuscleGroup.Chest],
            null).Value;
        await AddAsync(chestExercises);

        var request = new ListExercisesRequest(MuscleGroup: MuscleGroup.Biceps);
        var client = GetAnonymousClient();

        // Act
        var result = await client.GETAsync<ListExercisesEndpoint, ListExercisesRequest, List<ExerciseResponse>>(request);

        // Assert
        await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        await Assert.That(result.Result).IsNotNull();
        await Assert.That(result.Result!.Count).IsEqualTo(0);
    }

    [Test]
    public async Task ListExercises_WithEmptyDatabase_ShouldReturnEmptyList()
    {
        // Arrange
        var request = new ListExercisesRequest();
        var client = GetAnonymousClient();

        // Act
        var result = await client.GETAsync<ListExercisesEndpoint, ListExercisesRequest, List<ExerciseResponse>>(request);

        // Assert
        await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        await Assert.That(result.Result).IsNotNull();
        await Assert.That(result.Result!.Count).IsEqualTo(0);
    }

    [Test]
    public async Task ListExercises_WithMultipleMuscleGroups_ShouldReturnExercisesWithAllDetails()
    {
        // Arrange
        var exercise = Exercise.Create(
            "Deadlift",
            ExerciseType.RepsAndWeight,
            [MuscleGroup.Back, MuscleGroup.Hamstrings, MuscleGroup.Glutes],
            "A compound exercise").Value;
        await AddAsync(exercise);

        var request = new ListExercisesRequest();
        var client = GetAnonymousClient();

        // Act
        var result = await client.GETAsync<ListExercisesEndpoint, ListExercisesRequest, List<ExerciseResponse>>(request);

        // Assert
        await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        await Assert.That(result.Result).IsNotNull();
        await Assert.That(result.Result!.Count).IsEqualTo(1);
        await Assert.That(result.Result[0].Name).IsEqualTo("Deadlift");
        await Assert.That(result.Result[0].Description).IsEqualTo("A compound exercise");
        await Assert.That(result.Result[0].Type).IsEqualTo(ExerciseType.RepsAndWeight);
        await Assert.That(result.Result[0].MuscleGroups.Count).IsEqualTo(3);
        await Assert.That(result.Result[0].MuscleGroups).Contains(MuscleGroup.Back);
        await Assert.That(result.Result[0].MuscleGroups).Contains(MuscleGroup.Hamstrings);
        await Assert.That(result.Result[0].MuscleGroups).Contains(MuscleGroup.Glutes);
    }
}
