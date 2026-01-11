using Microsoft.EntityFrameworkCore.Metadata.Builders;
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

        builder.Property(se => se.ExerciseName)
            .HasMaxLength(SessionExercise.ExerciseNameMaxLength)
            .IsRequired();

        builder.Property(se => se.ExerciseType)
            .IsRequired();

        builder.Property(se => se.TargetSets)
            .IsRequired();

        builder.Property(se => se.TargetWeight)
            .HasPrecision(8, 2);

        builder.Property(se => se.ActualWeight)
            .HasPrecision(8, 2);

        builder.Property(se => se.Order)
            .IsRequired();

        // Shadow property for FK - configured in WorkoutSessionConfiguration
        builder.Property<WorkoutSessionId>("WorkoutSessionId")
            .IsRequired();

        builder.HasIndex("WorkoutSessionId");
    }
}
