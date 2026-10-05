using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace Social.Comments.Infrastructure.Persistence.Context;

public sealed class CommentsDbContext : DbContext
{
    public CommentsDbContext(DbContextOptions<CommentsDbContext> options)
        : base(options) {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            Assembly.GetExecutingAssembly());
        
        base.OnModelCreating(modelBuilder);
    }
}