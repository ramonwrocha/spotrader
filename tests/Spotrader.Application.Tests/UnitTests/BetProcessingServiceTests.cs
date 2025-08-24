using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Spotrader.Service.Application.Configuration;
using Spotrader.Service.Application.Interfaces;
using Spotrader.Service.Application.Services;
using Spotrader.Service.Domain.Entities;
using Spotrader.Service.Domain.Interfaces.Repositories;
using Spotrader.Service.Domain.ValueObjects;

namespace Spotrader.Application.Tests.UnitTests;

public class BetProcessingServiceTests
{
    private readonly Mock<IBetRepository> _mockRepository;
    private readonly Mock<IBetChannelService> _mockChannelService;
    private readonly Mock<ILogger<BetProcessingService>> _mockLogger;
    private readonly Mock<IOptions<ApplicationSettings>> _mockOptions;
    private readonly BetProcessingService _sut;

    public BetProcessingServiceTests()
    {
        _mockRepository = new Mock<IBetRepository>();
        _mockChannelService = new Mock<IBetChannelService>();
        _mockLogger = new Mock<ILogger<BetProcessingService>>();
        _mockOptions = new Mock<IOptions<ApplicationSettings>>();

        _mockOptions.Setup(o => o.Value).Returns(new ApplicationSettings
        {
            ProcessingDelayMs = 50
        });

        _sut = new BetProcessingService(
            _mockOptions.Object,
            _mockRepository.Object,
            _mockChannelService.Object,
            _mockLogger.Object
        );
    }

    [Fact]
    public async Task AddBetAsync_ShouldPublishToChannel()
    {
        // Arrange
        var bet = Bet.Create(100, 2.0, "Client1", "Event1", "Market1", "Selection1");

        // Act
        await _sut.AddBetAsync(bet);

        // Assert
        _mockChannelService.Verify(c => c.PublishAsync(bet), Times.Once);
    }

    [Fact]
    public async Task AddBetAsync_WithNullBet_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _sut.AddBetAsync(null!));
    }

    [Fact]
    public async Task AddBetBatchAsync_ShouldPublishBatchToChannel()
    {
        // Arrange
        var bets = new[]
        {
            Bet.Create(100, 2.0, "Client1", "Event1", "Market1", "Selection1"),
            Bet.Create(200, 1.5, "Client2", "Event2", "Market2", "Selection2")
        };

        // Act
        await _sut.AddBetBatchAsync(bets);

        // Assert
        _mockChannelService.Verify(c => c.PublishBatchAsync(bets), Times.Once);
    }

    [Fact]
    public async Task ProcessBetAsync_ValidBet_ShouldUpdateStatusAndSave()
    {
        // Arrange
        var bet = Bet.Create(100, 2.0, "Client1", "Event1", "Market1", "Selection1");

        // Act
        await _sut.ProcessBetAsync(bet);

        // Assert
        Assert.NotEqual(BetStatus.OPEN, bet.Status);
        _mockRepository.Verify(r => r.AddAsync(bet), Times.Once);
    }

    [Fact]
    public async Task ProcessBetAsync_WithInvalidStatus_ShouldThrowException()
    {
        // Arrange
        var bet = Bet.Create(100, 2.0, "Client1", "Event1", "Market1", "Selection1");
        bet.UpdateStatus(BetStatus.WINNER);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.ProcessBetAsync(bet));
    }

    [Fact]
    public async Task ProcessBetBatchAsync_ShouldProcessAllBets()
    {
        // Arrange
        var bets = new[]
        {
            Bet.Create(100, 2.0, "Client1", "Event1", "Market1", "Selection1"),
            Bet.Create(200, 1.5, "Client2", "Event2", "Market2", "Selection2")
        };

        // Act
        await _sut.ProcessBetBatchAsync(bets);

        // Assert
        foreach (var bet in bets)
        {
            Assert.NotEqual(BetStatus.OPEN, bet.Status);
        }
        _mockRepository.Verify(r => r.AddRangeAsync(bets), Times.Once);
    }

    [Fact]
    public async Task ProcessBetBatchAsync_WithEmptyBatch_ShouldNotCallRepository()
    {
        // Arrange
        var emptyBets = Array.Empty<Bet>();

        // Act
        await _sut.ProcessBetBatchAsync(emptyBets);

        // Assert
        _mockRepository.Verify(r => r.AddRangeAsync(It.IsAny<IEnumerable<Bet>>()), Times.Never);
    }

    [Fact]
    public async Task CompleteProcessing_ShouldCallChannelCompleteAdding()
    {
        // Act
        await _sut.CompleteProcessingAsync();

        // Assert
        _mockChannelService.Verify(c => c.CompleteAdding(), Times.Once);
    }

    [Fact]
    public async Task ProcessBetAsync_ShouldRespectProcessingDelay()
    {
        // Arrange
        var bet = Bet.Create(100, 2.0, "Client1", "Event1", "Market1", "Selection1");
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act
        await _sut.ProcessBetAsync(bet);

        // Assert
        stopwatch.Stop();
        Assert.True(stopwatch.ElapsedMilliseconds >= 45); // Al menos cerca de 50ms
    }

    [Fact]
    public async Task ProcessBetAsync_RepositoryThrows_ShouldPropagateException()
    {
        // Arrange
        var bet = Bet.Create(100, 2.0, "Client1", "Event1", "Market1", "Selection1");
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Bet>()))
                      .ThrowsAsync(new InvalidOperationException("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.ProcessBetAsync(bet));
    }
}
