using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Social.Communities.Domain;

namespace Social.Communities.Infrastructure.Persistence.Context;

public sealed class CommunitiesDbContext : DbContext
{
    public CommunitiesDbContext(DbContextOptions<CommunitiesDbContext> options)
        : base(options){}

    public DbSet<Community> Communities => Set<Community>();
    public DbSet<CommunityMember> Members => Set<CommunityMember>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }
}
