using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GymBuddy.Domain.WorkoutSessions;

namespace GymBuddy.Api.Common.Persistence.WorkoutSessions;

public class WorkoutSessionConfiguration : AuditableConfiguration<WorkoutSession>
{
    public override void PostConfigure(EntityTypeBuilder<WorkoutSession> builder)
    {
        builder.HasKey(ws => ws.Id);

        builder.Property(ws => ws.ClientId)
            .IsRequired();

        // Foreign key to Users table
        builder.HasOne<GymBuddy.Domain.Users.User>()
            .WithMany()
            .HasForeignKey(ws => ws.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(ws => ws.WorkoutPlanId)
            .IsRequired();

        // Foreign key to WorkoutPlans table
        builder.HasOne<GymBuddy.Domain.WorkoutPlans.WorkoutPlan>()
            .WithMany()
            .HasForeignKey(ws => ws.WorkoutPlanId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(ws => ws.WorkoutPlanName)
            .HasMaxLength(WorkoutSession.WorkoutPlanNameMaxLength)
            .IsRequired();

        builder.Property(ws => ws.StartedAt)
            .IsRequired();

        builder.Property(ws => ws.Status)
            .IsRequired();

        // SessionExercise is now an entity with its own table
        builder.HasMany(ws => ws.Exercises)
            .WithOne()
            .HasForeignKey("WorkoutSessionId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(ws => ws.Exercises)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
