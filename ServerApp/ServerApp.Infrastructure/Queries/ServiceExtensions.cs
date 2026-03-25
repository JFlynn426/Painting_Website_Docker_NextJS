using Microsoft.Extensions.DependencyInjection;
using Scrutor;
using ServerApp.Infrastructure.Queries.Handlers;
using ServerApp.Shared.Abstractions.Queries;

namespace ServerApp.Infrastructure.Queries;

/// <summary>
/// Extension methods for registering query handler services with the DI container.
/// </summary>
public static class QueryServiceExtensions
{
    /// <summary>
    /// Registers query handlers using Scrutor for automatic discovery.
    /// Query handlers implement IQueryHandler<TQuery, TResult> interface.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    public static void AddQueryHandlers(this IServiceCollection services)
    {
        services.Scan(scan => scan
            .FromAssemblyOf<GetPaintingHandler>()
            .AddClasses(classes => classes
                .AssignableTo(typeof(IQueryHandler<,>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());
    }
}