using Microsoft.Extensions.Options;
using Spotrader.Service.Application.Configuration;
using Spotrader.Service.Application.Interfaces;
using Spotrader.Service.Domain.Entities;

namespace Spotrader.Service.Api.Services;

public class DataSeedingService
{
    private readonly IBetProcessingService _betProcessingService;
    private readonly int _threadsPerCpu = 2; // cantidad recomendado por MS
    private readonly int _minimalAmount = 100;
    private readonly int _maxThreadsCount = 30;
    private readonly int _maxBets = 5_000_000;

    private readonly ApplicationSettings _appSettings;

    public DataSeedingService(IBetProcessingService betProcessingService, IOptions<ApplicationSettings> appSettings)
    {
        _betProcessingService = betProcessingService;
        _appSettings = appSettings.Value;
    }

    public async Task SeedInitialBetsAsync(long totalBets)
    {
        totalBets = Math.Min(totalBets, _maxBets);

        var dummyClients = new[]
        {
            "Client_A", "Client_B", "Client_C", "Client_D", "Client_E",
            "Client_F", "Client_G", "Client_H", "Client_I", "Client_J"
        };

        var dummyEvents = new[] { "Football", "Basketball" };
        var markets = new[] { "Match_Winner" };
        var selections = new[] { "Home_Team", "Away_Team" };

        var bets = GenerateBets(totalBets, dummyClients, dummyEvents, markets, selections);

        var batches = bets
            .Select((bet, index) => new { bet, index })
            .GroupBy(x => x.index / _appSettings.MaxBetBatchSize)
            .Select(g => g.Select(x => x.bet).ToList())
            .ToList();

        var optimalThreads = OptimalThreadsCount();

        using var semaphore = new SemaphoreSlim(optimalThreads, optimalThreads);
        
        var tasks = batches.Select(async (batch, batchIndex) =>
        {
            await semaphore.WaitAsync();

            try
            {
                await _betProcessingService.AddBetBatchAsync(batch);
            }
            catch
            {
                throw;
            }
            finally
            {
                semaphore.Release();
            }
        });

        await Task.WhenAll(tasks);
    }

    private int OptimalThreadsCount()
    {
        var logicsCpus = Environment.ProcessorCount * _threadsPerCpu;

        return Math.Min(logicsCpus, _maxThreadsCount);
    }

    private IEnumerable<Bet> GenerateBets(
        long totalBets, string[] clients, string[] events, string[] markets, string[] selections)
    {
        var random = new Random();
        for (var i = 1; i <= totalBets; i++)
        {
            var amount = _minimalAmount + random.Next(0, 300);

            yield return Bet.Create(
                amount: amount,
                odds: 2.0,
                client: clients[i % clients.Length],
                @event: events[i % events.Length],
                market: markets[0],
                selection: selections[i % selections.Length]
            );
        }
    }
}
