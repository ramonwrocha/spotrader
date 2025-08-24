using Sportradar.Service.Api.Services;
using Sportradar.Service.Application.Configuration;
using Sportradar.Service.Infrastructure.Configuration;

namespace Sportradar.Service.Api.DependencyInjection;

public static class IoCConfig
{
    public static void RegisterIoCContainers(this IServiceCollection services)
    {
        services.RegisterInfrastructureModule();
        services.RegisterApplicationModule();
        services.AddSingleton<DataSeedingService>();
    }
}
