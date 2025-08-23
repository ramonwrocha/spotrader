using Spotrader.Service.Infrastructure.Configuration;
using Spotrader.Service.Application.Configuration;

namespace Spotrader.Service.Api.DependencyInjection;

public static class IoCConfig
{
    public static void RegisterIoCContainers(this IServiceCollection services, IConfiguration configuration)
    {
        services.RegisterInfrastructureModule(configuration);
        services.RegisterApplicationModule(configuration);
    }
}
