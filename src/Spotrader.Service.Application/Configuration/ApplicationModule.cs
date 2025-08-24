using Microsoft.Extensions.DependencyInjection;
using Spotrader.Service.Application.Interfaces;
using Spotrader.Service.Application.Services;
using Spotrader.Service.Application.Workers;

namespace Spotrader.Service.Application.Configuration;

public static class ApplicationModule
{
    public static void RegisterApplicationModule(this IServiceCollection services)
    {
        services
            .RegisterSettings()
            .RegisterServices();
    }

    private static IServiceCollection RegisterServices(this IServiceCollection services)
    {
        services.AddSingleton<IBetChannelService, BetChannelService>();
        services.AddSingleton<IBetProcessingService, BetProcessingService>();

        services.AddHostedService<BettingWorker>();

        return services;
    }

    private static IServiceCollection RegisterSettings(this IServiceCollection services)
    {
         services.AddOptions<ApplicationSettings>()
            .BindConfiguration(ApplicationSettings.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }
}
