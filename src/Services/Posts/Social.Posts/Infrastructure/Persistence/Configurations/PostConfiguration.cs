using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Social.Posts.Domain;

namespace Social.Posts.Infrastructure.Persistence.Configurations;

public sealed class PostConfiguration
    : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> builder)
    {
        builder.ToTable("Posts");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.AuthorId)
            .IsRequired();

        builder.Property(x => x.CommunityId);

        builder.HasIndex(x => x.AuthorId);

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Content)
            .IsRequired()
            .HasMaxLength(10000);

        builder.Property(x => x.ImagePath)
            .HasMaxLength(2048);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt);

        builder.Property(x => x.DeletedAt);
    }
}