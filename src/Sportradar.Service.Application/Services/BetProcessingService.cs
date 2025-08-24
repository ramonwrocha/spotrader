using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sportradar.Service.Application.Configuration;
using Sportradar.Service.Application.Extensions;
using Sportradar.Service.Application.Interfaces;
using Sportradar.Service.Domain.Entities;
using Sportradar.Service.Domain.Interfaces.Repositories;
using Sportradar.Service.Domain.ValueObjects;

namespace Sportradar.Service.Application.Services;

public class BetProcessingService : IBetProcessingService
{
    private readonly IBetRepository _betRepository;
    private readonly IBetChannelService _channelService;
    private readonly ApplicationSettings _settings;

    private readonly ILogger<BetProcessingService> _logger;

    public BetProcessingService(
        IOptions<ApplicationSettings> settings,
        IBetRepository betRepository,
        IBetChannelService channelService,
        ILogger<BetProcessingService> logger)
    {
        _betRepository = betRepository;
        _channelService = channelService;
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task AddBetAsync(Bet bet)
    {
        ArgumentNullException.ThrowIfNull(bet);

        await _channelService.PublishAsync(bet);
    }

    public async Task AddBetBatchAsync(IEnumerable<Bet> bets)
    {
        ArgumentNullException.ThrowIfNull(bets);

        await _channelService.PublishBatchAsync(bets);
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

    public async Task<string> GetSummaryAsync()
    {
        var basicStats = await _betRepository.GetBasicStatsAsync();
        var topProfits = await _betRepository.GetTopClientsWithProfitsAsync(take: 5);
        var topLosses = await _betRepository.GetTopClientsWithLossesAsync(take: 5);

        return basicStats.ToSummary(topProfits, topLosses);
    }

    public async Task CompleteProcessingAsync()
    {
        _channelService.CompleteAdding();

        await Task.CompletedTask;
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
