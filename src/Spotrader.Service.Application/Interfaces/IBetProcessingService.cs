using Spotrader.Service.Domain.Entities;

namespace Spotrader.Service.Application.Interfaces;

public interface IBetProcessingService
{
    Task AddBetAsync(Bet bet);

    Task AddBetBatchAsync(IEnumerable<Bet> bets);

    Task ProcessBetAsync(Bet bet);

    Task ProcessBetBatchAsync(IEnumerable<Bet> bets);

    Task<string> GetSummaryAsync();

    Task CompleteProcessingAsync();
}
