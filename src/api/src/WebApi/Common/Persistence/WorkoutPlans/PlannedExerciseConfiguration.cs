using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GymBuddy.Domain.Common;
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

        // Foreign key to Exercises table
        builder.HasOne<GymBuddy.Domain.Exercises.Exercise>()
            .WithMany()
            .HasForeignKey(pe => pe.ExerciseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(pe => pe.ExerciseName)
            .HasMaxLength(PlannedExercise.ExerciseNameMaxLength)
            .IsRequired();

        builder.Property(pe => pe.ExerciseType)
            .IsRequired();

        builder.Property(pe => pe.Sets)
            .IsRequired();

        builder.ComplexProperty(pe => pe.Weight, w =>
        {
            w.Property(x => x.Value)
                .HasPrecision(8, 2)
                .HasColumnName("Weight");
            w.Property(x => x.Unit)
                .HasColumnName("WeightUnit")
                .HasDefaultValue(WeightUnit.Kilograms);
        });

        builder.ComplexProperty(pe => pe.Duration, d =>
        {
            d.Property(x => x.Seconds)
                .HasColumnName("Duration");
        });

        builder.Property(pe => pe.Order)
            .IsRequired();

        // Shadow property for FK - configured in WorkoutPlanConfiguration
        builder.Property<WorkoutPlanId>("WorkoutPlanId")
            .IsRequired();

        builder.HasIndex("WorkoutPlanId");
    }
}
