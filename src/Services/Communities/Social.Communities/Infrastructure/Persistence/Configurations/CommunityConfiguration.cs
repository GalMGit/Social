using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Social.Communities.Domain;

namespace Social.Communities.Infrastructure.Persistence.Configurations;

public sealed class CommunityConfiguration
    : IEntityTypeConfiguration<Community>
{
    public void Configure(EntityTypeBuilder<Community> builder)
    {
        builder.ToTable("Communities");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.OwnerId)
            .IsRequired();

        builder.Property(c => c.Description)
            .HasMaxLength(1000);

        builder.HasMany(c => c.Members)
            .WithOne(m => m.Community)
            .HasForeignKey(m => m.CommunityId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(c => c.OwnerId);
    }
}