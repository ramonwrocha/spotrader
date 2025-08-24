using Sportradar.Service.Domain.ValueObjects;

namespace Sportradar.Service.Domain.Entities;

public sealed class Bet
{
    public long Id { get; private set; }

    public double Amount { get; private set; }

    public double Odds { get; private set; }

    public string Client { get; private set; }

    public string Event { get; private set; }

    public string Market { get; private set; }

    public string Selection { get; private set; }

    public BetStatus Status { get; private set; }

    private Bet() { }

    public static Bet Create(
        double amount,
        double odds,
        string client,
        string @event,
        string market,
        string selection)
    {
        ValidateBet(amount, odds, client, @event, market, selection);

        return new Bet
        {
            Amount = amount,
            Odds = odds,
            Client = client,
            Event = @event,
            Market = market,
            Selection = selection,
            Status = BetStatus.OPEN
        };
    }

    public static Bet Read(
        long id,
        double amount,
        double odds,
        string client,
        string @event,
        string market,
        BetStatus status,
        string selection)
    {
        return new Bet
        {
            Id = id,
            Amount = amount,
            Odds = odds,
            Client = client,
            Event = @event,
            Market = market,
            Selection = selection,
            Status = status
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
            BetStatus.WINNER => Amount * (Odds - 1),
            BetStatus.LOSER => -Amount,
            BetStatus.VOID => 0,
            _ => 0
        };
    }

    private static void ValidateBet(double amount, double odds, string client, string @event, string market, string selection)
    {
        if (amount <= 0)
        {
            throw new InvalidOperationException("Bet amount must be greater than zero.");
        }

        if (odds <= 1)
        {
            throw new InvalidOperationException("Odds must be greater than one.");
        }

        if (string.IsNullOrEmpty(client))
        {
            throw new InvalidOperationException("Client cannot be null or empty.");
        }

        if (string.IsNullOrEmpty(@event))
        {
            throw new InvalidOperationException("Event cannot be null or empty.");
        }

        if (string.IsNullOrEmpty(market))
        {
            throw new InvalidOperationException("Market cannot be null or empty.");
        }

        if (string.IsNullOrEmpty(selection))
        {
            throw new InvalidOperationException("Selection cannot be null or empty.");
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