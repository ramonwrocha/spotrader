using Microsoft.Extensions.Hosting;
using Spotrader.Service.Application.Interfaces;
using Spotrader.Service.Domain.Entities;
using Spotrader.Service.Domain.Interfaces.Queue;

namespace Spotrader.Service.Application.Workers;

public class BetWorker : BackgroundService
{
    private readonly IBetQueue _betQueue;
    private readonly IBetProcessingService _betProcessingService;

    public BetWorker(
        IBetQueue betQueue,
        IBetProcessingService betProcessingService)
    {
        _betQueue = betQueue;
        _betProcessingService = betProcessingService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var bet = await _betQueue.DequeueAsync(stoppingToken);

                if (bet is not null)
                {
                    await ProcessBetAsync(bet);
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }
    }

    private async Task ProcessBetAsync(Bet bet)
    {
        try
        {
            await _betProcessingService.ProcessBetAsync(bet);
        }
        catch (Exception ex)
        {
            throw;
        }
    }
}