using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Social.Comments.Domain;

namespace Social.Comments.Infrastructure.Persistence.Configurations;

public sealed class KnownUserConfiguration 
    : IEntityTypeConfiguration<KnownUser>
{
    public void Configure(EntityTypeBuilder<KnownUser> builder)
    {
        builder.ToTable("KnownUsers");
        
        builder.HasKey(x => x.UserId);
        
        builder.Property(x => x.Username)
            .IsRequired();
    }
}