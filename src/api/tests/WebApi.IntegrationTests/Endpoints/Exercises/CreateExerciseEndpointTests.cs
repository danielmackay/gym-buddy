using FastEndpoints;
using GymBuddy.Domain.Exercises;
using GymBuddy.Api.Features.Exercises.Endpoints;
using GymBuddy.Api.IntegrationTests.Common;
using System.Net;

namespace GymBuddy.Api.IntegrationTests.Endpoints.Exercises;

public class CreateExerciseEndpointTests : IntegrationTestBase
{
    [Test]
    public async Task CreateExercise_WithValidRepsAndWeight_ShouldReturnCreatedExercise()
    {
        // Arrange
        var request = new CreateExerciseRequest(
            "Bench Press",
            ExerciseType.RepsAndWeight,
            [MuscleGroup.Chest, MuscleGroup.Triceps],
            "A compound chest exercise");
        var client = GetAnonymousClient();

        // Act
        var result = await client.POSTAsync<CreateExerciseEndpoint, CreateExerciseRequest, CreateExerciseResponse>(request);

        // Assert
        await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.Created);
        await Assert.That(result.Result).IsNotNull();
        await Assert.That(result.Result!.Id).IsNotEqualTo(Guid.Empty);
    }

    [Test]
    public async Task CreateExercise_WithValidTimeBased_ShouldReturnCreatedExercise()
    {
        // Arrange
        var request = new CreateExerciseRequest(
            "Plank",
            ExerciseType.TimeBased,
            [MuscleGroup.Abs, MuscleGroup.Obliques],
            "An isometric core exercise");
        var client = GetAnonymousClient();

        // Act
        var result = await client.POSTAsync<CreateExerciseEndpoint, CreateExerciseRequest, CreateExerciseResponse>(request);

        // Assert
        await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.Created);
        await Assert.That(result.Result).IsNotNull();
        await Assert.That(result.Result!.Id).IsNotEqualTo(Guid.Empty);
    }

    [Test]
    public async Task CreateExercise_WithNoDescription_ShouldReturnCreatedExercise()
    {
        // Arrange
        var request = new CreateExerciseRequest(
            "Deadlift",
            ExerciseType.RepsAndWeight,
            [MuscleGroup.Back, MuscleGroup.Hamstrings, MuscleGroup.Glutes],
            null);
        var client = GetAnonymousClient();

        // Act
        var result = await client.POSTAsync<CreateExerciseEndpoint, CreateExerciseRequest, CreateExerciseResponse>(request);

        // Assert
        await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.Created);
        await Assert.That(result.Result).IsNotNull();
        await Assert.That(result.Result!.Id).IsNotEqualTo(Guid.Empty);
    }

    [Test]
    public async Task CreateExercise_WithEmptyName_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new CreateExerciseRequest(
            "",
            ExerciseType.RepsAndWeight,
            [MuscleGroup.Chest],
            "Description");
        var client = GetAnonymousClient();

        // Act
        var result = await client.POSTAsync<CreateExerciseEndpoint, CreateExerciseRequest, CreateExerciseResponse>(request);

        // Assert
        await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task CreateExercise_WithNoMuscleGroups_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new CreateExerciseRequest(
            "Squat",
            ExerciseType.RepsAndWeight,
            [],
            "A leg exercise");
        var client = GetAnonymousClient();

        // Act
        var result = await client.POSTAsync<CreateExerciseEndpoint, CreateExerciseRequest, CreateExerciseResponse>(request);

        // Assert
        await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task CreateExercise_WithInvalidType_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new CreateExerciseRequest(
            "Invalid Exercise",
            (ExerciseType)999,
            [MuscleGroup.Chest],
            "Invalid type");
        var client = GetAnonymousClient();

        // Act
        var result = await client.POSTAsync<CreateExerciseEndpoint, CreateExerciseRequest, CreateExerciseResponse>(request);

        // Assert
        await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task CreateExercise_WithTooLongName_ShouldReturnBadRequest()
    {
        // Arrange
        var longName = new string('a', Exercise.NameMaxLength + 1);
        var request = new CreateExerciseRequest(
            longName,
            ExerciseType.RepsAndWeight,
            [MuscleGroup.Chest],
            "Description");
        var client = GetAnonymousClient();

        // Act
        var result = await client.POSTAsync<CreateExerciseEndpoint, CreateExerciseRequest, CreateExerciseResponse>(request);

        // Assert
        await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task CreateExercise_WithTooLongDescription_ShouldReturnBadRequest()
    {
        // Arrange
        var longDescription = new string('a', Exercise.DescriptionMaxLength + 1);
        var request = new CreateExerciseRequest(
            "Valid Name",
            ExerciseType.RepsAndWeight,
            [MuscleGroup.Chest],
            longDescription);
        var client = GetAnonymousClient();

        // Act
        var result = await client.POSTAsync<CreateExerciseEndpoint, CreateExerciseRequest, CreateExerciseResponse>(request);

        // Assert
        await Assert.That(result.Response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
    }
}
