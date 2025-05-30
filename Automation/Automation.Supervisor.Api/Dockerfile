#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Automation.Supervisor.Api/Automation.Supervisor.Api.csproj", "Automation.Supervisor.Api/"]
RUN dotnet restore "./Automation.Supervisor.Api/Automation.Supervisor.Api.csproj"
COPY . .
WORKDIR "/src/Automation.Supervisor.Api"
RUN dotnet build "./Automation.Supervisor.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Automation.Supervisor.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY .env .
ENTRYPOINT ["dotnet", "Automation.Supervisor.Api.dll"]