using Spotrader.Service.Domain.DTOs;
using Spotrader.Service.Domain.Entities;

namespace Spotrader.Service.Domain.Interfaces.Repositories;

public interface IBetRepository
{
    Task<Bet?> GetByIdAsync(long id);

    Task AddAsync(Bet bet, CancellationToken cancellationToken);

    Task AddRangeAsync(IEnumerable<Bet> bets, CancellationToken cancellationToken);

    Task<BasicStatsDto> GetBasicStatsAsync();

    Task<List<ClientProfitDto>> GetTopClientsWithProfitsAsync(int take = 5);

    Task<List<ClientLossDto>> GetTopClientsWithLossesAsync(int take = 5);
}
