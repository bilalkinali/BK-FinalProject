# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ContentModerationService/ContentModerationService.Api/ContentModerationService.Api.csproj", "ContentModerationService/ContentModerationService.Api/"]
COPY ["ContentModerationService/ContentModerationService.Application/ContentModerationService.Application.csproj", "ContentModerationService/ContentModerationService.Application/"]
COPY ["ContentModerationService/ContentModerationService.Domain/ContentModerationService.Domain.csproj", "ContentModerationService/ContentModerationService.Domain/"]
COPY ["ContentModerationService/ContentModerationService.DatabaseMigration/ContentModerationService.DatabaseMigration.csproj", "ContentModerationService/ContentModerationService.DatabaseMigration/"]
COPY ["ContentModerationService/ContentModerationService.Infrastructure/ContentModerationService.Infrastructure.csproj", "ContentModerationService/ContentModerationService.Infrastructure/"]
RUN dotnet restore "./ContentModerationService/ContentModerationService.Api/ContentModerationService.Api.csproj"
COPY . .
WORKDIR "/src/ContentModerationService/ContentModerationService.Api"
RUN dotnet build "./ContentModerationService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ContentModerationService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ContentModerationService.Api.dll"]