using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Social.Posts.Domain;

namespace Social.Posts.Infrastructure.Persistence.Configurations;

public sealed class KnownCommunityConfiguration 
    : IEntityTypeConfiguration<KnownCommunity>
{
    public void Configure(EntityTypeBuilder<KnownCommunity> builder)
    {
        builder.ToTable("KnownCommunities");
        
        builder.HasKey(x => x.CommunityId);
        
        builder.HasIndex(x => x.CommunityId)
            .IsUnique();
        
        builder.Property(x => x.Name)
            .IsRequired();
    }
}