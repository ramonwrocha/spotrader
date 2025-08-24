using System.Threading.Channels;
using Spotrader.Service.Domain.Entities;

namespace Spotrader.Service.Application.Interfaces;

public interface IBetChannelService
{
    Task PublishAsync(Bet bet);
    
    Task PublishBatchAsync(IEnumerable<Bet> bets);

    ChannelReader<Bet> Reader { get; }
    
    ChannelReader<IEnumerable<Bet>> BatchReader { get; }

    void CompleteAdding();
}
