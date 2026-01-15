using Microsoft.EntityFrameworkCore;
using GymBuddy.Api.IntegrationTests.Common;
using GymBuddy.Api.IntegrationTests.Common.Factories;
using GymBuddy.Domain.Common;
using GymBuddy.Domain.Exercises;
using GymBuddy.Domain.Users;
using GymBuddy.Domain.WorkoutPlans;
using GymBuddy.Domain.WorkoutSessions;

namespace GymBuddy.Api.IntegrationTests.Persistence;

public class WorkoutSessionPersistenceTests : IntegrationTestBase
{
    [Test]
    public async Task CreateAndSave_ShouldPersistWorkoutSession()
    {
        // Arrange
        var trainer = UserFactory.GenerateTrainer();
        UserId trainerId = trainer.Id;
        await AddAsync(trainer);

        var client = UserFactory.GenerateClient();
        UserId clientId = client.Id;
        await AddAsync(client);

        var exercise = ExerciseFactory.GenerateRepsAndWeight();
        await AddAsync(exercise);

        var workoutPlan = WorkoutPlanFactory.Generate(trainerId);
        workoutPlan.AddExercise(exercise, sets: 3, reps: 10, weight: new Weight(50));
        await AddAsync(workoutPlan);

        var session = WorkoutSessionFactory.Generate(clientId, workoutPlan);
        WorkoutSessionId sessionId = session.Id;

        // Act
        await AddAsync(session);

        // Assert
        DetachAllEntities();
        var retrieved = await GetQueryable<WorkoutSession>()
            .Include(s => s.Exercises)
            .FirstOrDefaultAsync(s => s.Id == sessionId, CancellationToken);

        await Assert.That(retrieved).IsNotNull();
        await Assert.That(retrieved!.ClientId).IsEqualTo(clientId);
        await Assert.That(retrieved.WorkoutPlanId).IsEqualTo(workoutPlan.Id);
        await Assert.That(retrieved.WorkoutPlanName).IsEqualTo(workoutPlan.Name);
        await Assert.That(retrieved.Status).IsEqualTo(SessionStatus.InProgress);
        await Assert.That(retrieved.StartedAt).IsBetween(DateTimeOffset.UtcNow.AddSeconds(-10), DateTimeOffset.UtcNow.AddSeconds(10));
        await Assert.That(retrieved.CompletedAt).IsNull();
        await Assert.That(retrieved.CreatedAt).IsBetween(DateTimeOffset.UtcNow.AddSeconds(-10), DateTimeOffset.UtcNow.AddSeconds(10));
    }

    [Test]
    public async Task CreateAndSave_WithSessionExercises_ShouldPersistSessionExercises()
    {
        // Arrange
        var trainer = UserFactory.GenerateTrainer();
        UserId trainerId = trainer.Id;
        await AddAsync(trainer);

        var client = UserFactory.GenerateClient();
        UserId clientId = client.Id;
        await AddAsync(client);

        var exercise = ExerciseFactory.GenerateRepsAndWeight();
        ExerciseId exerciseId = exercise.Id;
        await AddAsync(exercise);

        var workoutPlan = WorkoutPlanFactory.Generate(trainerId);
        workoutPlan.AddExercise(exercise, sets: 3, reps: 10, weight: new Weight(50));
        await AddAsync(workoutPlan);

        var session = WorkoutSessionFactory.Generate(clientId, workoutPlan);
        WorkoutSessionId sessionId = session.Id;

        // Act
        await AddAsync(session);

        // Assert
        DetachAllEntities();
        var retrieved = await GetQueryable<WorkoutSession>()
            .Include(s => s.Exercises)
            .FirstOrDefaultAsync(s => s.Id == sessionId, CancellationToken);

        await Assert.That(retrieved).IsNotNull();
        await Assert.That(retrieved!.Exercises.Count).IsEqualTo(1);

        var sessionExercise = retrieved.Exercises.First();
        await Assert.That(sessionExercise.ExerciseId).IsEqualTo(exerciseId);
        await Assert.That(sessionExercise.ExerciseName).IsEqualTo(exercise.Name);
        await Assert.That(sessionExercise.ExerciseType).IsEqualTo(ExerciseType.RepsAndWeight);
        await Assert.That(sessionExercise.TargetSets).IsEqualTo(3);
        await Assert.That(sessionExercise.TargetReps).IsEqualTo(10);
        await Assert.That(sessionExercise.TargetWeight!.Value).IsEqualTo(50);
        await Assert.That(sessionExercise.ActualSets).IsNull();
        await Assert.That(sessionExercise.IsCompleted).IsFalse();
    }

    [Test]
    public async Task CreateAndSave_WithMultipleExercises_ShouldPersistAllSessionExercises()
    {
        // Arrange
        var trainer = UserFactory.GenerateTrainer();
        UserId trainerId = trainer.Id;
        await AddAsync(trainer);

        var client = UserFactory.GenerateClient();
        UserId clientId = client.Id;
        await AddAsync(client);

        var exercise1 = ExerciseFactory.GenerateRepsAndWeight();
        var exercise2 = ExerciseFactory.GenerateRepsAndWeight();
        await AddRangeAsync([exercise1, exercise2]);

        var workoutPlan = WorkoutPlanFactory.Generate(trainerId);
        workoutPlan.AddExercise(exercise1, sets: 3, reps: 10, weight: new Weight(50));
        workoutPlan.AddExercise(exercise2, sets: 4, reps: 8, weight: new Weight(60));
        await AddAsync(workoutPlan);

        var session = WorkoutSessionFactory.Generate(clientId, workoutPlan);
        WorkoutSessionId sessionId = session.Id;

        // Act
        await AddAsync(session);

        // Assert
        DetachAllEntities();
        var retrieved = await GetQueryable<WorkoutSession>()
            .Include(s => s.Exercises)
            .FirstOrDefaultAsync(s => s.Id == sessionId, CancellationToken);

        await Assert.That(retrieved).IsNotNull();
        await Assert.That(retrieved!.Exercises.Count).IsEqualTo(2);

        var orderedExercises = retrieved.Exercises.OrderBy(e => e.Order).ToList();
        await Assert.That(orderedExercises[0].ExerciseType).IsEqualTo(ExerciseType.RepsAndWeight);
        await Assert.That(orderedExercises[1].ExerciseType).IsEqualTo(ExerciseType.RepsAndWeight);
    }

    [Test]
    public async Task Update_CompleteExercise_ShouldPersistActualValues()
    {
        // Arrange
        var trainer = UserFactory.GenerateTrainer();
        UserId trainerId = trainer.Id;
        await AddAsync(trainer);

        var client = UserFactory.GenerateClient();
        UserId clientId = client.Id;
        await AddAsync(client);

        var exercise = ExerciseFactory.GenerateRepsAndWeight();
        ExerciseId exerciseId = exercise.Id;
        await AddAsync(exercise);

        var workoutPlan = WorkoutPlanFactory.Generate(trainerId);
        workoutPlan.AddExercise(exercise, sets: 3, reps: 10, weight: new Weight(50));
        await AddAsync(workoutPlan);

        var session = WorkoutSessionFactory.Generate(clientId, workoutPlan);
        WorkoutSessionId sessionId = session.Id;
        await AddAsync(session);
        DetachAllEntities();

        // Act
        var sessionToUpdate = await GetBySpecAsync(new WorkoutSessionByIdSpec(sessionId));
        sessionToUpdate!.CompleteExercise(
            exerciseId,
            actualSets: 3,
            TimeProvider.System,
            actualReps: 12,
            actualWeight: new Weight(55));
        await SaveAsync();

        // Assert
        DetachAllEntities();
        var retrieved = await GetQueryable<WorkoutSession>()
            .Include(s => s.Exercises)
            .FirstOrDefaultAsync(s => s.Id == sessionId, CancellationToken);

        await Assert.That(retrieved).IsNotNull();
        var sessionExercise = retrieved!.Exercises.First();
        await Assert.That(sessionExercise.IsCompleted).IsTrue();
        await Assert.That(sessionExercise.ActualSets).IsEqualTo(3);
        await Assert.That(sessionExercise.ActualReps).IsEqualTo(12);
        await Assert.That(sessionExercise.ActualWeight!.Value).IsEqualTo(55);
        await Assert.That(sessionExercise.CompletedAt).IsNotNull();
    }

    [Test]
    public async Task Update_CompleteSession_ShouldPersistCompletedStatus()
    {
        // Arrange
        var trainer = UserFactory.GenerateTrainer();
        UserId trainerId = trainer.Id;
        await AddAsync(trainer);

        var client = UserFactory.GenerateClient();
        UserId clientId = client.Id;
        await AddAsync(client);

        var exercise = ExerciseFactory.GenerateRepsAndWeight();
        await AddAsync(exercise);

        var workoutPlan = WorkoutPlanFactory.Generate(trainerId);
        workoutPlan.AddExercise(exercise, sets: 3, reps: 10, weight: new Weight(50));
        await AddAsync(workoutPlan);

        var session = WorkoutSessionFactory.Generate(clientId, workoutPlan);
        WorkoutSessionId sessionId = session.Id;
        await AddAsync(session);

        // Act
        var trackedSession = await GetBySpecAsync(new WorkoutSessionByIdSpec(sessionId));
        trackedSession!.Complete(TimeProvider.System);
        await SaveAsync();

        // Assert
        DetachAllEntities();
        var retrieved = await GetQueryable<WorkoutSession>()
            .FirstOrDefaultAsync(s => s.Id == sessionId, CancellationToken);

        await Assert.That(retrieved).IsNotNull();
        await Assert.That(retrieved!.Status).IsEqualTo(SessionStatus.Completed);
        await Assert.That(retrieved.CompletedAt).IsNotNull();
    }

    [Test]
    public async Task Update_AbandonSession_ShouldPersistAbandonedStatus()
    {
        // Arrange
        var trainer = UserFactory.GenerateTrainer();
        UserId trainerId = trainer.Id;
        await AddAsync(trainer);

        var client = UserFactory.GenerateClient();
        UserId clientId = client.Id;
        await AddAsync(client);

        var exercise = ExerciseFactory.GenerateRepsAndWeight();
        await AddAsync(exercise);

        var workoutPlan = WorkoutPlanFactory.Generate(trainerId);
        workoutPlan.AddExercise(exercise, sets: 3, reps: 10, weight: new Weight(50));
        await AddAsync(workoutPlan);

        var session = WorkoutSessionFactory.Generate(clientId, workoutPlan);
        WorkoutSessionId sessionId = session.Id;
        await AddAsync(session);

        // Act
        var trackedSession = await GetBySpecAsync(new WorkoutSessionByIdSpec(sessionId));
        trackedSession!.Abandon(TimeProvider.System);
        await SaveAsync();

        // Assert
        DetachAllEntities();
        var retrieved = await GetQueryable<WorkoutSession>()
            .FirstOrDefaultAsync(s => s.Id == sessionId, CancellationToken);

        await Assert.That(retrieved).IsNotNull();
        await Assert.That(retrieved!.Status).IsEqualTo(SessionStatus.Abandoned);
        await Assert.That(retrieved.CompletedAt).IsNotNull();
    }

    [Test]
    public async Task Delete_ShouldRemoveWorkoutSession()
    {
        // Arrange
        var trainer = UserFactory.GenerateTrainer();
        UserId trainerId = trainer.Id;
        await AddAsync(trainer);

        var client = UserFactory.GenerateClient();
        UserId clientId = client.Id;
        await AddAsync(client);

        var exercise = ExerciseFactory.GenerateRepsAndWeight();
        await AddAsync(exercise);

        var workoutPlan = WorkoutPlanFactory.Generate(trainerId);
        workoutPlan.AddExercise(exercise, sets: 3, reps: 10, weight: new Weight(50));
        await AddAsync(workoutPlan);

        var session = WorkoutSessionFactory.Generate(clientId, workoutPlan);
        WorkoutSessionId sessionId = session.Id;
        await AddAsync(session);

        // Act
        var sessionToDelete = await GetBySpecAsync(new WorkoutSessionByIdSpec(sessionId));
        await RemoveAsync(sessionToDelete!);

        // Assert
        DetachAllEntities();
        var retrieved = await GetQueryable<WorkoutSession>().FirstOrDefaultAsync(s => s.Id == sessionId, CancellationToken);

        await Assert.That(retrieved).IsNull();
    }

    [Test]
    public async Task Delete_WithSessionExercises_ShouldRemoveSessionAndExercises()
    {
        // Arrange
        var trainer = UserFactory.GenerateTrainer();
        UserId trainerId = trainer.Id;
        await AddAsync(trainer);

        var client = UserFactory.GenerateClient();
        UserId clientId = client.Id;
        await AddAsync(client);

        var exercise = ExerciseFactory.GenerateRepsAndWeight();
        await AddAsync(exercise);

        var workoutPlan = WorkoutPlanFactory.Generate(trainerId);
        workoutPlan.AddExercise(exercise, sets: 3, reps: 10, weight: new Weight(50));
        await AddAsync(workoutPlan);

        var session = WorkoutSessionFactory.Generate(clientId, workoutPlan);
        WorkoutSessionId sessionId = session.Id;
        await AddAsync(session);

        // Act
        var sessionToDelete = await GetQueryable<WorkoutSession>()
            .Include(s => s.Exercises)
            .FirstAsync(s => s.Id == sessionId, CancellationToken);
        DetachAllEntities();

        var trackedSession = await GetBySpecAsync(new WorkoutSessionByIdSpec(sessionId));
        await RemoveAsync(trackedSession!);

        // Assert
        DetachAllEntities();
        var retrieved = await GetQueryable<WorkoutSession>().FirstOrDefaultAsync(s => s.Id == sessionId, CancellationToken);
        var sessionExerciseCount = await GetQueryable<SessionExercise>().CountAsync(CancellationToken);

        await Assert.That(retrieved).IsNull();
        await Assert.That(sessionExerciseCount).IsEqualTo(0);
    }
}
