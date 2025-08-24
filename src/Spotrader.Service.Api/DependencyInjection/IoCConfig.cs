using Spotrader.Service.Api.Services;
using Spotrader.Service.Application.Configuration;
using Spotrader.Service.Infrastructure.Configuration;

namespace Spotrader.Service.Api.DependencyInjection;

public static class IoCConfig
{
    public static void RegisterIoCContainers(this IServiceCollection services)
    {
        services.RegisterInfrastructureModule();
        services.RegisterApplicationModule();
        services.AddSingleton<DataSeedingService>();
    }
}
