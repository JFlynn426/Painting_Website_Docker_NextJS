using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServerApp.Infrastructure.Commands;
using ServerApp.Infrastructure.EF;
using ServerApp.Infrastructure.Queries;
using ServerApp.Infrastructure.Services;

namespace ServerApp.Infrastructure;

/// <summary>
/// Extension methods for registering infrastructure services with the DI container.
/// Follows DDD and CQRS patterns with proper separation of concerns.
/// </summary>
public static class InfrastructureExtensions
{
    /// <summary>
    /// Adds all infrastructure services to the service collection.
    /// This includes DbContexts, repositories, command handlers, and query handlers.
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

        // Register command handlers using Scrutor for automatic discovery
        services.AddCommandHandlers();

        // Register query handlers using Scrutor for automatic discovery
        services.AddQueryHandlers();

        // Register the app initializer for database migrations
        services.AddHostedService<AppInitializer>();

        return services;
    }
}