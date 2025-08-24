using System.Threading.Channels;
using Spotrader.Service.Application.Interfaces;
using Spotrader.Service.Domain.Entities;

namespace Spotrader.Service.Application.Services;

public class BetChannelService : IBetChannelService
{
    private readonly Channel<Bet> _channel;
    private readonly ChannelWriter<Bet> _writer;

    public BetChannelService()
    {
        var options = new UnboundedChannelOptions
        {
            SingleReader = false,
            SingleWriter = false,
            AllowSynchronousContinuations = false
        };

        _channel = Channel.CreateUnbounded<Bet>(options);
        _writer = _channel.Writer;
    }

    public ChannelReader<Bet> Reader => _channel.Reader;

    public async ValueTask EnqueueAsync(Bet bet)
    {
        ArgumentNullException.ThrowIfNull(bet);
        await _writer.WriteAsync(bet);
    }

    public void CompleteAdding()
    {
        _writer.Complete();
    }
}
