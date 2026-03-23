using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.MicrosoftExtensions;
using ServerApp.Application;
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
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Add Application layer services
            builder.Services.AddApplicationServices();

            var app = builder.Build();

            app.UseSwagger();
            app.UseSwaggerUI();

            //remove before production - not using dev environment yet: add https too.
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
