using Spotrader.Service.Application.Interfaces;

namespace Sportradar.Service.Worker.Workers;

public class BettingWorker : BackgroundService
{
    private readonly IBetChannelService _channelService;
    private readonly IBetProcessingService _betProcessingService;

    public BettingWorker(IBetChannelService betChannelService, IBetProcessingService betProcessingService)
    {
        _channelService = betChannelService;
        _betProcessingService = betProcessingService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
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
}
