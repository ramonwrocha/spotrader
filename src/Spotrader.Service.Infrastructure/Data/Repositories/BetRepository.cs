using Microsoft.EntityFrameworkCore;
using Spotrader.Service.Domain.Entities;
using Spotrader.Service.Domain.Interfaces.Repositories;
using Spotrader.Service.Infrastructure.Data.Mappers;

namespace Spotrader.Service.Infrastructure.Data.Repositories;

public sealed class BetRepository : IBetRepository
{
    private readonly SpotraderDbContext _context;

    public BetRepository(SpotraderDbContext context)
    {
        _context = context;
    }

    public async Task<Bet?> GetByIdAsync(long id)
    {
        var entity = await _context.Bets
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == id);

        return entity?.ToDomain();
    }

    public async Task AddAsync(Bet bet)
    {
        await _context.Bets.AddAsync(bet.ToEntity());

        await _context.SaveChangesAsync();
    }

    public async Task<List<Bet>> GetAllAsync()
    {
        var entities = await _context.Bets
            .AsNoTracking()
            .ToListAsync();

        return entities.Select(bet => bet.ToDomain()).ToList();
    }
}
