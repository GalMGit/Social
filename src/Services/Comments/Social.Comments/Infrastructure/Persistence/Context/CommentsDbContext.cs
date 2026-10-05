using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Social.Comments.Domain;

namespace Social.Comments.Infrastructure.Persistence.Context;

public sealed class CommentsDbContext : DbContext
{
    public CommentsDbContext(DbContextOptions<CommentsDbContext> options)
        : base(options) {}

    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<KnownPost> KnownPosts => Set<KnownPost>();
    public DbSet<KnownUser> KnownUsers => Set<KnownUser>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            Assembly.GetExecutingAssembly());
        
        base.OnModelCreating(modelBuilder);
    }
}