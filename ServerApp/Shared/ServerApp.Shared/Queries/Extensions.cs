using Microsoft.Extensions.DependencyInjection;
using ServerApp.Shared.Abstractions.Queries;
using System.Reflection;

namespace ServerApp.Shared.Queries;

public static class Extensions
{
    public static IServiceCollection AddQueries(this IServiceCollection services)
    {
        var assembly = Assembly.GetCallingAssembly();
        services.AddSingleton<IQueryDispatcher, InMemoryQueryDispatcher>();
        services.Scan(
            x => x.FromAssemblies(assembly)
                .AddClasses(c => c.AssignableTo(typeof(IQueryHandler<,>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime());
        return services;
    }
}
