# ============================================================================
# .NET API Dockerfile - Non-Root User Configuration
# ============================================================================
# This Dockerfile builds a .NET 10 API application that runs as a non-root user
# for improved security. The base image provides a built-in non-root user.
# ============================================================================

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
# The .NET base image includes a non-root user (appuser, UID 1)
# Explicitly set to ensure non-root execution
USER $APP_UID
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

# This stage is used in production or when running from VS in regular mode
FROM base AS final
WORKDIR /app
# Switch to root to install curl, then switch back to non-root
USER root
RUN apt-get update && apt-get install -y --no-install-recommends curl && rm -rf /var/lib/apt/lists/*
# Copy published files from the publish stage
COPY --from=publish /app/publish .
# Switch to non-root user (the .NET base image already sets up permissions correctly)
USER $APP_UID
ENTRYPOINT ["dotnet", "ServerApp.Api.dll"]