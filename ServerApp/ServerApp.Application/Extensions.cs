using Microsoft.Extensions.DependencyInjection;
using Scrutor;
using ServerApp.Shared.Abstractions.Queries;
using ServerApp.Application.Queries;

namespace ServerApp.Application;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.Scan(scan => scan
            .FromAssemblyOf<GetAllPaintingCategories>()
            .AddClasses(classes => classes
                .AssignableTo(typeof(IQueryHandler<,>))
            )
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        return services;
    }
}