using Spotrader.Service.Domain.Entities;

namespace Spotrader.Service.Domain.Interfaces.Repositories;

public interface IBetRepository
{
    Task<Bet?> GetByIdAsync(long id);

    Task AddAsync(Bet bet);
    
    Task<List<Bet>> GetAllAsync();
}
