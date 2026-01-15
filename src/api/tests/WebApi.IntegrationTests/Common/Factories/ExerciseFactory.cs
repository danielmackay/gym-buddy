using GymBuddy.Domain.Exercises;

#pragma warning disable CA1707

namespace GymBuddy.Api.IntegrationTests.Common.Factories;

public static class ExerciseFactory
{
    private static readonly MuscleGroup[] AllMuscleGroups =
    [
        MuscleGroup.Chest,
        MuscleGroup.Back,
        MuscleGroup.Shoulders,
        MuscleGroup.Biceps,
        MuscleGroup.Triceps,
        MuscleGroup.Forearms,
        MuscleGroup.Quadriceps,
        MuscleGroup.Hamstrings,
        MuscleGroup.Glutes,
        MuscleGroup.Calves,
        MuscleGroup.Abs,
        MuscleGroup.Obliques
    ];

    private static readonly Faker<Exercise> RepsAndWeightExerciseFaker = new Faker<Exercise>()
        .CustomInstantiator(f =>
        {
            var muscleGroups = f.PickRandom(AllMuscleGroups, f.Random.Number(1, 3));
            var result = Exercise.Create(
                f.Lorem.Word() + " " + f.Lorem.Word(),
                ExerciseType.RepsAndWeight,
                muscleGroups,
                f.Lorem.Sentence()
            );
            return result.Value;
        });

    private static readonly Faker<Exercise> TimeBasedExerciseFaker = new Faker<Exercise>()
        .CustomInstantiator(f =>
        {
            var muscleGroups = f.PickRandom(AllMuscleGroups, f.Random.Number(1, 3));
            var result = Exercise.Create(
                f.Lorem.Word() + " " + f.Lorem.Word(),
                ExerciseType.TimeBased,
                muscleGroups,
                f.Lorem.Sentence()
            );
            return result.Value;
        });

    public static Exercise Generate() => RepsAndWeightExerciseFaker.Generate();

    public static IReadOnlyList<Exercise> Generate(int count) => RepsAndWeightExerciseFaker.Generate(count);

    public static Exercise GenerateRepsAndWeight() => RepsAndWeightExerciseFaker.Generate();

    public static Exercise GenerateTimeBased() => TimeBasedExerciseFaker.Generate();

    public static IReadOnlyList<Exercise> GenerateRepsAndWeight(int count) =>
        RepsAndWeightExerciseFaker.Generate(count);

    public static IReadOnlyList<Exercise> GenerateTimeBased(int count) =>
        TimeBasedExerciseFaker.Generate(count);
}
