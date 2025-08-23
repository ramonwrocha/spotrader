using Spotrader.Service.Domain.Entities;

namespace Spotrader.Service.Domain.Interfaces.Queue;

public interface IBetQueue
{
    Task EnqueueAsync(Bet bet);

    Task<Bet?> DequeueAsync(CancellationToken cancellationToken = default);

    int Count { get; }

    bool IsEmpty { get; }
}
