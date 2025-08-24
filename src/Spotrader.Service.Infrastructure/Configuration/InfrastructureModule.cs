using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Spotrader.Service.Domain.Interfaces.Repositories;
using Spotrader.Service.Infrastructure.Data;
using Spotrader.Service.Infrastructure.Data.Repositories;

namespace Spotrader.Service.Infrastructure.Configuration;

public static class InfrastructureModule
{
    public static void RegisterInfrastructureModule(this IServiceCollection services)
    {
        services.RegisterSettings();
        services.RegisterDataServices();
        services.RegisterRepositories();
    }

    public static async Task ApplyMigrationsAsync(this IServiceProvider serviceProvider)
    {
        var contextFactory = serviceProvider.GetRequiredService<IDbContextFactory<SpotraderDbContext>>();
        using var context = await contextFactory.CreateDbContextAsync();
        await context.Database.MigrateAsync();
    }

    private static void RegisterSettings(this IServiceCollection services)
    {
        services.AddOptions<PostgreSqlSettings>()
            .BindConfiguration(PostgreSqlSettings.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();
    }

    private static void RegisterDataServices(this IServiceCollection services)
    {
        services.AddDbContextFactory<SpotraderDbContext>((serviceProvider, options) =>
        {
            var settings = serviceProvider.GetRequiredService<IOptions<PostgreSqlSettings>>();

            options.UseNpgsql(settings.Value.ConnectionString, npgsqlOptions =>
            {
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorCodesToAdd: null);
            });
        });
    }

    private static IServiceCollection RegisterRepositories(this IServiceCollection services)
    {
        services.AddSingleton<IBetRepository, BetRepository>();
        return services;
    }
}
