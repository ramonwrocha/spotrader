using Microsoft.EntityFrameworkCore;
using Spotrader.Service.Domain.Entities;
using Spotrader.Service.Infrastructure.Data;
using Spotrader.Service.Infrastructure.Data.Repositories;

namespace Spotrader.Infrastructura.Tests.Tests;

public class BetRepositoryTests : IDisposable
{
    private readonly DbContextOptions<SpotraderDbContext> _options;

    public BetRepositoryTests()
    {
        _options = new DbContextOptionsBuilder<SpotraderDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public async Task AddAsync_ShouldPersistBetCorrectly()
    {
        // Arrange
        var factory = new TestDbContextFactory(_options);
        var repository = new BetRepository(factory);
        var bet = Bet.Create(100, 2.0, "Client1", "Event1", "Market1", "Selection1");

        // Act
        await repository.AddAsync(bet);

        // Assert
        using var context = new SpotraderDbContext(_options);
        var saved = await context.Bets.FirstOrDefaultAsync();
        Assert.NotNull(saved);
        Assert.Equal("Client1", saved.Client);
        Assert.Equal(100m, (decimal)saved.Amount);
    }

    [Fact]
    public async Task AddRangeAsync_ShouldPersistMultipleBets()
    {
        // Arrange
        var factory = new TestDbContextFactory(_options);
        var repository = new BetRepository(factory);
        var bets = new[]
        {
            Bet.Create(100, 2.0, "Client1", "Event1", "Market1", "Selection1"),
            Bet.Create(100, 2.0, "Client2", "Event2", "Market2", "Selection2")
        };

        // Act
        await repository.AddRangeAsync(bets);

        // Assert
        using var context = new SpotraderDbContext(_options);
        var count = await context.Bets.CountAsync();
        Assert.Equal(2, count);
    }

    public void Dispose()
    {
        using var context = new SpotraderDbContext(_options);
        context.Database.EnsureDeleted();
    }
}
