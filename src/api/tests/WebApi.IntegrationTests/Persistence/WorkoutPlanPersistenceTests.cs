using Microsoft.EntityFrameworkCore;
using GymBuddy.Api.IntegrationTests.Common;
using GymBuddy.Api.IntegrationTests.Common.Factories;
using GymBuddy.Domain.Common;
using GymBuddy.Domain.Exercises;
using GymBuddy.Domain.Users;
using GymBuddy.Domain.WorkoutPlans;

namespace GymBuddy.Api.IntegrationTests.Persistence;

public class WorkoutPlanPersistenceTests : IntegrationTestBase
{
    [Test]
    public async Task CreateAndSave_ShouldPersistWorkoutPlan()
    {
        // Arrange
        var trainer = UserFactory.GenerateTrainer();
        UserId trainerId = trainer.Id;
        await AddAsync(trainer);

        var workoutPlan = WorkoutPlanFactory.Generate(trainerId);
        WorkoutPlanId workoutPlanId = workoutPlan.Id;

        // Act
        await AddAsync(workoutPlan);

        // Assert
        DetachAllEntities();
        var retrieved = await GetQueryable<WorkoutPlan>().FirstOrDefaultAsync(w => w.Id == workoutPlanId, CancellationToken);

        await Assert.That(retrieved).IsNotNull();
        await Assert.That(retrieved!.Name).IsEqualTo(workoutPlan.Name);
        await Assert.That(retrieved.Description).IsEqualTo(workoutPlan.Description);
        await Assert.That(retrieved.TrainerId).IsEqualTo(trainerId);
        await Assert.That(retrieved.CreatedAt).IsBetween(DateTimeOffset.UtcNow.AddSeconds(-10), DateTimeOffset.UtcNow.AddSeconds(10));
    }

    [Test]
    public async Task CreateAndSave_WithRepsAndWeightExercise_ShouldPersistWorkoutPlanWithExercise()
    {
        // Arrange
        var trainer = UserFactory.GenerateTrainer();
        UserId trainerId = trainer.Id;
        await AddAsync(trainer);

        var exercise = ExerciseFactory.GenerateRepsAndWeight();
        ExerciseId exerciseId = exercise.Id;
        await AddAsync(exercise);

        var workoutPlan = WorkoutPlanFactory.Generate(trainerId);
        workoutPlan.AddExercise(exercise, sets: 3, reps: 10, weight: new Weight(50));
        WorkoutPlanId workoutPlanId = workoutPlan.Id;

        // Act
        await AddAsync(workoutPlan);

        // Assert
        DetachAllEntities();
        var retrieved = await GetQueryable<WorkoutPlan>()
            .Include(w => w.Exercises)
            .FirstOrDefaultAsync(w => w.Id == workoutPlanId, CancellationToken);

        await Assert.That(retrieved).IsNotNull();
        await Assert.That(retrieved!.Exercises.Count).IsEqualTo(1);

        var plannedExercise = retrieved.Exercises.First();
        await Assert.That(plannedExercise.ExerciseId).IsEqualTo(exerciseId);
        await Assert.That(plannedExercise.Sets).IsEqualTo(3);
        await Assert.That(plannedExercise.Reps).IsEqualTo(10);
        await Assert.That(plannedExercise.Weight).IsNotNull();
        await Assert.That(plannedExercise.Weight!.Value).IsEqualTo(50);
        await Assert.That(plannedExercise.Order).IsEqualTo(1);
    }

    [Test]
    public async Task CreateAndSave_WithSecondExercise_ShouldPersistWorkoutPlanWithExercise()
    {
        // Arrange
        var trainer = UserFactory.GenerateTrainer();
        UserId trainerId = trainer.Id;
        await AddAsync(trainer);

        var exercise = ExerciseFactory.GenerateRepsAndWeight();
        ExerciseId exerciseId = exercise.Id;
        await AddAsync(exercise);

        var workoutPlan = WorkoutPlanFactory.Generate(trainerId);
        workoutPlan.AddExercise(exercise, sets: 2, reps: 15, weight: new Weight(25));
        WorkoutPlanId workoutPlanId = workoutPlan.Id;

        // Act
        await AddAsync(workoutPlan);

        // Assert
        DetachAllEntities();
        var retrieved = await GetQueryable<WorkoutPlan>()
            .Include(w => w.Exercises)
            .FirstOrDefaultAsync(w => w.Id == workoutPlanId, CancellationToken);

        await Assert.That(retrieved).IsNotNull();
        await Assert.That(retrieved!.Exercises.Count).IsEqualTo(1);

        var plannedExercise = retrieved.Exercises.First();
        await Assert.That(plannedExercise.ExerciseId).IsEqualTo(exerciseId);
        await Assert.That(plannedExercise.Sets).IsEqualTo(2);
        await Assert.That(plannedExercise.Reps).IsEqualTo(15);
        await Assert.That(plannedExercise.Weight).IsNotNull();
        await Assert.That(plannedExercise.Weight!.Value).IsEqualTo(25);
    }

    [Test]
    public async Task CreateAndSave_WithMultipleExercises_ShouldPersistAllExercisesWithCorrectOrder()
    {
        // Arrange
        var trainer = UserFactory.GenerateTrainer();
        UserId trainerId = trainer.Id;
        await AddAsync(trainer);

        var exercise1 = ExerciseFactory.GenerateRepsAndWeight();
        var exercise2 = ExerciseFactory.GenerateRepsAndWeight();
        var exercise3 = ExerciseFactory.GenerateRepsAndWeight();
        await AddRangeAsync([exercise1, exercise2, exercise3]);

        var workoutPlan = WorkoutPlanFactory.Generate(trainerId);
        workoutPlan.AddExercise(exercise1, sets: 3, reps: 10, weight: new Weight(50));
        workoutPlan.AddExercise(exercise2, sets: 4, reps: 8, weight: new Weight(60));
        workoutPlan.AddExercise(exercise3, sets: 2, reps: 12, weight: new Weight(40));
        WorkoutPlanId workoutPlanId = workoutPlan.Id;

        // Act
        await AddAsync(workoutPlan);

        // Assert
        DetachAllEntities();
        var retrieved = await GetQueryable<WorkoutPlan>()
            .Include(w => w.Exercises)
            .FirstOrDefaultAsync(w => w.Id == workoutPlanId, CancellationToken);

        await Assert.That(retrieved).IsNotNull();
        await Assert.That(retrieved!.Exercises.Count).IsEqualTo(3);

        var orderedExercises = retrieved.Exercises.OrderBy(e => e.Order).ToList();
        await Assert.That(orderedExercises[0].Order).IsEqualTo(1);
        await Assert.That(orderedExercises[1].Order).IsEqualTo(2);
        await Assert.That(orderedExercises[2].Order).IsEqualTo(3);
    }

    [Test]
    public async Task Update_ShouldPersistChanges()
    {
        // Arrange
        var trainer = UserFactory.GenerateTrainer();
        UserId trainerId = trainer.Id;
        await AddAsync(trainer);

        var workoutPlan = WorkoutPlanFactory.Generate(trainerId);
        WorkoutPlanId workoutPlanId = workoutPlan.Id;
        await AddAsync(workoutPlan);

        // Act
        var planToUpdate = await GetBySpecAsync(new WorkoutPlanByIdSpec(workoutPlanId));
        planToUpdate!.Name = "Updated Workout Plan";
        planToUpdate.Description = "Updated description";
        await SaveAsync();

        // Assert
        DetachAllEntities();
        var retrieved = await GetQueryable<WorkoutPlan>().FirstOrDefaultAsync(w => w.Id == workoutPlanId, CancellationToken);

        await Assert.That(retrieved).IsNotNull();
        await Assert.That(retrieved!.Name).IsEqualTo("Updated Workout Plan");
        await Assert.That(retrieved.Description).IsEqualTo("Updated description");
        await Assert.That(retrieved.UpdatedAt).IsNotNull();
    }

    [Test]
    public async Task Update_RemoveExercise_ShouldPersistExerciseRemoval()
    {
        // Arrange
        var trainer = UserFactory.GenerateTrainer();
        UserId trainerId = trainer.Id;
        await AddAsync(trainer);

        var exercise1 = ExerciseFactory.GenerateRepsAndWeight();
        var exercise2 = ExerciseFactory.GenerateRepsAndWeight();
        ExerciseId exercise1Id = exercise1.Id;
        ExerciseId exercise2Id = exercise2.Id;
        await AddRangeAsync([exercise1, exercise2]);

        var workoutPlan = WorkoutPlanFactory.Generate(trainerId);
        workoutPlan.AddExercise(exercise1, sets: 3, reps: 10, weight: new Weight(50));
        workoutPlan.AddExercise(exercise2, sets: 4, reps: 8, weight: new Weight(60));
        WorkoutPlanId workoutPlanId = workoutPlan.Id;
        await AddAsync(workoutPlan);
        DetachAllEntities();

        // Act
        var planToUpdate = await GetBySpecAsync(new WorkoutPlanByIdSpec(workoutPlanId));
        planToUpdate!.RemoveExercise(exercise1Id);
        await SaveAsync();

        // Assert
        DetachAllEntities();
        var retrieved = await GetQueryable<WorkoutPlan>()
            .Include(w => w.Exercises)
            .FirstOrDefaultAsync(w => w.Id == workoutPlanId, CancellationToken);

        await Assert.That(retrieved).IsNotNull();
        await Assert.That(retrieved!.Exercises.Count).IsEqualTo(1);
        await Assert.That(retrieved.Exercises.First().ExerciseId).IsEqualTo(exercise2Id);
        await Assert.That(retrieved.Exercises.First().Order).IsEqualTo(1); // Order should be updated
    }

    [Test]
    public async Task Update_UpdateExercise_ShouldPersistExerciseChanges()
    {
        // Arrange
        var trainer = UserFactory.GenerateTrainer();
        UserId trainerId = trainer.Id;
        await AddAsync(trainer);

        var exercise = ExerciseFactory.GenerateRepsAndWeight();
        ExerciseId exerciseId = exercise.Id;
        await AddAsync(exercise);

        var workoutPlan = WorkoutPlanFactory.Generate(trainerId);
        workoutPlan.AddExercise(exercise, sets: 3, reps: 10, weight: new Weight(50));
        WorkoutPlanId workoutPlanId = workoutPlan.Id;
        await AddAsync(workoutPlan);
        DetachAllEntities();

        // Act
        var planToUpdate = await GetBySpecAsync(new WorkoutPlanByIdSpec(workoutPlanId));
        planToUpdate!.UpdateExercise(exerciseId, sets: 5, reps: 12, weight: new Weight(75));
        await SaveAsync();

        // Assert
        DetachAllEntities();
        var retrieved = await GetQueryable<WorkoutPlan>()
            .Include(w => w.Exercises)
            .FirstOrDefaultAsync(w => w.Id == workoutPlanId, CancellationToken);

        await Assert.That(retrieved).IsNotNull();
        var plannedExercise = retrieved!.Exercises.First();
        await Assert.That(plannedExercise.Sets).IsEqualTo(5);
        await Assert.That(plannedExercise.Reps).IsEqualTo(12);
        await Assert.That(plannedExercise.Weight!.Value).IsEqualTo(75);
    }

    [Test]
    public async Task Delete_ShouldRemoveWorkoutPlan()
    {
        // Arrange
        var trainer = UserFactory.GenerateTrainer();
        UserId trainerId = trainer.Id;
        await AddAsync(trainer);

        var workoutPlan = WorkoutPlanFactory.Generate(trainerId);
        WorkoutPlanId workoutPlanId = workoutPlan.Id;
        await AddAsync(workoutPlan);

        // Act
        var planToDelete = await GetBySpecAsync(new WorkoutPlanByIdSpec(workoutPlanId));
        await RemoveAsync(planToDelete!);

        // Assert
        DetachAllEntities();
        var retrieved = await GetQueryable<WorkoutPlan>().FirstOrDefaultAsync(w => w.Id == workoutPlanId, CancellationToken);

        await Assert.That(retrieved).IsNull();
    }

    [Test]
    public async Task Delete_WithExercises_ShouldRemoveWorkoutPlanAndExercises()
    {
        // Arrange
        var trainer = UserFactory.GenerateTrainer();
        UserId trainerId = trainer.Id;
        await AddAsync(trainer);

        var exercise = ExerciseFactory.GenerateRepsAndWeight();
        await AddAsync(exercise);

        var workoutPlan = WorkoutPlanFactory.Generate(trainerId);
        workoutPlan.AddExercise(exercise, sets: 3, reps: 10, weight: new Weight(50));
        WorkoutPlanId workoutPlanId = workoutPlan.Id;
        await AddAsync(workoutPlan);

        // Act
        var planToDelete = await GetQueryable<WorkoutPlan>()
            .Include(w => w.Exercises)
            .FirstAsync(w => w.Id == workoutPlanId, CancellationToken);
        DetachAllEntities();

        var trackedPlan = await GetBySpecAsync(new WorkoutPlanByIdSpec(workoutPlanId));
        await RemoveAsync(trackedPlan!);

        // Assert
        DetachAllEntities();
        var retrieved = await GetQueryable<WorkoutPlan>().FirstOrDefaultAsync(w => w.Id == workoutPlanId, CancellationToken);
        var plannedExerciseCount = await GetQueryable<PlannedExercise>().CountAsync(CancellationToken);

        await Assert.That(retrieved).IsNull();
        await Assert.That(plannedExerciseCount).IsEqualTo(0);
    }
}
