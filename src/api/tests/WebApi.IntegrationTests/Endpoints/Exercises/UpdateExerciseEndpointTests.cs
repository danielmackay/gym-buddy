using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using GymBuddy.Domain.Exercises;
using GymBuddy.Api.Features.Exercises.Endpoints;
using GymBuddy.Api.IntegrationTests.Common;
using GymBuddy.Api.IntegrationTests.Common.Factories;
using System.Net;

namespace GymBuddy.Api.IntegrationTests.Endpoints.Exercises;

public class UpdateExerciseEndpointTests : IntegrationTestBase
{
    [Test]
    public async Task UpdateExercise_WithValidChanges_ShouldUpdateExercise()
    {
        // Arrange
        var exercise = Exercise.Create(
            "Bench Press",
            ExerciseType.RepsAndWeight,
            [MuscleGroup.Chest],
            "Original description").Value;
        await AddAsync(exercise);

        var request = new UpdateExerciseRequest(
            exercise.Id.Value,
            "Barbell Bench Press",
            [MuscleGroup.Chest, MuscleGroup.Triceps],
            "Updated description");
        var client = GetAnonymousClient();

        // Act
        var result = await client.PUTAsync<UpdateExerciseEndpoint, UpdateExerciseRequest>(request);

        // Assert
        await Assert.That(result.StatusCode).IsEqualTo(HttpStatusCode.OK);

        // Verify changes persisted
        DetachAllEntities();
        var exerciseId = ExerciseId.From(exercise.Id.Value);
        var updatedExercise = await GetQueryable<Exercise>().FirstOrDefaultAsync(e => e.Id == exerciseId, CancellationToken);
        await Assert.That(updatedExercise).IsNotNull();
        await Assert.That(updatedExercise!.Name).IsEqualTo("Barbell Bench Press");
        await Assert.That(updatedExercise.Description).IsEqualTo("Updated description");
        await Assert.That(updatedExercise.MuscleGroups.Count).IsEqualTo(2);
        await Assert.That(updatedExercise.MuscleGroups).Contains(MuscleGroup.Chest);
        await Assert.That(updatedExercise.MuscleGroups).Contains(MuscleGroup.Triceps);
    }

    [Test]
    public async Task UpdateExercise_WithEmptyMuscleGroups_ShouldReturnBadRequest()
    {
        // Arrange
        var exercise = ExerciseFactory.Generate();
        await AddAsync(exercise);

        var request = new UpdateExerciseRequest(
            exercise.Id.Value,
            "Valid Name",
            [],
            null);
        var client = GetAnonymousClient();

        // Act
        var result = await client.PUTAsync<UpdateExerciseEndpoint, UpdateExerciseRequest>(request);

        // Assert
        await Assert.That(result.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task UpdateExercise_WithTooLongName_ShouldReturnBadRequest()
    {
        // Arrange
        var exercise = ExerciseFactory.Generate();
        await AddAsync(exercise);

        var longName = new string('a', Exercise.NameMaxLength + 1);
        var request = new UpdateExerciseRequest(
            exercise.Id.Value,
            longName,
            [MuscleGroup.Chest],
            null);
        var client = GetAnonymousClient();

        // Act
        var result = await client.PUTAsync<UpdateExerciseEndpoint, UpdateExerciseRequest>(request);

        // Assert
        await Assert.That(result.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task UpdateExercise_WithTooLongDescription_ShouldReturnBadRequest()
    {
        // Arrange
        var exercise = ExerciseFactory.Generate();
        await AddAsync(exercise);

        var longDescription = new string('a', Exercise.DescriptionMaxLength + 1);
        var request = new UpdateExerciseRequest(
            exercise.Id.Value,
            "Valid Name",
            [MuscleGroup.Chest],
            longDescription);
        var client = GetAnonymousClient();

        // Act
        var result = await client.PUTAsync<UpdateExerciseEndpoint, UpdateExerciseRequest>(request);

        // Assert
        await Assert.That(result.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task UpdateExercise_WithEmptyId_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new UpdateExerciseRequest(
            Guid.Empty,
            "Exercise Name",
            [MuscleGroup.Chest],
            null);
        var client = GetAnonymousClient();

        // Act
        var result = await client.PUTAsync<UpdateExerciseEndpoint, UpdateExerciseRequest>(request);

        // Assert
        await Assert.That(result.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task UpdateExercise_CannotChangeType_TypeRemainsUnchanged()
    {
        // Arrange
        var exercise = Exercise.Create(
            "Bench Press",
            ExerciseType.RepsAndWeight,
            [MuscleGroup.Chest],
            null).Value;
        await AddAsync(exercise);

        // Note: UpdateExerciseRequest doesn't include Type parameter
        // This test verifies that the type remains unchanged after update
        var request = new UpdateExerciseRequest(
            exercise.Id.Value,
            "Bench Press Updated",
            [MuscleGroup.Chest],
            null);
        var client = GetAnonymousClient();

        // Act
        var result = await client.PUTAsync<UpdateExerciseEndpoint, UpdateExerciseRequest>(request);

        // Assert
        await Assert.That(result.StatusCode).IsEqualTo(HttpStatusCode.OK);
        DetachAllEntities();
        var exerciseId = ExerciseId.From(exercise.Id.Value);
        var updatedExercise = await GetQueryable<Exercise>().FirstOrDefaultAsync(e => e.Id == exerciseId, CancellationToken);
        await Assert.That(updatedExercise!.Type).IsEqualTo(ExerciseType.RepsAndWeight);
    }
}
