using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ServerApp.Domain.Entities;
using ServerApp.Domain.Factories;
using ServerApp.Domain.ValueObjects.Page;
using ServerApp.Domain.ValueObjects.Painting;
using ServerApp.Domain.ValueObjects.PaintingCategory;
using ServerApp.Infrastructure.EF.Contexts;
using ServerApp.Infrastructure.SeedData;
using ServerApp.Infrastructure.SeedData.SiteSpecific;

namespace ServerApp.Infrastructure.Services;

/// <summary>
/// Seeds the database with initial data after migrations are applied.
/// Uses site-specific seed data providers resolved via the SITE_NAME environment variable.
/// </summary>
internal sealed class DatabaseSeeder
{
    private readonly ILogger<DatabaseSeeder> _logger;
    private readonly ISiteSeedDataProvider _seedDataProvider;
    private readonly IPaintingCategoryFactory _categoryFactory;
    private readonly IPaintingFactory _paintingFactory;
    private readonly IPageContentFactory _pageContentFactory;
    private readonly WriteDbContext _writeDbContext;
    private readonly ReadDbContext _readDbContext;

    public DatabaseSeeder(
        ILogger<DatabaseSeeder> logger,
        IConfiguration configuration,
        IPaintingCategoryFactory categoryFactory,
        IPaintingFactory paintingFactory,
        IPageContentFactory pageContentFactory,
        WriteDbContext writeDbContext,
        ReadDbContext readDbContext)
    {
        _logger = logger;
        var siteName = configuration["SITE_NAME"];
        _seedDataProvider = SiteSeedDataProviderFactory.GetProvider(siteName);
        _categoryFactory = categoryFactory;
        _paintingFactory = paintingFactory;
        _pageContentFactory = pageContentFactory;
        _writeDbContext = writeDbContext;
        _readDbContext = readDbContext;
    }

    /// <summary>
    /// Seeds both Write and Read databases with initial data.
    /// </summary>
    public async Task SeedAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting database seeding...");

        // Seed Write database if empty
        await SeedDatabaseAsync(_writeDbContext, "Write", cancellationToken);

        // Seed Read database if empty
        await SeedDatabaseAsync(_readDbContext, "Read", cancellationToken);

        _logger.LogInformation("Database seeding completed successfully.");
    }

    private async Task SeedDatabaseAsync(DbContext context, string databaseName, CancellationToken cancellationToken)
    {
        // Check if data already exists to avoid duplicate seeding
        var hasCategories = await context.Set<PaintingCategory>().AnyAsync(cancellationToken);
        var hasPaintings = await context.Set<Painting>().AnyAsync(cancellationToken);
        var hasPageContent = await context.Set<PageContent>().AnyAsync(cancellationToken);

        if (hasCategories || hasPaintings || hasPageContent)
        {
            _logger.LogInformation($"{databaseName} database already contains data. Skipping seeding.");
            return;
        }

        _logger.LogInformation("Database is empty. Seeding initial data...");

        // Seed painting categories from site-specific seed data provider
        var categories = new Dictionary<string, PaintingCategory>();
        foreach (var seedCategory in _seedDataProvider.Categories)
        {
            var category = await CreateCategoryAsync(
                seedCategory.Name,
                seedCategory.Description,
                cancellationToken
            );
            categories[seedCategory.Slug] = category;
            context.Set<PaintingCategory>().Add(category);
        }

        await context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation($"Seeded {categories.Count} painting categories.");

        // Seed paintings from site-specific seed data provider
        var paintings = new List<Painting>();
        foreach (var seedPainting in _seedDataProvider.Paintings)
        {
            if (categories.TryGetValue(seedPainting.CategorySlug, out var category))
            {
                var painting = CreatePaintingFromSeed(seedPainting, category);
                paintings.Add(painting);
                context.Set<Painting>().Add(painting);
            }
            else
            {
                _logger.LogWarning($"Category '{seedPainting.CategorySlug}' not found for painting '{seedPainting.Title}'. Skipping.");
            }
        }

        await context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation($"Seeded {paintings.Count} paintings.");

        // Seed page content from site-specific seed data provider
        var pageContents = new List<PageContent>();
        foreach (var seedPageContent in _seedDataProvider.PageContents)
        {
            var pageContent = _pageContentFactory.Create(
                new PageAddress(seedPageContent.Address),
                PageTitle.FromNullable(seedPageContent.Title),
                new PageContentText(seedPageContent.Content),
                PagePhotoUrls.FromNullable(seedPageContent.PhotoUrls)
            );
            pageContents.Add(pageContent);
            context.Set<PageContent>().Add(pageContent);
        }

        await context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation($"Seeded {pageContents.Count} page content entries.");
    }

    private async Task<PaintingCategory> CreateCategoryAsync(string name, string? description, CancellationToken cancellationToken)
    {
        return await _categoryFactory.CreateAsync(
            name,
            description,
            cancellationToken
        );
    }

    private Painting CreatePaintingFromSeed(PaintingSeed seed, PaintingCategory category)
    {
        return _paintingFactory.Create(
            new PaintingName(seed.Title),
            new PaintingDescription(seed.Description ?? string.Empty),
            new PaintingImageUrl(seed.ImageUrl),
            thumbnailUrl: PaintingThumbnailUrl.FromNullable(seed.ThumbnailUrl),
            new PaintingCategorySlug(seed.CategorySlug),
            price: seed.Price,
            width: seed.Width.HasValue ? new PaintingWidth(seed.Width.Value) : null,
            height: seed.Height.HasValue ? new PaintingHeight(seed.Height.Value) : null,
            depth: seed.Depth.HasValue ? new PaintingDepth(seed.Depth.Value) : null,
            year: seed.Year.HasValue ? new PaintingYear(seed.Year.Value) : null,
            isAvailable: new PaintingIsAvailable(seed.IsAvailable),
            isNew: new PaintingIsNew(seed.IsNew),
            isLandscape: new PaintingIsLandscape(seed.IsLandscape)
        );
    }
}