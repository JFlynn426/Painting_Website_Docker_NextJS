using Microsoft.Extensions.DependencyInjection;
using MediatR;
using ServerApp.Application.Commands.Handlers;
using ServerApp.Application.Queries.Handlers;

namespace ServerApp.Application;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register MediatR for handling commands and queries
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(AddPaintingHandler).Assembly));

        return services;
    }
}