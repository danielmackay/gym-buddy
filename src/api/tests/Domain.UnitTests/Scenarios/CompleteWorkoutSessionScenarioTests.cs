using GymBuddy.Domain.Exercises;
using GymBuddy.Domain.Users;
using GymBuddy.Domain.WorkoutPlans;
using GymBuddy.Domain.WorkoutSessions;

namespace GymBuddy.Domain.UnitTests.Scenarios;

public class CompleteWorkoutSessionScenarioTests
{
    /// <summary>
    /// Acceptance test that exercises the complete workout flow:
    /// 1. Create exercises
    /// 2. Create a trainer and client user
    /// 3. Create a workout plan with exercises
    /// 4. Assign the plan to a client
    /// 5. Start a workout session from the plan
    /// 6. Complete all exercises in the session
    /// 7. Complete the entire session
    /// </summary>
    [Test]
    public async Task CompleteWorkoutFlow_FromPlanCreationToSessionCompletion_ShouldSucceed()
    {
        // Arrange
        var timeProvider = TimeProvider.System;

        // ========================================
        // Step 1: Create exercises
        // ========================================
        var benchPressResult = Exercise.Create(
            name: "Bench Press",
            type: ExerciseType.RepsAndWeight,
            muscleGroups: [MuscleGroup.Chest, MuscleGroup.Triceps],
            description: "Barbell bench press for chest development");

        var squatResult = Exercise.Create(
            name: "Barbell Squat",
            type: ExerciseType.RepsAndWeight,
            muscleGroups: [MuscleGroup.Quadriceps, MuscleGroup.Glutes, MuscleGroup.Hamstrings],
            description: "Compound leg exercise");

        var plankResult = Exercise.Create(
            name: "Plank",
            type: ExerciseType.TimeBased,
            muscleGroups: [MuscleGroup.Abs, MuscleGroup.Obliques],
            description: "Core stability exercise");

        await Assert.That(benchPressResult.IsError).IsFalse();
        await Assert.That(squatResult.IsError).IsFalse();
        await Assert.That(plankResult.IsError).IsFalse();

        var benchPress = benchPressResult.Value;
        var squat = squatResult.Value;
        var plank = plankResult.Value;

        // ========================================
        // Step 2: Create trainer and client users
        // ========================================
        var trainer = User.Create("John Smith", "john.smith@gym.com");
        var trainerRoleResult = trainer.AddRole(UserRole.Trainer);
        await Assert.That(trainerRoleResult.IsError).IsFalse();

        var client = User.Create("Jane Doe", "jane.doe@email.com");
        var clientRoleResult = client.AddRole(UserRole.Client);
        await Assert.That(clientRoleResult.IsError).IsFalse();

        // Assign trainer to client
        var assignTrainerResult = client.AssignTrainer(trainer.Id);
        await Assert.That(assignTrainerResult.IsError).IsFalse();
        await Assert.That(client.TrainerId).IsEqualTo(trainer.Id);

        // ========================================
        // Step 3: Create workout plan with exercises
        // ========================================
        var workoutPlan = WorkoutPlan.Create(
            name: "Strength Training - Week 1",
            trainerId: trainer.Id,
            description: "Beginner strength training program focusing on compound movements");

        await Assert.That(workoutPlan.Name).IsEqualTo("Strength Training - Week 1");
        await Assert.That(workoutPlan.TrainerId).IsEqualTo(trainer.Id);

        // Add exercises to the plan
        var addBenchPressResult = workoutPlan.AddExercise(
            exercise: benchPress,
            sets: 3,
            reps: 10,
            weight: 60.0m);
        await Assert.That(addBenchPressResult.IsError).IsFalse();

        var addSquatResult = workoutPlan.AddExercise(
            exercise: squat,
            sets: 4,
            reps: 8,
            weight: 80.0m);
        await Assert.That(addSquatResult.IsError).IsFalse();

        var addPlankResult = workoutPlan.AddExercise(
            exercise: plank,
            sets: 3,
            durationSeconds: 60);
        await Assert.That(addPlankResult.IsError).IsFalse();

        await Assert.That(workoutPlan.Exercises).HasCount().EqualTo(3);
        await Assert.That(workoutPlan.Exercises[0].ExerciseName).IsEqualTo("Bench Press");
        await Assert.That(workoutPlan.Exercises[0].Order).IsEqualTo(1);
        await Assert.That(workoutPlan.Exercises[1].ExerciseName).IsEqualTo("Barbell Squat");
        await Assert.That(workoutPlan.Exercises[1].Order).IsEqualTo(2);
        await Assert.That(workoutPlan.Exercises[2].ExerciseName).IsEqualTo("Plank");
        await Assert.That(workoutPlan.Exercises[2].Order).IsEqualTo(3);

        // ========================================
        // Step 4: Assign the workout plan to the client
        // ========================================
        var assignPlanResult = client.AssignWorkoutPlan(workoutPlan.Id);
        await Assert.That(assignPlanResult.IsError).IsFalse();
        await Assert.That(client.AssignedWorkoutPlanIds).Contains(workoutPlan.Id);

        // ========================================
        // Step 5: Client starts a workout session from the plan
        // ========================================
        var session = WorkoutSession.Start(
            clientId: client.Id,
            workoutPlan: workoutPlan,
            timeProvider: timeProvider);

        await Assert.That(session.ClientId).IsEqualTo(client.Id);
        await Assert.That(session.WorkoutPlanId).IsEqualTo(workoutPlan.Id);
        await Assert.That(session.WorkoutPlanName).IsEqualTo(workoutPlan.Name);
        await Assert.That(session.Status).IsEqualTo(SessionStatus.InProgress);
        await Assert.That(session.Exercises).HasCount().EqualTo(3);
        await Assert.That(session.GetCompletedExerciseCount()).IsEqualTo(0);
        await Assert.That(session.AreAllExercisesCompleted()).IsFalse();

        // Verify exercise snapshots were created correctly
        var sessionBenchPress = session.Exercises.First(e => e.ExerciseName == "Bench Press");
        await Assert.That(sessionBenchPress.ExerciseId).IsEqualTo(benchPress.Id);
        await Assert.That(sessionBenchPress.ExerciseType).IsEqualTo(ExerciseType.RepsAndWeight);
        await Assert.That(sessionBenchPress.TargetSets).IsEqualTo(3);
        await Assert.That(sessionBenchPress.TargetReps).IsEqualTo(10);
        await Assert.That(sessionBenchPress.TargetWeight).IsEqualTo(60.0m);
        await Assert.That(sessionBenchPress.IsCompleted).IsFalse();

        var sessionPlank = session.Exercises.First(e => e.ExerciseName == "Plank");
        await Assert.That(sessionPlank.ExerciseType).IsEqualTo(ExerciseType.TimeBased);
        await Assert.That(sessionPlank.TargetDurationSeconds).IsEqualTo(60);

        // ========================================
        // Step 6: Complete all exercises in the session
        // ========================================

        // Complete Bench Press (RepsAndWeight exercise)
        var recordBenchPressResult = session.CompleteExercise(
            exerciseId: benchPress.Id,
            actualSets: 3,
            timeProvider: timeProvider,
            actualReps: 10,
            actualWeight: 62.5m); // Client lifted slightly more than target

        await Assert.That(recordBenchPressResult.IsError).IsFalse();
        await Assert.That(session.GetCompletedExerciseCount()).IsEqualTo(1);

        // Verify the recorded values
        var completedBenchPress = session.Exercises.First(e => e.ExerciseName == "Bench Press");
        await Assert.That(completedBenchPress.IsCompleted).IsTrue();
        await Assert.That(completedBenchPress.ActualSets).IsEqualTo(3);
        await Assert.That(completedBenchPress.ActualReps).IsEqualTo(10);
        await Assert.That(completedBenchPress.ActualWeight).IsEqualTo(62.5m);
        await Assert.That(completedBenchPress.CompletedAt).IsNotNull();

        // Complete Barbell Squat (RepsAndWeight exercise)
        var recordSquatResult = session.CompleteExercise(
            exerciseId: squat.Id,
            actualSets: 4,
            timeProvider: timeProvider,
            actualReps: 8,
            actualWeight: 80.0m);

        await Assert.That(recordSquatResult.IsError).IsFalse();
        await Assert.That(session.GetCompletedExerciseCount()).IsEqualTo(2);

        // Complete Plank (TimeBased exercise)
        var recordPlankResult = session.CompleteExercise(
            exerciseId: plank.Id,
            actualSets: 3,
            timeProvider: timeProvider,
            actualDurationSeconds: 65); // Client held longer than target

        await Assert.That(recordPlankResult.IsError).IsFalse();
        await Assert.That(session.GetCompletedExerciseCount()).IsEqualTo(3);

        // Verify the time-based exercise recorded correctly
        var completedPlank = session.Exercises.First(e => e.ExerciseName == "Plank");
        await Assert.That(completedPlank.IsCompleted).IsTrue();
        await Assert.That(completedPlank.ActualSets).IsEqualTo(3);
        await Assert.That(completedPlank.ActualDurationSeconds).IsEqualTo(65);
        await Assert.That(completedPlank.ActualReps).IsNull(); // Time-based exercises don't have reps
        await Assert.That(completedPlank.ActualWeight).IsNull(); // Time-based exercises don't have weight

        // All exercises should now be completed
        await Assert.That(session.AreAllExercisesCompleted()).IsTrue();
        await Assert.That(session.GetTotalExerciseCount()).IsEqualTo(3);

        // ========================================
        // Step 7: Complete the entire session
        // ========================================
        var completeSessionResult = session.Complete(timeProvider);

        await Assert.That(completeSessionResult.IsError).IsFalse();
        await Assert.That(session.Status).IsEqualTo(SessionStatus.Completed);
        await Assert.That(session.CompletedAt).IsNotNull();

        // Verify the session cannot be completed again
        var doubleCompleteResult = session.Complete(timeProvider);
        await Assert.That(doubleCompleteResult.IsError).IsTrue();

        // Verify the session cannot be abandoned after completion
        var abandonAfterCompleteResult = session.Abandon(timeProvider);
        await Assert.That(abandonAfterCompleteResult.IsError).IsTrue();

        // Verify exercises cannot be recorded after session completion
        var recordAfterCompleteResult = session.CompleteExercise(
            exerciseId: benchPress.Id,
            actualSets: 1,
            timeProvider: timeProvider,
            actualReps: 5);
        await Assert.That(recordAfterCompleteResult.IsError).IsTrue();
    }
}
