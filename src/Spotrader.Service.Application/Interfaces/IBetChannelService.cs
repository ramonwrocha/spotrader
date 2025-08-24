using System.Threading.Channels;
using Spotrader.Service.Domain.Entities;

namespace Spotrader.Service.Application.Interfaces;

public interface IBetChannelService
{
    ValueTask EnqueueAsync(Bet bet);
    ChannelReader<Bet> Reader { get; }
    void CompleteAdding();
}
