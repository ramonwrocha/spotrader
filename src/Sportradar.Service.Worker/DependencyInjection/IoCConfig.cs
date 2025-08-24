using Spotrader.Service.Infrastructure.Configuration;
using Spotrader.Service.Application.Configuration;

namespace Sportradar.Service.Worker.DependencyInjection;

public static class IoCConfig
{
    public static void RegisterIoCContainers(this IServiceCollection services)
    {
        services.RegisterInfrastructureModule();
        services.RegisterApplicationModule();
    }
}
