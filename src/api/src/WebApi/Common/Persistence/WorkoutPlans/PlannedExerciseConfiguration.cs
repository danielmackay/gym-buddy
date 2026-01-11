using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GymBuddy.Domain.WorkoutPlans;

namespace GymBuddy.Api.Common.Persistence.WorkoutPlans;

public class PlannedExerciseConfiguration : AuditableConfiguration<PlannedExercise>
{
    public override void PostConfigure(EntityTypeBuilder<PlannedExercise> builder)
    {
        builder.ToTable("PlannedExercises");

        builder.HasKey(pe => pe.Id);

        builder.Property(pe => pe.ExerciseId)
            .IsRequired();

        builder.Property(pe => pe.ExerciseName)
            .HasMaxLength(PlannedExercise.ExerciseNameMaxLength)
            .IsRequired();

        builder.Property(pe => pe.ExerciseType)
            .IsRequired();

        builder.Property(pe => pe.Sets)
            .IsRequired();

        builder.Property(pe => pe.Weight)
            .HasPrecision(8, 2);

        builder.Property(pe => pe.Order)
            .IsRequired();

        // Shadow property for FK - configured in WorkoutPlanConfiguration
        builder.Property<WorkoutPlanId>("WorkoutPlanId")
            .IsRequired();

        builder.HasIndex("WorkoutPlanId");
    }
}
