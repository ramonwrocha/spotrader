using Microsoft.EntityFrameworkCore;
using Spotrader.Service.Domain.Entities;
using Spotrader.Service.Domain.Interfaces;
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
        var result = await _context.Bets
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == id);

        return result is null
            ? null
            : result.ToDomain();
    }

    public async Task SaveAsync(Bet bet)
    {
        var existing = await _context.Bets.FirstOrDefaultAsync(b => b.Id == bet.Id);

        if (existing is not null)
        {
            _context.Entry(existing).CurrentValues.SetValues(bet);
        }
        else
        {
            await _context.Bets.AddAsync(bet.ToEntity());
        }

        await _context.SaveChangesAsync();
    }

    public async Task<List<Bet>> GetAllAsync()
    {
        var result = await _context.Bets
            .AsNoTracking()
            .ToListAsync();

        return result.Select(bet => bet.ToDomain()).ToList();
    }
}
