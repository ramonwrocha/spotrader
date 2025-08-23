using Spotrader.Service.Domain.Entities;

namespace Spotrader.Service.Application.Interfaces;

public interface IBetProcessingService
{
    Task AddBetAsync(Bet bet);
    
    Task<string> GetSummaryAsync();
    
    Task ShutdownAsync();
    
    bool IsShuttingDown { get; }
}
