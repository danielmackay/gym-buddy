using System.Text.Json;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GymBuddy.Domain.Users;
using GymBuddy.Domain.WorkoutPlans;

namespace GymBuddy.Api.Common.Persistence.Users;

public class UserConfiguration : AuditableConfiguration<User>
{
    public override void PostConfigure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Name)
            .HasMaxLength(User.NameMaxLength)
            .IsRequired();

        builder.Property(u => u.Email)
            .HasMaxLength(User.EmailMaxLength)
            .IsRequired();

        builder.HasIndex(u => u.Email)
            .IsUnique();

        // Self-referencing FK for Trainer-Client relationship
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(u => u.TrainerId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        // Store roles as JSON array using value converter
        builder.Property(u => u.Roles)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<UserRole>>(v, (JsonSerializerOptions?)null) ?? new List<UserRole>())
            .Metadata.SetValueComparer(new ValueComparer<IReadOnlyList<UserRole>>(
                (c1, c2) => c1!.SequenceEqual(c2!),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToList()));

        // Store assigned workout plan IDs as JSON array
        builder.Property(u => u.AssignedWorkoutPlanIds)
            .HasConversion(
                v => JsonSerializer.Serialize(v.Select(id => id.Value).ToList(), (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<Guid>>(v, (JsonSerializerOptions?)null)!
                    .Select(WorkoutPlanId.From).ToList() as IReadOnlyList<WorkoutPlanId>
                    ?? new List<WorkoutPlanId>())
            .Metadata.SetValueComparer(new ValueComparer<IReadOnlyList<WorkoutPlanId>>(
                (c1, c2) => c1!.SequenceEqual(c2!),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToList()));
    }
}
