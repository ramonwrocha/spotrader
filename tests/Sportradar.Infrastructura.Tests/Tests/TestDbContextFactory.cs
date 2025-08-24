using Microsoft.EntityFrameworkCore;
using Sportradar.Service.Infrastructure.Data;
namespace Sportradar.Infrastructura.Tests.Tests;

public class TestDbContextFactory : IDbContextFactory<SportradarDbContext>
{
    private readonly DbContextOptions<SportradarDbContext> _options;

    public TestDbContextFactory(DbContextOptions<SportradarDbContext> options)
    {
        _options = options;
    }

    public SportradarDbContext CreateDbContext() => new SportradarDbContext(_options);
    public Task<SportradarDbContext> CreateDbContextAsync() => Task.FromResult(CreateDbContext());
}
