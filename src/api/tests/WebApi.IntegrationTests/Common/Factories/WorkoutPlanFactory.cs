using GymBuddy.Domain.Common;
using GymBuddy.Domain.Exercises;
using GymBuddy.Domain.Users;
using GymBuddy.Domain.WorkoutPlans;

#pragma warning disable CA1707

namespace GymBuddy.Api.IntegrationTests.Common.Factories;

public static class WorkoutPlanFactory
{
    private static readonly Faker Faker = new();

    public static WorkoutPlan Generate(UserId trainerId)
    {
        var workoutPlan = WorkoutPlan.Create(
            Faker.Lorem.Word() + " Workout",
            trainerId,
            Faker.Lorem.Sentence()
        );

        return workoutPlan;
    }

    public static IReadOnlyList<WorkoutPlan> Generate(UserId trainerId, int count)
    {
        var plans = new List<WorkoutPlan>();
        for (var i = 0; i < count; i++)
        {
            plans.Add(Generate(trainerId));
        }
        return plans;
    }

    public static WorkoutPlan GenerateWithExercises(UserId trainerId, IReadOnlyList<Exercise> exercises)
    {
        var workoutPlan = Generate(trainerId);

        foreach (var exercise in exercises)
        {
            if (exercise.Type == ExerciseType.RepsAndWeight)
            {
                workoutPlan.AddExercise(
                    exercise,
                    sets: Faker.Random.Number(2, 5),
                    reps: Faker.Random.Number(8, 15),
                    weight: new Weight(Faker.Random.Decimal(10, 100))
                );
            }
            else
            {
                workoutPlan.AddExercise(
                    exercise,
                    sets: Faker.Random.Number(2, 5),
                    duration: Duration.FromSeconds(Faker.Random.Number(30, 120))
                );
            }
        }

        return workoutPlan;
    }
}
