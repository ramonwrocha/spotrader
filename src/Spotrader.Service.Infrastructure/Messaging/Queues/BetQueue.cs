using Spotrader.Service.Domain.Entities;
using Spotrader.Service.Domain.Interfaces.Queue;
using System.Threading.Channels;

namespace Spotrader.Service.Infrastructure.Messaging.Queues;

public class BetQueue : IBetQueue
{
    private readonly Channel<Bet> _channel;
    
    private readonly ChannelWriter<Bet> _writer;
    
    private readonly ChannelReader<Bet> _reader;

    public BetQueue()
    {
        var options = new UnboundedChannelOptions
        {
            SingleReader = false,
            SingleWriter = false,
            AllowSynchronousContinuations = false
        };

        _channel = Channel.CreateUnbounded<Bet>(options);
        _writer = _channel.Writer;
        _reader = _channel.Reader;
    }

    public async Task EnqueueAsync(Bet bet)
    {
        ArgumentNullException.ThrowIfNull(bet);

        if (!_writer.TryWrite(bet))
        {
            await _writer.WriteAsync(bet);
        }
    }

    public async Task<Bet?> DequeueAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (await _reader.WaitToReadAsync(cancellationToken))
            {
                if (_reader.TryRead(out var bet))
                {
                    return bet;
                }
            }
        }
        catch (OperationCanceledException)
        {
            return null;
        }

        return null;
    }

    public int Count => _reader.CanCount ? _reader.Count : 0;

    public bool IsEmpty => Count == 0;
}
