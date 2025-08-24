using Microsoft.Extensions.Hosting;
using Sportradar.Service.Api.Services;
using Sportradar.Service.Application.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace Sportradar.Service.Application.Workers;

[ExcludeFromCodeCoverage]
public class BettingWorker : BackgroundService
{
    private readonly IBetChannelService _channelService;
    private readonly IBetProcessingService _betProcessingService;
    private readonly DataSeedingService _dataSeedingService;

    public BettingWorker(
        IBetChannelService betChannelService,
        IBetProcessingService betProcessingService,
        DataSeedingService dataSeedingService)
    {
        _channelService = betChannelService;
        _betProcessingService = betProcessingService;
        _dataSeedingService = dataSeedingService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _dataSeedingService.SeedInitialBetsAsync(100);

        var singleBetTask = ProcessSingleBets(stoppingToken);
        var batchBetTask = ProcessBatchBets(stoppingToken);

        await Task.WhenAll(singleBetTask, batchBetTask);
    }

    private async Task ProcessSingleBets(CancellationToken stoppingToken)
    {
        await foreach (var bet in _channelService.Reader.ReadAllAsync(stoppingToken))
        {
            await _betProcessingService.ProcessBetAsync(bet);
        }
    }

    private async Task ProcessBatchBets(CancellationToken stoppingToken)
    {
        await foreach (var bets in _channelService.BatchReader.ReadAllAsync(stoppingToken))
        {
            await _betProcessingService.ProcessBetBatchAsync(bets);
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _channelService.CompleteAdding();
        await base.StopAsync(cancellationToken);
    }
}
