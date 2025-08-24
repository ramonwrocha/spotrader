using Sportradar.Service.Domain.Entities;
using Sportradar.Service.Infrastructure.Data.Models;

namespace Sportradar.Service.Infrastructure.Data.Mappers;

public static class BetMapper
{
    public static BetEntity ToEntity(this Bet bet) => new()
    {
        Amount = bet.Amount,
        Odds = bet.Odds,
        Client = bet.Client,
        Event = bet.Event,
        Market = bet.Market,
        Selection = bet.Selection,
        Status = bet.Status
    };

    public static Bet ToDomain(this BetEntity entity) => Bet.Read(
        id: entity.Id,
        amount: entity.Amount,
        odds: entity.Odds,
        client: entity.Client,
        @event: entity.Event,
        market: entity.Market,
        status: entity.Status,
        selection: entity.Selection
    );
}
