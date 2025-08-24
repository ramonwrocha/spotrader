#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:9.0.0-noble-amd64 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0.100-noble-amd64 AS build
WORKDIR /src
COPY Directory.Build.* ./
COPY NuGet.config .

COPY ["src/Sportradar.Service.Api/Sportradar.Service.Api.csproj", "Sportradar.Service.Api/"]
RUN dotnet restore "Sportradar.Service.Api/Sportradar.Service.Api.csproj"
COPY src .
WORKDIR "/src/Sportradar.Service.Api"
RUN dotnet build "Sportradar.Service.Api.csproj" -c Release -o /app/build

FROM build AS publish
WORKDIR "/src/Sportradar.Service.Api"
RUN dotnet publish "Sportradar.Service.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sportradar.Service.Api.dll"]