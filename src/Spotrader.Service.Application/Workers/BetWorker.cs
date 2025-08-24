using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Spotrader.Service.Application.Interfaces;
using Spotrader.Service.Application.Services;

namespace Spotrader.Service.Application.Workers;

public class BetWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IBetChannelService _channelService;

    public BetWorker(
        IServiceScopeFactory scopeFactory,
        IBetChannelService channelService)
    {
        _scopeFactory = scopeFactory;
        _channelService = channelService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var bet in _channelService.Reader.ReadAllAsync(stoppingToken))
        {
            using var scope = _scopeFactory.CreateScope();
            var betProcessingService = scope.ServiceProvider.GetRequiredService<BetProcessingService>();

            await betProcessingService.ProcessBetAsync(bet);
        }
    }
}