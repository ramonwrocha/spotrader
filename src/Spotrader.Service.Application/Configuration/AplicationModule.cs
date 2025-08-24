using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spotrader.Service.Application.Interfaces;
using Spotrader.Service.Application.Services;
using Spotrader.Service.Application.Workers;

namespace Spotrader.Service.Application.Configuration;

public static class AplicationModule
{
    public static void RegisterApplicationModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IBetChannelService, BetChannelService>();        
        services.AddScoped<BetProcessingService>();        
        services.AddHostedService<BetWorker>();
    }
}
