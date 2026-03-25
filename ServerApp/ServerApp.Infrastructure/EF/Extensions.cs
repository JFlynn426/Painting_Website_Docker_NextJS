using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServerApp.Domain.Repositories;
using ServerApp.Infrastructure.EF.Contexts;
using ServerApp.Infrastructure.EF.Options;
using ServerApp.Infrastructure.EF.Repositories;

namespace ServerApp.Infrastructure.EF;

/// <summary>
/// Extension methods for registering EF-related services with the DI container.
/// </summary>
public static class EfExtensions
{
    /// <summary>
    /// Registers SQL Server DbContexts and repositories with the DI container.
    /// Implements CQRS pattern with separate contexts for read and write operations.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="configuration">The application configuration.</param>
    public static IServiceCollection AddSQLServer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IPaintingRepository, SQLServerPaintingRepository>();
        services.AddScoped<IPaintingCategoryRepository, SQLServerPaintingCategoryRepository>();
        services.AddScoped<IPageContentRepository, SQLServerPageContentRepository>();

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? configuration[$"{nameof(SQLServerOptions)}:ConnectionString"]
            ?? string.Empty;

        services.AddDbContext<ReadDbContext>(ctx => ctx.UseSqlServer(connectionString));
        services.AddDbContext<WriteDbContext>(ctx => ctx.UseSqlServer(connectionString));

        return services;
    }
}