using Spotrader.Service.Application.Interfaces;
using Spotrader.Service.Domain.Entities;

namespace Spotrader.Service.Application.Services;

public class BetProcessingService : IBetProcessingService
{
    public bool IsShuttingDown => throw new NotImplementedException();

    public Task AddBetAsync(Bet bet)
    {
        throw new NotImplementedException();
    }

    public Task<string> GetSummaryAsync()
    {
        throw new NotImplementedException();
    }

    public Task ShutdownAsync()
    {
        throw new NotImplementedException();
    }
}
