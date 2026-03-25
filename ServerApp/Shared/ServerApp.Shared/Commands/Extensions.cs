using Microsoft.Extensions.DependencyInjection;
using ServerApp.Shared.Abstractions.Commands;
using System.Reflection;

namespace ServerApp.Shared.Commands;

public static class Extensions
{
    public static IServiceCollection AddCommands(this IServiceCollection services)
    {
        var assembly = Assembly.GetCallingAssembly();
        services.AddSingleton<ICommandDispatcher, InMemoryCommandDispatcher>();
        services.Scan(
            x => x.FromAssemblies(assembly)
                .AddClasses(c => c.AssignableTo(typeof(ICommandHandler<>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime());
        return services;
    }
}
