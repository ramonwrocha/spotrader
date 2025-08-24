using Spotrader.Service.Application.DTOs;
using Spotrader.Service.Domain.Entities;

namespace Spotrader.Service.Application.Interfaces;

public interface IBetProcessingService
{
    Task AddBetAsync(Bet bet);

    Task AddBetBatchAsync(List<Bet> bets);

    Task ProcessBetAsync(Bet bet);

    Task ProcessBetBatchAsync(IEnumerable<Bet> bets);

    Task<BetSummary> GetSummaryAsync();

    void CompleteProcessing();
}
