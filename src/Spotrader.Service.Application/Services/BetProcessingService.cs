using Spotrader.Service.Application.Interfaces;
using Spotrader.Service.Domain.Entities;
using Spotrader.Service.Domain.Interfaces.Repositories;
using Spotrader.Service.Domain.ValueObjects;

namespace Spotrader.Service.Application.Services;

public class BetProcessingService : IBetProcessingService
{
    private readonly IBetRepository _betRepository;
    private static int _totalBetsProcessed = 0;

    public BetProcessingService(IBetRepository betRepository)
    {
        _betRepository = betRepository;
    }

    public bool IsShuttingDown => throw new NotImplementedException();

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
        throw new NotImplementedException();
    }

    public async Task ProcessBetAsync(Bet bet)
    {
        if (bet.Status != BetStatus.OPEN)
        {
            throw new InvalidOperationException($"Bet {bet.Id} has invalid status {bet.Status} for processing");
        }

        await Task.Delay(50);

        bet.UpdateStatus(SimulateRandomResult());

        await _betRepository.SaveAsync(bet);

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
