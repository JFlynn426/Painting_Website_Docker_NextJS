namespace ServerApp.Infrastructure.EF.Contexts;

using Microsoft.EntityFrameworkCore;
using ServerApp.Infrastructure.EF.Config;
using ServerApp.Infrastructure.EF.Models;

internal sealed class WriteDbContext : DbContext
{
    public DbSet<PaintingWriteModel> Paintings { get; set; } = default!;
    public DbSet<PaintingCategoryWriteModel> PaintingCategories { get; set; } = default!;
    public DbSet<PageContentWriteModel> PageContents { get; set; } = default!;

    public WriteDbContext(DbContextOptions<WriteDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var paintingConfig = new PaintingWriteModelConfiguration();
        modelBuilder.ApplyConfiguration(paintingConfig);

        var paintingCategoryConfig = new PaintingCategoryWriteModelConfiguration();
        modelBuilder.ApplyConfiguration(paintingCategoryConfig);

        var pageContentConfig = new PageContentWriteModelConfiguration();
        modelBuilder.ApplyConfiguration(pageContentConfig);
    }
}