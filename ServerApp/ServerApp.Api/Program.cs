using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
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
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });

            // Add Application layer services
            builder.Services.AddApplicationServices();

            // Add Infrastructure layer services
            builder.Services.AddInfrastructureServices(builder.Configuration);

            // Register authorized admin emails from configuration
            var authorizedEmailsConfig = builder.Configuration["Admin:AuthorizedEmails"];
            var authorizedEmails = !string.IsNullOrEmpty(authorizedEmailsConfig)
                ? authorizedEmailsConfig.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                : Array.Empty<string>();
            builder.Services.AddSingleton<IEnumerable<string>>(authorizedEmails);

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

            // Serve uploaded images statically
            var imagesPath = builder.Configuration["ImageProcessing:StoragePath"] ?? "/app/images";
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(imagesPath),
                RequestPath = "/images"
            });

            app.MapControllers();

            app.Run();
        }
    }
}
