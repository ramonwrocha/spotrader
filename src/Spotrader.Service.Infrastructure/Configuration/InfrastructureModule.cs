using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Spotrader.Service.Domain.Interfaces.Queue;
using Spotrader.Service.Domain.Interfaces.Repositories;
using Spotrader.Service.Infrastructure.Data;
using Spotrader.Service.Infrastructure.Data.Repositories;
using Spotrader.Service.Infrastructure.Messaging.Queues;

namespace Spotrader.Service.Infrastructure.Configuration;

public static class InfrastructureModule
{
    public static void RegisterInfrastructureModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.RegisterSettings(configuration);
        services.RegisterDataServices();
        services.RegisterMessagingServices();
    }

    public static async Task ApplyMigrationsAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<SpotraderDbContext>();
        await context.Database.MigrateAsync();
    }

    private static void RegisterSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<PostgreSqlSettings>()
            .BindConfiguration(PostgreSqlSettings.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();
    }

    private static void RegisterMessagingServices(this IServiceCollection services)
    {
        services.AddSingleton<IBetQueue, BetQueue>();
    }

    private static void RegisterDataServices(this IServiceCollection services)
    {
        services.AddDbContext<SpotraderDbContext>((serviceProvider, options) =>
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

        services.AddScoped<IBetRepository, BetRepository>();

    }
}
