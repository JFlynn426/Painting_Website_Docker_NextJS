using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServerApp.Domain.Factories;
using ServerApp.Infrastructure.EF;
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
        // Register SQL Server DbContexts and repositories
        services.AddSQLServer(configuration);

        // Register UnitOfWork for transaction management
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Register domain factories for entity creation
        services.AddScoped<IPaintingFactory, PaintingFactory>();
        services.AddScoped<IPaintingCategoryFactory, PaintingCategoryFactory>();
        services.AddScoped<IPageContentFactory, PageContentFactory>();

        // Register the app initializer for database migrations
        services.AddHostedService<AppInitializer>();

        return services;
    }
}