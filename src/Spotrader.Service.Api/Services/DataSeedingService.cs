using Spotrader.Service.Application.Services;
using Spotrader.Service.Domain.Entities;

namespace Spotrader.Service.Api.Services;

public class DataSeedingService
{
    private readonly BetProcessingService _betProcessingService;

    public DataSeedingService(BetProcessingService betProcessingService)
    {
        _betProcessingService = betProcessingService;
    }

    public async Task SeedInitialBetsAsync()
    {
        var clients = new[] { "Client_A", "Client_B" };
        var events = new[] { "Football", "Basketball" };
        var markets = new[] { "Match_Winner" };
        var selections = new[] { "Home_Team", "Away_Team" };

        for (int i = 1; i <= 5; i++)
        {
            var bet = Bet.Create(
                amount: 100 + i,
                odds: 2.0,
                client: clients[i % clients.Length],
                @event: events[i % events.Length],
                market: markets[0],
                selection: selections[i % selections.Length]
            );

            await _betProcessingService.AddBetAsync(bet);
        }
    }
}
