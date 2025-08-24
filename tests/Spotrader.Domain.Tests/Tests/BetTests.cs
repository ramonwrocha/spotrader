using Spotrader.Service.Domain.Entities;
using Spotrader.Service.Domain.ValueObjects;

namespace Spotrader.Domain.Tests.Tests;

public class BetTests
{
    [Fact]
    public void Create_WithValidParameters_ShouldCreateBetWithOpenStatus()
    {
        // Act
        var bet = Bet.Create(100.0, 2.5, "Client1", "Event1", "Market1", "Selection1");

        // Assert
        Assert.Equal(BetStatus.OPEN, bet.Status);
        Assert.Equal(100.0, bet.Amount);
        Assert.Equal(2.5, bet.Odds);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_WithInvalidAmount_ShouldThrowException(double invalidAmount)
    {
        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            Bet.Create(invalidAmount, 2.5, "Client1", "Event1", "Market1", "Selection1"));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(0)]
    public void Create_WithInvalidOdds_ShouldThrowException(double invalidOdds)
    {
        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            Bet.Create(100.0, invalidOdds, "Client1", "Event1", "Market1", "Selection1"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Create_WithInvalidClient_ShouldThrowException(string invalidClient)
    {
        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            Bet.Create(100.0, 2.5, invalidClient, "Event1", "Market1", "Selection1"));
    }

    [Fact]
    public void UpdateStatus_FromOpenToWinner_ShouldUpdateSuccessfully()
    {
        // Arrange
        var bet = Bet.Create(100.0, 2.5, "Client1", "Event1", "Market1", "Selection1");

        // Act
        bet.UpdateStatus(BetStatus.WINNER);

        // Assert
        Assert.Equal(BetStatus.WINNER, bet.Status);
    }

    [Fact]
    public void UpdateStatus_FromWinnerToLoser_ShouldThrowException()
    {
        // Arrange
        var bet = Bet.Read(1L, 100.0, 2.5, "Client1", "Event1", "Market1", BetStatus.WINNER, "Selection1");

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => bet.UpdateStatus(BetStatus.LOSER));
    }

    [Fact]
    public void UpdateStatus_ToOpen_ShouldThrowException()
    {
        // Arrange
        var bet = Bet.Create(100.0, 2.5, "Client1", "Event1", "Market1", "Selection1");

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => bet.UpdateStatus(BetStatus.OPEN));
    }

    [Fact]
    public void GetResult_WhenWinner_ShouldReturnProfit()
    {
        // Arrange
        var bet = Bet.Create(100.0, 2.5, "Client1", "Event1", "Market1", "Selection1");
        bet.UpdateStatus(BetStatus.WINNER);

        // Act
        var result = bet.GetResult();

        // Assert
        Assert.Equal(150.0, result); // 100 * (2.5 - 1)
    }

    [Fact]
    public void GetResult_WhenLoser_ShouldReturnNegativeAmount()
    {
        // Arrange
        var bet = Bet.Create(100.0, 2.5, "Client1", "Event1", "Market1", "Selection1");
        bet.UpdateStatus(BetStatus.LOSER);

        // Act
        var result = bet.GetResult();

        // Assert
        Assert.Equal(-100.0, result);
    }

    [Fact]
    public void GetResult_WhenOpen_ShouldReturnZero()
    {
        // Arrange
        var bet = Bet.Create(100.0, 2.5, "Client1", "Event1", "Market1", "Selection1");

        // Act
        var result = bet.GetResult();

        // Assert
        Assert.Equal(0, result);
    }
}
