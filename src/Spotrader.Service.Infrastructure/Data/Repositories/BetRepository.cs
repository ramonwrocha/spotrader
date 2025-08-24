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
    private readonly IDbContextFactory<SpotraderDbContext> _contextFactory;

    public BetRepository(IDbContextFactory<SpotraderDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<Bet?> GetByIdAsync(long id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var entity = await context.Bets.FindAsync(id);
        return entity?.ToDomain();
    }

    public async Task AddAsync(Bet bet)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        await context.Bets.AddAsync(bet.ToEntity());

        await context.SaveChangesAsync();
    }

    public async Task AddRangeAsync(IEnumerable<Bet> bets)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        await context.Bets.AddRangeAsync(bets.Select(bet => bet.ToEntity()));

        await context.SaveChangesAsync();
    }

    public async Task<BasicStatsDto> GetBasicStatsAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var simpleStats = await context.Bets
            .Where(b => b.Status != BetStatus.OPEN)
            .GroupBy(b => 1)
            .Select(g => new
            {
                TotalProcessed = g.Count(),
                TotalAmount = g.Sum(b => b.Amount)
            })
            .FirstOrDefaultAsync();

        var profitLossData = await context.Bets
            .Where(b => b.Status != BetStatus.OPEN)
            .Select(b => new { b.Status, b.Amount, b.Odds })
            .ToListAsync();

        var totalProfitLoss = profitLossData
            .AsParallel()
            .Sum(b => CalculateProfitLossStatic(b.Status, b.Amount, b.Odds));

        return new BasicStatsDto
        {
            TotalProcessed = simpleStats?.TotalProcessed ?? 0,
            TotalAmount = simpleStats?.TotalAmount ?? 0.0,
            TotalProfitLoss = (double)totalProfitLoss
        };
    }

    public async Task<List<ClientProfitDto>> GetTopClientsWithProfitsAsync(int take = 5)
    {
        var clientProfits = await GetClientProfitsAsync();
        
        return clientProfits
            .Where(x => x.Profit > 0)
            .OrderByDescending(x => x.Profit)
            .Take(take)
            .ToList();
    }

    public async Task<List<ClientLossDto>> GetTopClientsWithLossesAsync(int take = 5)
    {
        var clientProfits = await GetClientProfitsAsync();
        
        return clientProfits
            .Where(x => x.Profit < 0)
            .Select(x => new ClientLossDto { Client = x.Client, Loss = x.Profit })
            .OrderBy(x => x.Loss)
            .Take(take)
            .ToList();
    }

    private static decimal CalculateProfitLossStatic(BetStatus status, double amount, double odds)
    {
        return status switch
        {
            BetStatus.WINNER => (decimal)((amount * odds) - amount),
            BetStatus.LOSER => -(decimal)amount,
            _ => 0m
        };
    }
    
    private async Task<List<ClientProfitDto>> GetClientProfitsAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var clientBetsData = await context.Bets
            .Where(b => b.Status != BetStatus.OPEN)
            .Select(b => new { b.Client, b.Status, b.Amount, b.Odds })
            .ToListAsync();

        return clientBetsData
            .GroupBy(b => b.Client)
            .Select(g => new ClientProfitDto
            {
                Client = g.Key,
                Profit = (double)g.Sum(bet => CalculateProfitLossStatic(bet.Status, bet.Amount, bet.Odds))
            })
            .ToList();
    }
}
