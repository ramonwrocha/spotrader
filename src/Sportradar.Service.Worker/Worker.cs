namespace Sportradar.Service.Worker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IBetChannelService _channelService;

    public Worker(IServiceScopeFactory scopeFactory, IBetChannelService channelService)
    {
        _channelService = channelService;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var bet in _channelService.Reader.ReadAllAsync(stoppingToken))
        {
            using var scope = _scopeFactory.CreateScope();
            var betProcessingService = scope.ServiceProvider.GetRequiredService<BetProcessingService>();

            try
            {
                await betProcessingService.ProcessBetAsync(bet);
                _logger.LogInformation("Processed bet {BetId} for client {Client}", bet.Id, bet.Client);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing bet {BetId}: {Error}", bet.Id, ex.Message);
            }
        }
    }
}
