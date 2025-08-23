using Microsoft.EntityFrameworkCore;
using Spotrader.Service.Domain.DTOs;
using Spotrader.Service.Domain.Entities;
using Spotrader.Service.Domain.Interfaces.Repositories;
using Spotrader.Service.Domain.ValueObjects;
using Spotrader.Service.Infrastructure.Data.Extensions;
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

    public async Task<BasicStatsDto> GetBasicStatsAsync()
    {
        var processedBets = await _context.Bets
            .Where(bet => bet.Status != BetStatus.OPEN)
            .Select(bet => new
            {
                Amount = bet.Amount,
                ProfitLoss = bet.CalculateProfitLoss()
            })
            .ToListAsync();

        return new BasicStatsDto
        {
            TotalProcessed = processedBets.Count,
            TotalAmount = processedBets.Sum(x => x.Amount),
            TotalProfitLoss = processedBets.Sum(x => x.ProfitLoss)
        };
    }

    public async Task<List<ClientProfitDto>> GetTopClientsWithProfitsAsync(int take = 5)
    {
        return await _context.Bets
            .Where(b => b.Status != BetStatus.OPEN)
            .GroupBy(b => b.Client)
            .Select(g => new ClientProfitDto
            {
                Client = g.Key,
                Profit = g.Sum(bet => bet.CalculateProfitLoss())
            })
            .Where(x => x.Profit > 0)
            .OrderByDescending(x => x.Profit)
            .Take(take)
            .ToListAsync();
    }

    public async Task<List<ClientLossDto>> GetTopClientsWithLossesAsync(int take = 5)
    {
        return await _context.Bets
            .Where(b => b.Status != BetStatus.OPEN)
            .GroupBy(b => b.Client)
            .Select(g => new ClientLossDto
            {
                Client = g.Key,
                Loss = g.Sum(bet => bet.CalculateProfitLoss())
            })
            .Where(x => x.Loss < 0)
            .OrderBy(x => x.Loss)
            .Take(take)
            .ToListAsync();
    }
}
