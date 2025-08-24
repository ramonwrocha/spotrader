using Spotrader.Service.Application.DTOs;
using Spotrader.Service.Domain.Entities;

namespace Spotrader.Service.Application.Interfaces;

public interface IBetProcessingService
{
    Task AddBetAsync(Bet bet);

    Task AddBetBatchAsync(IEnumerable<Bet> bets);

    Task ProcessBetAsync(Bet bet, CancellationToken cancellationToken);

    Task ProcessBetBatchAsync(IEnumerable<Bet> bets, CancellationToken cancellationToken);

    Task<BetSummary> GetSummaryAsync();

    void CompleteProcessing();
}
