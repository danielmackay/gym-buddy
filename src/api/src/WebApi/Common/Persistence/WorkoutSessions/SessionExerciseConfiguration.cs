using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GymBuddy.Domain.Common;
using GymBuddy.Domain.WorkoutSessions;

namespace GymBuddy.Api.Common.Persistence.WorkoutSessions;

public class SessionExerciseConfiguration : AuditableConfiguration<SessionExercise>
{
    public override void PostConfigure(EntityTypeBuilder<SessionExercise> builder)
    {
        builder.ToTable("SessionExercises");

        builder.HasKey(se => se.Id);

        builder.Property(se => se.ExerciseId)
            .IsRequired();

        // Foreign key to Exercises table
        builder.HasOne<GymBuddy.Domain.Exercises.Exercise>()
            .WithMany()
            .HasForeignKey(se => se.ExerciseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(se => se.ExerciseName)
            .HasMaxLength(SessionExercise.ExerciseNameMaxLength)
            .IsRequired();

        builder.Property(se => se.ExerciseType)
            .IsRequired();

        builder.Property(se => se.TargetSets)
            .IsRequired();

        builder.ComplexProperty(se => se.TargetWeight, w =>
        {
            w.Property(x => x.Value)
                .HasPrecision(8, 2)
                .HasColumnName("TargetWeight");
            w.Property(x => x.Unit)
                .HasColumnName("TargetWeightUnit")
                .HasDefaultValue(WeightUnit.Kilograms);
        });

        builder.ComplexProperty(se => se.ActualWeight, w =>
        {
            w.Property(x => x.Value)
                .HasPrecision(8, 2)
                .HasColumnName("ActualWeight");
            w.Property(x => x.Unit)
                .HasColumnName("ActualWeightUnit")
                .HasDefaultValue(WeightUnit.Kilograms);
        });

        builder.ComplexProperty(se => se.TargetDuration, d =>
        {
            d.Property(x => x.Seconds)
                .HasColumnName("TargetDuration");
        });

        builder.ComplexProperty(se => se.ActualDuration, d =>
        {
            d.Property(x => x.Seconds)
                .HasColumnName("ActualDuration");
        });

        builder.Property(se => se.Order)
            .IsRequired();

        // Shadow property for FK - configured in WorkoutSessionConfiguration
        builder.Property<WorkoutSessionId>("WorkoutSessionId")
            .IsRequired();

        builder.HasIndex("WorkoutSessionId");
    }
}
