using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ServerApp.Infrastructure.EF.Contexts;

namespace ServerApp.Infrastructure.Services;

internal sealed class AppInitializer : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IHostEnvironment _environment;

    public AppInitializer(IServiceProvider serviceProvider, IHostEnvironment environment)
    {
        _serviceProvider = serviceProvider;
        _environment = environment;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();

        // Migrate WriteDbContext (both Read and Write contexts share the same tables)
        var writeDbContext = scope.ServiceProvider.GetRequiredService<WriteDbContext>();
        await writeDbContext.Database.MigrateAsync(cancellationToken);

        // Migrate ReadDbContext
        var readDbContext = scope.ServiceProvider.GetRequiredService<ReadDbContext>();
        await readDbContext.Database.MigrateAsync(cancellationToken);

        // Seed database if empty
        var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
        await seeder.SeedAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}