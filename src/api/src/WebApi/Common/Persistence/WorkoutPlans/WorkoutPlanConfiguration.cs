using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GymBuddy.Domain.WorkoutPlans;

namespace GymBuddy.Api.Common.Persistence.WorkoutPlans;

public class WorkoutPlanConfiguration : AuditableConfiguration<WorkoutPlan>
{
    public override void PostConfigure(EntityTypeBuilder<WorkoutPlan> builder)
    {
        builder.HasKey(wp => wp.Id);

        builder.Property(wp => wp.Name)
            .HasMaxLength(WorkoutPlan.NameMaxLength)
            .IsRequired();

        builder.Property(wp => wp.Description)
            .HasMaxLength(WorkoutPlan.DescriptionMaxLength);

        builder.Property(wp => wp.TrainerId)
            .IsRequired();

        // PlannedExercise is now an entity with its own table
        builder.HasMany(wp => wp.Exercises)
            .WithOne()
            .HasForeignKey("WorkoutPlanId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(wp => wp.Exercises)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
