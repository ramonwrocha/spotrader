using Spotrader.Service.Infrastructure.Configuration;

namespace Spotrader.Service.Api.DependencyInjection;

public static class IoCConfig
{
    public static void RegisterIoCContainers(this IServiceCollection services, IConfiguration configuration)
    {
        services.RegisterInfrastructure(configuration);
        services.RegisterServices();
        services.RegisterRepositories();
        services.RegisterWorkers();
    }

    private static void RegisterWorkers(this IServiceCollection services)
    {
        //services.AddHostedService<BettingWorker>();
    }

    private static void RegisterServices(this IServiceCollection services)
    {
    }

    private static void RegisterRepositories(this IServiceCollection services)
    {
    }

    private static void RegisterSettings(this IServiceCollection services)
    {
    }
}
