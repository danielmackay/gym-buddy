using Microsoft.EntityFrameworkCore;
using GymBuddy.Domain.Users;
using GymBuddy.Domain.Exercises;
using GymBuddy.Api.Common.Persistence;

namespace MigrationService.Initializers;

public class ApplicationDbContextInitializer(ApplicationDbContext dbContext) : DbContextInitializerBase<ApplicationDbContext>(dbContext)
{
    private readonly (string Name, ExerciseType Type, string Description, MuscleGroup[] MuscleGroups)[] _exercises =
    [
        // Chest
        ("Bench Press", ExerciseType.RepsAndWeight, "Horizontal pressing exercise for chest development", [MuscleGroup.Chest, MuscleGroup.Triceps, MuscleGroup.Shoulders]),
        ("Flyes", ExerciseType.RepsAndWeight, "Isolation exercise for chest with arms in arc motion", [MuscleGroup.Chest]),
        ("Push-ups", ExerciseType.RepsAndWeight, "Bodyweight pressing exercise for chest", [MuscleGroup.Chest, MuscleGroup.Triceps, MuscleGroup.Shoulders]),

        // Back
        ("Rows", ExerciseType.RepsAndWeight, "Horizontal pulling exercise for back thickness", [MuscleGroup.Back, MuscleGroup.Biceps]),
        ("Lat Pulldown", ExerciseType.RepsAndWeight, "Vertical pulling exercise for back width", [MuscleGroup.Back, MuscleGroup.Biceps]),
        ("Pull-ups", ExerciseType.RepsAndWeight, "Bodyweight vertical pulling exercise", [MuscleGroup.Back, MuscleGroup.Biceps]),
        ("Deadlift", ExerciseType.RepsAndWeight, "Compound exercise for entire posterior chain", [MuscleGroup.Back, MuscleGroup.Glutes, MuscleGroup.Hamstrings]),

        // Shoulders
        ("Overhead Press", ExerciseType.RepsAndWeight, "Vertical pressing exercise for shoulders", [MuscleGroup.Shoulders, MuscleGroup.Triceps]),
        ("Lateral Raise", ExerciseType.RepsAndWeight, "Isolation exercise for side deltoids", [MuscleGroup.Shoulders]),
        ("Front Raise", ExerciseType.RepsAndWeight, "Isolation exercise for front deltoids", [MuscleGroup.Shoulders]),
        ("Rear Delt Flye", ExerciseType.RepsAndWeight, "Isolation exercise for rear deltoids", [MuscleGroup.Shoulders]),
        ("Shrugs", ExerciseType.RepsAndWeight, "Isolation exercise for trapezius", [MuscleGroup.Shoulders]),

        // Biceps
        ("Bicep Curl", ExerciseType.RepsAndWeight, "Isolation exercise for biceps", [MuscleGroup.Biceps]),
        ("Hammer Curl", ExerciseType.RepsAndWeight, "Curl variation with neutral grip", [MuscleGroup.Biceps, MuscleGroup.Forearms]),

        // Triceps
        ("Tricep Extension", ExerciseType.RepsAndWeight, "Isolation exercise for triceps", [MuscleGroup.Triceps]),
        ("Tricep Dip", ExerciseType.RepsAndWeight, "Compound bodyweight exercise for triceps", [MuscleGroup.Triceps, MuscleGroup.Chest, MuscleGroup.Shoulders]),
        ("Tricep Pushdown", ExerciseType.RepsAndWeight, "Cable isolation exercise for triceps", [MuscleGroup.Triceps]),

        // Quadriceps
        ("Squat", ExerciseType.RepsAndWeight, "Compound lower body exercise", [MuscleGroup.Quadriceps, MuscleGroup.Glutes, MuscleGroup.Hamstrings]),
        ("Leg Press", ExerciseType.RepsAndWeight, "Machine-based compound lower body exercise", [MuscleGroup.Quadriceps, MuscleGroup.Glutes]),
        ("Lunge", ExerciseType.RepsAndWeight, "Unilateral lower body exercise", [MuscleGroup.Quadriceps, MuscleGroup.Glutes]),
        ("Leg Extension", ExerciseType.RepsAndWeight, "Isolation exercise for quadriceps", [MuscleGroup.Quadriceps]),

        // Hamstrings
        ("Leg Curl", ExerciseType.RepsAndWeight, "Isolation exercise for hamstrings", [MuscleGroup.Hamstrings]),
        ("Romanian Deadlift", ExerciseType.RepsAndWeight, "Hip hinge exercise for hamstrings", [MuscleGroup.Hamstrings, MuscleGroup.Glutes, MuscleGroup.Back]),

        // Glutes
        ("Hip Thrust", ExerciseType.RepsAndWeight, "Isolation exercise for glutes", [MuscleGroup.Glutes]),
        ("Glute Bridge", ExerciseType.RepsAndWeight, "Bodyweight exercise for glutes", [MuscleGroup.Glutes]),

        // Calves
        ("Calf Raise", ExerciseType.RepsAndWeight, "Isolation exercise for calves", [MuscleGroup.Calves]),

        // Abs
        ("Plank", ExerciseType.TimeBased, "Isometric core stability exercise", [MuscleGroup.Abs]),
        ("Side Plank", ExerciseType.TimeBased, "Isometric exercise for obliques and core", [MuscleGroup.Abs, MuscleGroup.Obliques]),
        ("Crunch", ExerciseType.RepsAndWeight, "Isolation exercise for abs", [MuscleGroup.Abs]),
        ("Leg Raise", ExerciseType.RepsAndWeight, "Lower ab exercise", [MuscleGroup.Abs]),
        ("Mountain Climbers", ExerciseType.RepsAndWeight, "Dynamic core and cardio exercise", [MuscleGroup.Abs, MuscleGroup.Shoulders]),

        // Obliques
        ("Russian Twist", ExerciseType.RepsAndWeight, "Rotational core exercise", [MuscleGroup.Obliques, MuscleGroup.Abs]),
        ("Side Bend", ExerciseType.RepsAndWeight, "Lateral flexion exercise for obliques", [MuscleGroup.Obliques])
    ];

    public async Task SeedDataAsync(CancellationToken cancellationToken)
    {
        var strategy = DbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            // Seed the database
            await using var transaction = await DbContext.Database.BeginTransactionAsync(cancellationToken);
            await SeedAdminUser();
            await SeedExercises();
            await transaction.CommitAsync(cancellationToken);
        });
    }

    private async Task SeedAdminUser()
    {
        // TODO: Efficiently query users by role

        // Check if admin user already exists
        var users = await DbContext.Users.ToListAsync();
        if (users.Any(u => u.Roles.Contains(UserRole.Admin)))
            return;

        var admin = User.Create("Admin User", "admin@gymbuddy.com");
        var addRoleResult = admin.AddRole(UserRole.Admin);

        if (addRoleResult.IsError)
            return;

        await DbContext.Users.AddAsync(admin);
        await DbContext.SaveChangesAsync();
    }

    private async Task SeedExercises()
    {
        if (DbContext.Exercises.Any())
            return;

        var exercises = new List<Exercise>();

        foreach (var (name, type, description, muscleGroups) in _exercises)
        {
            var result = Exercise.Create(name, type, muscleGroups, description);

            if (result.IsError)
                continue; // Skip invalid exercises

            exercises.Add(result.Value);
        }

        await DbContext.Exercises.AddRangeAsync(exercises);
        await DbContext.SaveChangesAsync();
    }
}
