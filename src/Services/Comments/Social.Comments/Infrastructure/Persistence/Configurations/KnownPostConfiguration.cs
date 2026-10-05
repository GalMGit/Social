using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Social.Comments.Domain;

namespace Social.Comments.Infrastructure.Persistence.Configurations;

public sealed class KnownPostConfiguration 
    : IEntityTypeConfiguration<KnownPost>
{
    public void Configure(EntityTypeBuilder<KnownPost> builder)
    {
        builder.ToTable("KnownPosts");
        
        builder.HasKey(x => x.PostId);
        
        builder.Property(x => x.CreatedAt)
            .IsRequired();
    }
}