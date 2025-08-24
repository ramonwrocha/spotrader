using Sportradar.Service.Domain.ValueObjects;
using Sportradar.Service.Infrastructure.Data.Models;

namespace Sportradar.Service.Infrastructure.Data.Extensions;

public static class BetEntityExtensions
{
    public static double CalculateProfitLoss(this BetEntity bet)
    {
        return bet.Status switch
        {
            BetStatus.WINNER => bet.Amount * (bet.Odds - 1),
            BetStatus.LOSER => -bet.Amount,
            BetStatus.VOID => 0.0,
            _ => 0.0
        };
    }
}
