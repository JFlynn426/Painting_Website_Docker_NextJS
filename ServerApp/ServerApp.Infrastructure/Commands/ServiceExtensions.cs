using Microsoft.Extensions.DependencyInjection;
using Scrutor;
using ServerApp.Infrastructure.Commands.Handlers;
using ServerApp.Shared.Abstractions.Commands;

namespace ServerApp.Infrastructure.Commands;

/// <summary>
/// Extension methods for registering command handler services with the DI container.
/// </summary>
public static class CommandServiceExtensions
{
    /// <summary>
    /// Registers command handlers using Scrutor for automatic discovery.
    /// Command handlers implement ICommandHandler<TCommand> interface.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    public static void AddCommandHandlers(this IServiceCollection services)
    {
        services.Scan(scan => scan
            .FromAssemblyOf<AddPaintingHandler>()
            .AddClasses(classes => classes
                .AssignableTo(typeof(ICommandHandler<>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());
    }
}