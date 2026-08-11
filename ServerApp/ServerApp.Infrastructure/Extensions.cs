using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServerApp.Application.Services;
using ServerApp.Domain.Factories;
using ServerApp.Domain.Services;
using ServerApp.Infrastructure.EF;
using ServerApp.Infrastructure.EF.Contexts;
using ServerApp.Infrastructure.Persistence;
using ServerApp.Infrastructure.Services;
using ServerApp.Shared.Persistence;

namespace ServerApp.Infrastructure;

/// <summary>
/// Extension methods for registering infrastructure services with the DI container.
/// Follows DDD and CQRS patterns with proper separation of concerns.
/// </summary>
public static class InfrastructureExtensions
{
    /// <summary>
    /// Adds all infrastructure services to the service collection.
    /// This includes DbContexts, repositories, UnitOfWork, and domain factories.
    /// Command and Query handlers are registered in the Application layer.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The service collection with infrastructure services registered.</returns>
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register EF Core DbContexts and repositories
        services.AddEFRepositories(configuration);

        // Register UnitOfWork for transaction management with read-only mode support
        var readOnlyMode = bool.Parse(configuration["Database:ReadOnlyMode"] ?? "false");
        services.AddScoped<IUnitOfWork>(sp =>
        {
            var dbContext = sp.GetRequiredService<WriteDbContext>();
            return new UnitOfWork(dbContext, readOnlyMode);
        });

        // Register domain factories for entity creation
        services.AddScoped<IPaintingFactory, PaintingFactory>();
        services.AddScoped<IPaintingCategoryFactory, PaintingCategoryFactory>();
        services.AddScoped<IPageContentFactory, PageContentFactory>();
        services.AddScoped<IAdminUserFactory, AdminUserFactory>();

        // Register HTML sanitizer service
        services.AddScoped<IHtmlSanitizer, HtmlSanitizer>();

        // Register the database seeder
        services.AddScoped<DatabaseSeeder>();

        // Register the app initializer for database migrations and seeding
        services.AddHostedService<AppInitializer>();

        // Register named HTTP clients for auth services with 30-second timeout
        services.AddHttpClient("GoogleAuth", client =>
        {
            client.Timeout = TimeSpan.FromSeconds(30);
        });
        services.AddHttpClient("YahooAuth", client =>
        {
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        // Register authentication services
        services.AddScoped<IGoogleAuthService, GoogleAuthService>();
        services.AddScoped<IYahooAuthService, YahooAuthService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();

        // Register OAuth state store
        services.AddScoped<IStateStore, StateStore>();

        // Register concurrency and idempotency services
        services.AddSingleton<IConcurrencyLockService, ConcurrencyLockService>();
        services.AddSingleton<IIdempotencyKeyService, IdempotencyKeyService>();

        // Register image processing service
        services.AddScoped<IImageProcessingService, ImageProcessingService>();

        return services;
    }
}