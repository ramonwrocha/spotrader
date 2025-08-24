using Sportradar.Service.Domain.DTOs;
using Sportradar.Service.Domain.Entities;

namespace Sportradar.Service.Domain.Interfaces.Repositories;

public interface IBetRepository
{
    Task<Bet?> GetByIdAsync(long id);

    Task AddAsync(Bet bet);

    Task AddRangeAsync(IEnumerable<Bet> bets);

    Task<BasicStatsDto> GetBasicStatsAsync();

    Task<List<ClientProfitDto>> GetTopClientsWithProfitsAsync(int take = 5);

    Task<List<ClientLossDto>> GetTopClientsWithLossesAsync(int take = 5);
}
