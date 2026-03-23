namespace ServerApp.Infrastructure.EF.Contexts;

using Microsoft.EntityFrameworkCore;
using ServerApp.Infrastructure.EF.Config;
using ServerApp.Infrastructure.EF.Models;

internal sealed class ReadDbContext : DbContext
{
    public DbSet<PaintingReadModel> Paintings { get; set; } = default!;
    public DbSet<PaintingCategoryReadModel> PaintingCategories { get; set; } = default!;
    public DbSet<PageContentReadModel> PageContents { get; set; } = default!;

    public ReadDbContext(DbContextOptions<ReadDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var paintingConfig = new PaintingReadModelConfiguration();
        modelBuilder.ApplyConfiguration(paintingConfig);

        var paintingCategoryConfig = new PaintingCategoryReadModelConfiguration();
        modelBuilder.ApplyConfiguration(paintingCategoryConfig);

        var pageContentConfig = new PageContentReadModelConfiguration();
        modelBuilder.ApplyConfiguration(pageContentConfig);
    }
}