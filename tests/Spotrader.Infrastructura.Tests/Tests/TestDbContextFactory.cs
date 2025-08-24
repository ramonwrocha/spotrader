using Microsoft.EntityFrameworkCore;
using Spotrader.Service.Infrastructure.Data;
namespace Spotrader.Infrastructura.Tests.Tests;

public class TestDbContextFactory : IDbContextFactory<SpotraderDbContext>
{
    private readonly DbContextOptions<SpotraderDbContext> _options;

    public TestDbContextFactory(DbContextOptions<SpotraderDbContext> options)
    {
        _options = options;
    }

    public SpotraderDbContext CreateDbContext() => new SpotraderDbContext(_options);
    public Task<SpotraderDbContext> CreateDbContextAsync() => Task.FromResult(CreateDbContext());
}
