using Microsoft.Extensions.Options;
using Spotrader.Service.Application.Configuration;
using Spotrader.Service.Application.DTOs;
using Spotrader.Service.Application.Interfaces;
using Spotrader.Service.Domain.Entities;
using Spotrader.Service.Domain.Interfaces.Repositories;
using Spotrader.Service.Domain.ValueObjects;

namespace Spotrader.Service.Application.Services;

public class BetProcessingService : IBetProcessingService
{
    private readonly IBetRepository _betRepository;
    private readonly IBetChannelService _channelService;
    private readonly ApplicationSettings _settings;

    public BetProcessingService(
        IOptions<ApplicationSettings> settings,
        IBetRepository betRepository,
        IBetChannelService channelService)
    {
        _betRepository = betRepository;
        _channelService = channelService;
        _settings = settings.Value;
    }

    public async Task AddBetAsync(Bet bet)
    {
        ArgumentNullException.ThrowIfNull(bet);
        
        await _channelService.PublishAsync(bet);
    }

    public async Task AddBetBatchAsync(List<Bet> bets)
    {
        ArgumentNullException.ThrowIfNull(bets);

        var publishTasks = bets.Select(_channelService.PublishAsync);

        await Task.WhenAll(publishTasks);
    }

    public async Task ProcessBetAsync(Bet bet)
    {
        if (bet.Status != BetStatus.OPEN)
        {
            throw new InvalidOperationException($"Bet {bet.Id} has invalid status {bet.Status} for processing");
        }

        await Task.Delay(_settings.ProcessingDelayMs);

        bet.UpdateStatus(SimulateRandomResult());

        await _betRepository.AddAsync(bet);
    }

    public async Task ProcessBetBatchAsync(IEnumerable<Bet> bets)
    {
        var processedBets = new List<Bet>(capacity: _settings.MaxBetBatchSize);

        await Task.Delay(_settings.ProcessingDelayMs);

        foreach (var bet in bets)
        {
            if (bet.Status != BetStatus.OPEN)
            {
                continue;
            }

            bet.UpdateStatus(SimulateRandomResult());

            processedBets.Add(bet);
        }

        if (processedBets.Any())
        {
            await _betRepository.AddRangeAsync(processedBets);
        }
    }

    public async Task<BetSummary> GetSummaryAsync()
    {
        var basicStats = await _betRepository.GetBasicStatsAsync();
        var topProfits = await _betRepository.GetTopClientsWithProfitsAsync(take: 10);
        var topLosses = await _betRepository.GetTopClientsWithLossesAsync(take: 10);

        return new BetSummary
        {
            TotalBets = basicStats.TotalProcessed,
            TotalAmount = (decimal)basicStats.TotalAmount,
            TotalWinnings = (decimal)basicStats.TotalProfitLoss,
            TopClientsByProfit = topProfits.Select(p => new ClientProfitInfo 
            { 
                Client = p.Client, 
                TotalProfit = (decimal)p.Profit 
            }).ToList(),
            TopClientsByLoss = topLosses.Select(l => new ClientLossInfo 
            { 
                Client = l.Client, 
                TotalLoss = (decimal)l.Loss 
            }).ToList()
        };
    }

    public void CompleteProcessing()
    {
        _channelService.CompleteAdding();
    }

    private BetStatus SimulateRandomResult()
    {
        var random = Random.Shared.NextDouble();

        if (random <= _settings.WinnerThreshold)
        {
            return BetStatus.WINNER;
        }

        if (random <= _settings.LoserThreshold)
        {
            return BetStatus.LOSER;
        }

        return BetStatus.VOID;
    }

    
}
