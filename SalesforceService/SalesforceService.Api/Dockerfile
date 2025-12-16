# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["SalesforceService/SalesforceService.Api/SalesforceService.Api.csproj", "SalesforceService/SalesforceService.Api/"]
COPY ["SalesforceService/SalesforceService.Application/SalesforceService.Application.csproj", "SalesforceService/SalesforceService.Application/"]
COPY ["SalesforceService/SalesforceService.Domain/SalesforceService.Domain.csproj", "SalesforceService/SalesforceService.Domain/"]
COPY ["SalesforceService/SalesforceService.DatabaseMigration/SalesforceService.DatabaseMigration.csproj", "SalesforceService/SalesforceService.DatabaseMigration/"]
COPY ["SalesforceService/SalesforceService.Infrastructure/SalesforceService.Infrastructure.csproj", "SalesforceService/SalesforceService.Infrastructure/"]
RUN dotnet restore "./SalesforceService/SalesforceService.Api/SalesforceService.Api.csproj"
COPY . .
WORKDIR "/src/SalesforceService/SalesforceService.Api"
RUN dotnet build "./SalesforceService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SalesforceService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

COPY SalesforceService/SalesforceService.Api/config/topic-definitions.yaml ./config/topic-definitions.yaml

ENTRYPOINT ["dotnet", "SalesforceService.Api.dll"]