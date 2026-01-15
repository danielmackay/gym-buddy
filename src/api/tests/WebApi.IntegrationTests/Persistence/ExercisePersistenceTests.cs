using Microsoft.EntityFrameworkCore;
using GymBuddy.Api.IntegrationTests.Common;
using GymBuddy.Api.IntegrationTests.Common.Factories;
using GymBuddy.Domain.Exercises;

namespace GymBuddy.Api.IntegrationTests.Persistence;

public class ExercisePersistenceTests : IntegrationTestBase
{
    [Test]
    public async Task CreateAndSave_RepsAndWeightExercise_ShouldPersistExercise()
    {
        // Arrange
        var exercise = ExerciseFactory.GenerateRepsAndWeight();
        ExerciseId exerciseId = exercise.Id;

        // Act
        await AddAsync(exercise);

        // Assert
        DetachAllEntities();
        var retrieved = await GetQueryable<Exercise>().FirstOrDefaultAsync(e => e.Id == exerciseId, CancellationToken);

        await Assert.That(retrieved).IsNotNull();
        await Assert.That(retrieved!.Name).IsEqualTo(exercise.Name);
        await Assert.That(retrieved.Description).IsEqualTo(exercise.Description);
        await Assert.That(retrieved.Type).IsEqualTo(ExerciseType.RepsAndWeight);
        await Assert.That(retrieved.MuscleGroups.Count).IsGreaterThanOrEqualTo(1);
        await Assert.That(retrieved.CreatedAt).IsBetween(DateTimeOffset.UtcNow.AddSeconds(-10), DateTimeOffset.UtcNow.AddSeconds(10));
    }

    [Test]
    public async Task CreateAndSave_TimeBasedExercise_ShouldPersistExercise()
    {
        // Arrange
        var exercise = ExerciseFactory.GenerateTimeBased();
        ExerciseId exerciseId = exercise.Id;

        // Act
        await AddAsync(exercise);

        // Assert
        DetachAllEntities();
        var retrieved = await GetQueryable<Exercise>().FirstOrDefaultAsync(e => e.Id == exerciseId, CancellationToken);

        await Assert.That(retrieved).IsNotNull();
        await Assert.That(retrieved!.Name).IsEqualTo(exercise.Name);
        await Assert.That(retrieved.Type).IsEqualTo(ExerciseType.TimeBased);
        await Assert.That(retrieved.MuscleGroups.Count).IsGreaterThanOrEqualTo(1);
    }

    [Test]
    public async Task CreateAndSave_WithMultipleMuscleGroups_ShouldPersistAllMuscleGroups()
    {
        // Arrange
        var result = Exercise.Create(
            "Compound Exercise",
            ExerciseType.RepsAndWeight,
            [MuscleGroup.Chest, MuscleGroup.Shoulders, MuscleGroup.Triceps],
            "A compound exercise targeting multiple muscle groups"
        );
        var exercise = result.Value;
        ExerciseId exerciseId = exercise.Id;

        // Act
        await AddAsync(exercise);

        // Assert
        DetachAllEntities();
        var retrieved = await GetQueryable<Exercise>().FirstOrDefaultAsync(e => e.Id == exerciseId, CancellationToken);

        await Assert.That(retrieved).IsNotNull();
        await Assert.That(retrieved!.MuscleGroups.Count).IsEqualTo(3);
        await Assert.That(retrieved.MuscleGroups).Contains(MuscleGroup.Chest);
        await Assert.That(retrieved.MuscleGroups).Contains(MuscleGroup.Shoulders);
        await Assert.That(retrieved.MuscleGroups).Contains(MuscleGroup.Triceps);
    }

    [Test]
    public async Task CreateAndSave_WithoutDescription_ShouldPersistExercise()
    {
        // Arrange
        var result = Exercise.Create(
            "Simple Exercise",
            ExerciseType.RepsAndWeight,
            [MuscleGroup.Biceps]
        );
        var exercise = result.Value;
        ExerciseId exerciseId = exercise.Id;

        // Act
        await AddAsync(exercise);

        // Assert
        DetachAllEntities();
        var retrieved = await GetQueryable<Exercise>().FirstOrDefaultAsync(e => e.Id == exerciseId, CancellationToken);

        await Assert.That(retrieved).IsNotNull();
        await Assert.That(retrieved!.Name).IsEqualTo("Simple Exercise");
        await Assert.That(retrieved.Description).IsNull();
    }

    [Test]
    public async Task CreateAndSave_MultipleExercises_ShouldPersistAll()
    {
        // Arrange
        var exercises = ExerciseFactory.Generate(5);

        // Act
        await AddRangeAsync(exercises);

        // Assert
        DetachAllEntities();
        var retrievedCount = await GetQueryable<Exercise>().CountAsync(CancellationToken);

        await Assert.That(retrievedCount).IsEqualTo(5);
    }

    [Test]
    public async Task Delete_ShouldRemoveExercise()
    {
        // Arrange
        var exercise = ExerciseFactory.Generate();
        ExerciseId exerciseId = exercise.Id;
        await AddAsync(exercise);

        // Act
        var exerciseToDelete = await GetBySpecAsync(new ExerciseByIdSpec(exerciseId));
        await RemoveAsync(exerciseToDelete!);

        // Assert
        DetachAllEntities();
        var retrieved = await GetQueryable<Exercise>().FirstOrDefaultAsync(e => e.Id == exerciseId, CancellationToken);

        await Assert.That(retrieved).IsNull();
    }

    [Test]
    public async Task Delete_ExerciseWithMuscleGroups_ShouldRemoveExerciseAndMuscleGroups()
    {
        // Arrange
        var result = Exercise.Create(
            "Multi-Muscle Exercise",
            ExerciseType.RepsAndWeight,
            [MuscleGroup.Back, MuscleGroup.Biceps, MuscleGroup.Forearms],
            "Exercise targeting back and arms"
        );
        var exercise = result.Value;
        ExerciseId exerciseId = exercise.Id;
        await AddAsync(exercise);

        // Act
        var exerciseToDelete = await GetBySpecAsync(new ExerciseByIdSpec(exerciseId));
        await RemoveAsync(exerciseToDelete!);

        // Assert
        DetachAllEntities();
        var retrieved = await GetQueryable<Exercise>().FirstOrDefaultAsync(e => e.Id == exerciseId, CancellationToken);

        await Assert.That(retrieved).IsNull();
    }
}
