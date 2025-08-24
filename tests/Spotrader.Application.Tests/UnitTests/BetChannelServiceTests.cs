using Spotrader.Service.Application.Services;
using Spotrader.Service.Domain.Entities;

namespace Spotrader.Application.Tests.UnitTests;

public class BetChannelServiceTests
{
    private readonly BetChannelService _sut;

    public BetChannelServiceTests()
    {
        _sut = new BetChannelService();
    }

    [Fact]
    public async Task PublishAsync_ShouldAddBetToChannel()
    {
        // Arrange
        var bet = Bet.Create(100, 2.0, "Client1", "Event1", "Market1", "Selection1");

        // Act
        await _sut.PublishAsync(bet);
        var readBet = await _sut.Reader.ReadAsync();

        // Assert
        Assert.Equal(bet, readBet);
    }

    [Fact]
    public async Task PublishBatchAsync_ShouldAddBetsToBatchChannel()
    {
        // Arrange
        var bets = new[]
        {
            Bet.Create(100, 2.0, "Client1", "Event1", "Market1", "Selection1"),
            Bet.Create(150, 3.0, "Client2", "Event2", "Market2", "Selection2")
        };

        // Act
        await _sut.PublishBatchAsync(bets);
        var readBets = await _sut.BatchReader.ReadAsync();

        // Assert
        Assert.Equal(bets, readBets);
    }

    [Fact]
    public void CompleteAdding_ShouldCompleteBothChannels()
    {
        // Act
        _sut.CompleteAdding();

        // Assert
        Assert.True(_sut.Reader.Completion.IsCompleted);
        Assert.True(_sut.BatchReader.Completion.IsCompleted);
    }
}
