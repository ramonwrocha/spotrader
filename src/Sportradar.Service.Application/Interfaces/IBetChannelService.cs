using Sportradar.Service.Domain.Entities;
using System.Threading.Channels;

namespace Sportradar.Service.Application.Interfaces;

public interface IBetChannelService
{
    Task PublishAsync(Bet bet);

    Task PublishBatchAsync(IEnumerable<Bet> bets);

    ChannelReader<Bet> Reader { get; }

    ChannelReader<IEnumerable<Bet>> BatchReader { get; }

    void CompleteAdding();
}
