using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Social.Posts.Domain;

namespace Social.Posts.Infrastructure.Persistence.Context;

public sealed class PostsDbContext : DbContext
{
    public PostsDbContext(DbContextOptions<PostsDbContext> options)
        : base(options) { }

    public DbSet<Post> Posts => Set<Post>();
    public DbSet<KnownUser> KnownUsers => Set<KnownUser>();
    public DbSet<KnownCommunity> KnownCommunities => Set<KnownCommunity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }
}