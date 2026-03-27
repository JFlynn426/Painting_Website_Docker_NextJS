using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ServerApp.Domain.Entities;
using ServerApp.Domain.Factories;
using ServerApp.Domain.ValueObjects.Painting;
using ServerApp.Domain.ValueObjects.PaintingCategory;
using ServerApp.Infrastructure.EF.Contexts;

namespace ServerApp.Infrastructure.Services;

/// <summary>
/// Seeds the database with initial data after migrations are applied.
/// </summary>
internal sealed class DatabaseSeeder
{
    private readonly ILogger<DatabaseSeeder> _logger;
    private readonly IPaintingCategoryFactory _categoryFactory;
    private readonly IPaintingFactory _paintingFactory;
    private readonly WriteDbContext _writeDbContext;
    private readonly ReadDbContext _readDbContext;

    public DatabaseSeeder(
        ILogger<DatabaseSeeder> logger,
        IPaintingCategoryFactory categoryFactory,
        IPaintingFactory paintingFactory,
        WriteDbContext writeDbContext,
        ReadDbContext readDbContext)
    {
        _logger = logger;
        _categoryFactory = categoryFactory;
        _paintingFactory = paintingFactory;
        _writeDbContext = writeDbContext;
        _readDbContext = readDbContext;
    }

    /// <summary>
    /// Seeds both Write and Read databases with initial data.
    /// </summary>
    public async Task SeedAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting database seeding...");

        // Seed Write database
        await SeedDatabaseAsync(_writeDbContext, cancellationToken);

        // Seed Read database
        await SeedDatabaseAsync(_readDbContext, cancellationToken);

        _logger.LogInformation("Database seeding completed successfully.");
    }

    private async Task SeedDatabaseAsync(DbContext context, CancellationToken cancellationToken)
    {
        // Check if data already exists to avoid duplicate seeding
        var hasCategories = await context.Set<PaintingCategory>().AnyAsync(cancellationToken);

        if (hasCategories)
        {
            _logger.LogInformation("Database already contains data. Skipping seeding.");
            return;
        }

        _logger.LogInformation("Database is empty. Seeding initial data...");

        // Seed painting categories
        var categories = new List<PaintingCategory>
        {
            await CreateCategoryAsync("animals", "Animals", "Beautiful animal paintings featuring wildlife and domestic animals", cancellationToken),
            await CreateCategoryAsync("flowers", "Flowers", "Vibrant floral paintings showcasing nature's beauty", cancellationToken),
            await CreateCategoryAsync("seascapes", "Seascapes", "Stunning ocean and coastal scene paintings", cancellationToken)
        };

        foreach (var category in categories)
        {
            context.Set<PaintingCategory>().Add(category);
        }

        await context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation($"Seeded {categories.Count} painting categories.");

        // Seed sample paintings for each category
        var paintings = new List<Painting>();

        // Animals paintings
        paintings.AddRange(CreateSamplePaintings(
            "animals",
            new[]
            {
                ("Abby's Horse", "A beautiful horse painting"),
                ("Fairy Wrens", "Colorful fairy wren birds"),
                ("Green Turtle", "A majestic sea turtle")
            }
        ));

        // Flowers paintings
        paintings.AddRange(CreateSamplePaintings(
            "flowers",
            new[]
            {
                ("Bird of Paradise", "Tropical bird of paradise flower"),
                ("Daffodils", "Bright yellow daffodils in spring"),
                ("Coneflowers", "Purple coneflowers in a meadow")
            }
        ));

        // Seascapes paintings
        paintings.AddRange(CreateSamplePaintings(
            "seascapes",
            new[]
            {
                ("Morning Glory", "Sunrise over the ocean"),
                ("Wave Blue", "Blue waves crashing on shore"),
                ("Sailing Sunset", "Boats sailing into sunset")
            }
        ));

        foreach (var painting in paintings)
        {
            context.Set<Painting>().Add(painting);
        }

        await context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation($"Seeded {paintings.Count} sample paintings.");
    }

    private async Task<PaintingCategory> CreateCategoryAsync(string slug, string name, string description, CancellationToken cancellationToken)
    {
        return await _categoryFactory.CreateAsync(
            new PaintingCategoryName(name),
            new PaintingCategoryDescription(description),
            cancellationToken
        );
    }

    private IEnumerable<Painting> CreateSamplePaintings(string categorySlug, params (string title, string description)[] paintings)
    {
        foreach (var (title, description) in paintings)
        {
            yield return _paintingFactory.Create(
                new PaintingName(title),
                new PaintingDescription(description),
                new PaintingImageUrl($"/{categorySlug}/{title.Replace(' ', '-').ToLower()}.jpg"),
                thumbnailUrl: null,
                new PaintingCategorySlug(categorySlug),
                price: null,
                isAvailable: new PaintingIsAvailable(true)
            );
        }
    }
}