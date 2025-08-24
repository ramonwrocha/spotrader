using Spotrader.Service.Application.Interfaces;
using Spotrader.Service.Domain.Entities;

namespace Sportradar.Service.Worker.Services;

public class DataSeedingService
{
    private readonly IBetProcessingService _betProcessingService;

    private readonly int _bathSize = 1000; // MaxBetBatchSize
    private readonly int _threadsPerCpu = 2; // cantidad recomendado por MS
    private readonly int _minimalAmount = 100;
    private readonly int _maxThreadsCount = 30;
    private readonly int _maxBets = 5_000_000;

    public DataSeedingService(IBetProcessingService betProcessingService)
    {
        _betProcessingService = betProcessingService;
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
            .GroupBy(x => x.index / _bathSize)
            .Select(g => g.Select(x => x.bet).ToList())
            .ToList();

        await Parallel.ForEachAsync(
            source: batches,
            parallelOptions: new ParallelOptions
            {
                MaxDegreeOfParallelism = OptimalThreadsCount()
            },
            body: async (batch, ct) =>
            {
                await _betProcessingService.AddBetBatchAsync(batch);
            });
    }

    private int OptimalThreadsCount()
    {
        return Math.Min(Environment.ProcessorCount * _threadsPerCpu, _maxThreadsCount);
    }

    private IEnumerable<Bet> GenerateBets(
        long totalBets, string[] clients, string[] events, string[] markets, string[] selections)
    {
        var random = new Random();
        for (int i = 1; i <= totalBets; i++)
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
