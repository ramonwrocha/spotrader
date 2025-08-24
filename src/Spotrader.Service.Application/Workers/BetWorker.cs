using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Spotrader.Service.Application.Interfaces;
using Spotrader.Service.Application.Services;

namespace Spotrader.Service.Application.Workers;

public class BetWorker : BackgroundService
{
    private readonly ILogger<BetWorker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IBetChannelService _channelService;

    public BetWorker(
        ILogger<BetWorker> logger,
        IServiceScopeFactory scopeFactory,
        IBetChannelService channelService)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _channelService = channelService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("BetWorker started");

        await foreach (var bet in _channelService.Reader.ReadAllAsync(stoppingToken))
        {
            using var scope = _scopeFactory.CreateScope();
            var betProcessingService = scope.ServiceProvider.GetRequiredService<BetProcessingService>();

            try
            {
                await betProcessingService.ProcessBetAsync(bet);
                _logger.LogInformation("Processed bet {BetId} for client {Client}", bet.Id, bet.Client);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing bet {BetId}: {Error}", bet.Id, ex.Message);
            }
        }

        _logger.LogInformation("BetWorker stopped");
    }
}