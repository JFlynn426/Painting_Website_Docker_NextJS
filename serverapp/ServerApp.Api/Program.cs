using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.MicrosoftExtensions;
using ServerApp.Application;
using ServerApp.Infrastructure;
using ServerApp.Api.Middleware;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ServerApp.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Register services
            builder.Services.AddControllers();

            // Add Swagger only in Development environment
            if (builder.Environment.IsDevelopment())
            {
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen();
            }

            // Add CORS configuration
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    var allowedOrigins = builder.Configuration["CORS_ALLOWED_ORIGINS"]?
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .ToList() ?? new List<string>();

                    policy.WithOrigins(allowedOrigins.ToArray())
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            // Add Application layer services
            builder.Services.AddApplicationServices();

            // Add Infrastructure layer services
            builder.Services.AddInfrastructureServices(builder.Configuration);

            var app = builder.Build();

            // Add exception handling middleware (must be early in pipeline)
            app.UseExceptionMiddleware();

            // Enable CORS
            app.UseCors("AllowFrontend");

            // Enable Swagger only in Development environment
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.MapControllers();

            app.Run();
        }
    }
}
