# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ServerApp.Api/ServerApp.Api.csproj", "ServerApp.Api/"]
COPY ["ServerApp.Application/ServerApp.Application.csproj", "ServerApp.Application/"]
COPY ["ServerApp.Infrastructure/ServerApp.Infrastructure.csproj", "ServerApp.Infrastructure/"]
COPY ["ServerApp.Domain/ServerApp.Domain.csproj", "ServerApp.Domain/"]
COPY ["ServerApp.Shared/ServerApp.Shared.csproj", "ServerApp.Shared/"]
RUN dotnet restore "ServerApp.Api/ServerApp.Api.csproj"

COPY . .
RUN dotnet build "ServerApp.Api/ServerApp.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ServerApp.Api/ServerApp.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false \
    /p:PublishReadyToRun=true /p:PublishReadyToRunComposite=true

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ServerApp.Api.dll"]