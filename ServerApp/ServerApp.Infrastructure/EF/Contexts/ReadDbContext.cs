namespace ServerApp.Infrastructure.EF.Contexts;

using Microsoft.EntityFrameworkCore;
using ServerApp.Domain.Entities;

internal sealed class ReadDbContext : DbContext
{
    public DbSet<Painting> Paintings { get; set; } = default!;
    public DbSet<PaintingCategory> PaintingCategories { get; set; } = default!;
    public DbSet<PageContent> PageContents { get; set; } = default!;

    public ReadDbContext(DbContextOptions<ReadDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations from this assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ReadDbContext).Assembly);
    }
}