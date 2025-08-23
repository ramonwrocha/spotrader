using Spotrader.Service.Application.Interfaces;
using Spotrader.Service.Domain.Entities;
using Spotrader.Service.Domain.Interfaces.Repositories;
using Spotrader.Service.Domain.ValueObjects;

namespace Spotrader.Service.Application.Services;

public class BetProcessingService : IBetProcessingService
{
    private readonly IBetRepository _betRepository;
    private readonly CancellationTokenSource _shutdownTokenSource = new();
    private static ulong _totalBetsProcessed = 0;

    public BetProcessingService(IBetRepository betRepository)
    {
        _betRepository = betRepository;
    }

    public bool IsShuttingDown => _shutdownTokenSource.Token.IsCancellationRequested;

    public Task AddBetAsync(Bet bet)
    {
        throw new NotImplementedException();
    }

    public Task<string> GetSummaryAsync()
    {
        throw new NotImplementedException();
    }

    public Task ShutdownAsync()
    {
        _shutdownTokenSource.Cancel();
        return Task.CompletedTask;
    }

    public async Task ProcessBetAsync(Bet bet)
    {
        if (bet.Status != BetStatus.OPEN)
        {
            throw new InvalidOperationException($"Bet {bet.Id} has invalid status {bet.Status} for processing");
        }

        await Task.Delay(50);

        bet.UpdateStatus(SimulateRandomResult());

        await _betRepository.AddAsync(bet);

        Interlocked.Increment(ref _totalBetsProcessed);
    }

    private static BetStatus SimulateRandomResult()
    {
        var random = Random.Shared.NextDouble();

        if (random <= 0.45)
        {
            return BetStatus.WINNER;            
        }

        if (random <= 0.90)
        {
            return BetStatus.LOSER;            
        }
        
        return BetStatus.VOID;
    }
}
