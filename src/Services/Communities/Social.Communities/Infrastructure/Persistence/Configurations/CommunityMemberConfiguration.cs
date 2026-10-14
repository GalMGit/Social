using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Social.Communities.Domain;

namespace Social.Communities.Infrastructure.Persistence.Configurations;

public sealed class CommunityMemberConfiguration
    : IEntityTypeConfiguration<CommunityMember>
{
    public void Configure(EntityTypeBuilder<CommunityMember> builder)
    {
        builder.ToTable("CommunityMembers");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.CommunityId)
            .IsRequired();

        builder.Property(m => m.UserId)
            .IsRequired();

        builder.Property(m => m.Role)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.HasOne(m => m.Community)
            .WithMany(c => c.Members)
            .HasForeignKey(m => m.CommunityId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(m => new { m.CommunityId, m.UserId })
            .IsUnique();

        builder.HasIndex(m => m.UserId);
    }
}