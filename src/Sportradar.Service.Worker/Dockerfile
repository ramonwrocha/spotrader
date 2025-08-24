FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project files
COPY ["src/Sportradar.Service.Worker/Sportradar.Service.Worker.csproj", "src/Sportradar.Service.Worker/"]
COPY ["src/Spotrader.Service.Application/Spotrader.Service.Application.csproj", "src/Spotrader.Service.Application/"]
COPY ["src/Spotrader.Service.Domain/Spotrader.Service.Domain.csproj", "src/Spotrader.Service.Domain/"]
COPY ["src/Spotrader.Service.Infrastructure/Spotrader.Service.Infrastructure.csproj", "src/Spotrader.Service.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "src/Sportradar.Service.Worker/Sportradar.Service.Worker.csproj"

# Copy source code
COPY . .
WORKDIR "/src/src/Sportradar.Service.Worker"

# Build
RUN dotnet build "Sportradar.Service.Worker.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Sportradar.Service.Worker.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Sportradar.Service.Worker.dll"]