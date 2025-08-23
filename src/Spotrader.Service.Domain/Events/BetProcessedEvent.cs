using Spotrader.Service.Domain.Entities;
using Spotrader.Service.Domain.ValueObjects;

namespace Spotrader.Service.Domain.Events;

public sealed class BetProcessedEvent
{
    public long BetId { get; private set; }
    public string Client { get; private set; }
    public double Amount { get; private set; }
    public double Odds { get; private set; }
    public BetStatus PreviousStatus { get; private set; }
    public BetStatus NewStatus { get; private set; }
    public double Result { get; private set; }
    public DateTime ProcessedAt { get; private set; }

    private BetProcessedEvent() { }

    public static BetProcessedEvent Create(
        long betId,
        string client,
        double amount,
        double odds,
        BetStatus previousStatus,
        BetStatus newStatus,
        double result)
    {
        ValidateEvent(betId, client, amount, odds, previousStatus, newStatus);

        return new BetProcessedEvent
        {
            BetId = betId,
            Client = client,
            Amount = amount,
            Odds = odds,
            PreviousStatus = previousStatus,
            NewStatus = newStatus,
            Result = result,
            ProcessedAt = DateTime.UtcNow
        };
    }

    public static BetProcessedEvent FromBet(Bet bet, BetStatus previousStatus)
    {
        return Create(
            bet.Id,
            bet.Client!,
            bet.Amount,
            bet.Odds,
            previousStatus,
            bet.Status,
            bet.GetResult()
        );
    }

    private static void ValidateEvent(
        long betId,
        string client,
        double amount,
        double odds,
        BetStatus previousStatus,
        BetStatus newStatus)
    {
        if (betId <= 0)
        {
            throw new InvalidOperationException("BetId must be greater than zero.");
        }

        if (string.IsNullOrEmpty(client))
        {
            throw new InvalidOperationException("Client cannot be null or empty.");
        }

        if (amount <= 0)
        {
            throw new InvalidOperationException("Amount must be greater than zero.");
        }

        if (odds <= 1)
        {
            throw new InvalidOperationException("Odds must be greater than one.");
        }

        if (previousStatus == newStatus)
        {
            throw new InvalidOperationException("Previous and new status cannot be the same.");
        }
    }
}
