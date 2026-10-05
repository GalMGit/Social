using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Social.Comments.Domain;

namespace Social.Comments.Infrastructure.Persistence.Configurations;

public sealed class CommentConfiguration
    : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.ToTable("Comments");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Text)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(x => x.PostId)
            .IsRequired();

        builder.Property(x => x.AuthorId)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt);

        builder.HasIndex(x => new
        {
            x.PostId,
            x.CreatedAt
        });

        builder.HasIndex(x => x.AuthorId);
    }
}