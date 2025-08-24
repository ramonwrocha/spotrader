using Sportradar.Service.Application.Interfaces;
using Sportradar.Service.Domain.Entities;
using System.Threading.Channels;

namespace Sportradar.Service.Application.Services;

public class BetChannelService : IBetChannelService
{
    private readonly Channel<Bet> _singleChannel;
    private readonly Channel<IEnumerable<Bet>> _batchChannel;

    private readonly ChannelWriter<Bet> _writer;

    public BetChannelService()
    {
        var options = new BoundedChannelOptions(50_000)
        {
            SingleReader = true,
            SingleWriter = false,
            AllowSynchronousContinuations = true
        };

        _singleChannel = Channel.CreateBounded<Bet>(options);
        _writer = _singleChannel.Writer;

        _batchChannel = Channel.CreateBounded<IEnumerable<Bet>>(options);
    }

    public ChannelReader<Bet> Reader => _singleChannel.Reader;

    public ChannelReader<IEnumerable<Bet>> BatchReader => _batchChannel.Reader;

    public void CompleteAdding()
    {
        _singleChannel.Writer.Complete();
        _batchChannel.Writer.Complete();
    }

    public async Task PublishAsync(Bet bet)
    {
        ArgumentNullException.ThrowIfNull(bet);

        await _writer.WriteAsync(bet);
    }

    public async Task PublishBatchAsync(IEnumerable<Bet> bets)
    {
        await _batchChannel.Writer.WriteAsync(bets);
    }
}
