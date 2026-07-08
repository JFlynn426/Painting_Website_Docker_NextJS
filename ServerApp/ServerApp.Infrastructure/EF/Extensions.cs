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
    /// Registers EF Core DbContexts and repositories with the DI container.
    /// Implements CQRS pattern with separate read and write repositories.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="configuration">The application configuration.</param>
    public static IServiceCollection AddEFRepositories(this IServiceCollection services, IConfiguration configuration)
    {
        // Register read repositories (query operations)
        services.AddScoped<IPaintingReadRepository, PaintingReadRepository>();
        services.AddScoped<IPaintingCategoryReadRepository, PaintingCategoryReadRepository>();
        services.AddScoped<IPageContentReadRepository, PageContentReadRepository>();
        services.AddScoped<IAdminUserReadRepository, AdminUserReadRepository>();

        // Register write repositories (command operations)
        services.AddScoped<IPaintingWriteRepository, PaintingWriteRepository>();
        services.AddScoped<IPaintingCategoryWriteRepository, PaintingCategoryWriteRepository>();
        services.AddScoped<IPageContentWriteRepository, PageContentWriteRepository>();
        services.AddScoped<IAdminUserWriteRepository, AdminUserWriteRepository>();

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? configuration[$"{nameof(EFRepositoryOptions)}:ConnectionString"]
            ?? string.Empty;

        services.AddDbContext<ReadDbContext>(ctx => ctx.UseNpgsql(connectionString));
        services.AddDbContext<WriteDbContext>(ctx => ctx.UseNpgsql(connectionString));

        return services;
    }
}
