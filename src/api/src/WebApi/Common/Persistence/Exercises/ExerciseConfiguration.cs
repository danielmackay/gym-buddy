using System.Text.Json;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GymBuddy.Domain.Exercises;

namespace GymBuddy.Api.Common.Persistence.Exercises;

public class ExerciseConfiguration : AuditableConfiguration<Exercise>
{
    public override void PostConfigure(EntityTypeBuilder<Exercise> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name)
            .HasMaxLength(Exercise.NameMaxLength)
            .IsRequired();

        builder.Property(e => e.Description)
            .HasMaxLength(Exercise.DescriptionMaxLength);

        builder.Property(e => e.Type)
            .IsRequired();

        // Store muscle groups as JSON array using value converter
        builder.Property(e => e.MuscleGroups)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<MuscleGroup>>(v, (JsonSerializerOptions?)null) ?? new List<MuscleGroup>())
            .Metadata.SetValueComparer(new ValueComparer<IReadOnlyList<MuscleGroup>>(
                (c1, c2) => c1!.SequenceEqual(c2!),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToList()));
    }
}
