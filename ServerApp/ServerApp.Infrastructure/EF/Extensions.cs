using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServerApp.Domain.Repositories.Read;
using ServerApp.Domain.Repositories.Write;
using ServerApp.Infrastructure.EF.Contexts;
using ServerApp.Infrastructure.EF.Options;
using ServerApp.Infrastructure.EF.Repositories.Read;
using ServerApp.Infrastructure.EF.Repositories.Write;

namespace ServerApp.Infrastructure.EF;

/// <summary>
/// Extension methods for registering EF-related services with the DI container.
/// </summary>
public static class EfExtensions
{
    /// <summary>
    /// Registers SQL Server DbContexts and repositories with the DI container.
    /// Implements CQRS pattern with separate read and write repositories.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="configuration">The application configuration.</param>
    public static IServiceCollection AddSQLServer(this IServiceCollection services, IConfiguration configuration)
    {
        // Register read repositories (query operations)
        services.AddScoped<IPaintingReadRepository, SQLServerPaintingReadRepository>();
        services.AddScoped<IPaintingCategoryReadRepository, SQLServerPaintingCategoryReadRepository>();
        services.AddScoped<IPageContentReadRepository, SQLServerPageContentReadRepository>();

        // Register write repositories (command operations)
        services.AddScoped<IPaintingWriteRepository, SQLServerPaintingWriteRepository>();
        services.AddScoped<IPaintingCategoryWriteRepository, SQLServerPaintingCategoryWriteRepository>();
        services.AddScoped<IPageContentWriteRepository, SQLServerPageContentWriteRepository>();

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? configuration[$"{nameof(SQLServerOptions)}:ConnectionString"]
            ?? string.Empty;

        services.AddDbContext<ReadDbContext>(ctx => ctx.UseSqlServer(connectionString));
        services.AddDbContext<WriteDbContext>(ctx => ctx.UseSqlServer(connectionString));

        return services;
    }
}