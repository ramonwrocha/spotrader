using Spotrader.Service.Domain.ValueObjects;

namespace Spotrader.Service.Domain.Entities;

public sealed class Bet
{
    public long Id { get; private set; }
    
    public double Amount { get; private set; }

    public double Multiplier { get; private set; }

    public long CustomerId { get; private set; }

    public long EventId { get; private set; }

    public BetStatus Status { get; private set; }

    private Bet() {}

    public static Bet Create(long id, double amount, double multiplier, long customerId, long eventId)
    {
        ValidateBet(amount, multiplier);

        return new Bet
        {
            Id = id,
            Amount = amount,
            CustomerId = customerId,
            Multiplier = multiplier,
            EventId = eventId,
            Status = BetStatus.OPEN
        };
    }

    public void UpdateStatus(BetStatus newStatus)
    {
        ValidateUpdateStatus(newStatus);

        Status = newStatus;
    }

    public double GetResult()
    {
        return Status switch
        {
            BetStatus.WINNER => Amount * Multiplier,
            BetStatus.LOSER => -Amount,
            BetStatus.REFUNDED => Amount,
            _ => 0
        };
    }

    private static void ValidateBet(double amount, double multiplier)
    {
        if (amount <= 0)
        {
            throw new InvalidOperationException("Bet amount must be greater than zero.");
        }

        if (multiplier <= 1)
        {
            throw new InvalidOperationException("Multiplier must be greater than one.");
        }
    }

    private void ValidateUpdateStatus(BetStatus newStatus)
    {
        if (Status != BetStatus.OPEN)
        {
            throw new InvalidOperationException("Bet status can only be updated from OPEN.");
        }

        if (newStatus == BetStatus.OPEN)
        {
            throw new InvalidOperationException("Cannot update status to OPEN.");
        }
    }
}