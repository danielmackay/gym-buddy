using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GymBuddy.Api.Common.Domain.Teams;
using GymBuddy.Api.Common.Persistence.Heroes;

namespace GymBuddy.Api.Common.Persistence.Teams;

public class MissionConfiguration : AuditableConfiguration<Mission>
{
    public override void PostConfigure(EntityTypeBuilder<Mission> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Description)
            .HasMaxLength(Mission.DescriptionMaxLength)
            .IsRequired();
    }
}