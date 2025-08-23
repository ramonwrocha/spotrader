using Spotrader.Service.Domain.Entities;

namespace Spotrader.Service.Domain.Interfaces;

public interface IBetRepository
{
    Task<Bet?> GetByIdAsync(long id);

    Task SaveAsync(Bet bet);
    
    Task<List<Bet>> GetAllAsync();
}
