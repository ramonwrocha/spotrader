using Microsoft.AspNetCore.Mvc;
using Spotrader.Service.Api.Services;
using Spotrader.Service.Application.Interfaces;

namespace Spotrader.Service.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SystemController : ControllerBase
{
    private readonly IBetChannelService _channelService;
    private readonly IHostApplicationLifetime _applicationLifetime;
    private readonly DataSeedingService _dataSeedingService;
    private readonly ILogger<SystemController> _logger;

    public SystemController(
        IBetChannelService channelService,
        IHostApplicationLifetime applicationLifetime,
        DataSeedingService dataSeedingService,
        ILogger<SystemController> logger)
    {
        _channelService = channelService;
        _applicationLifetime = applicationLifetime;
        _dataSeedingService = dataSeedingService;
        _logger = logger;
    }

    /// <summary>
    /// Seeds the system with 100 initial bets for testing
    /// </summary>
    /// <returns>Seeding confirmation</returns>
    [HttpPost("seed-data")]
    public async Task<IActionResult> SeedData()
    {
        try
        {
            await _dataSeedingService.SeedInitialBetsAsync();
            return Ok(new { Message = "Successfully seeded 100 initial bets" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error seeding data: {Error}", ex.Message);
            return StatusCode(500, new { Error = "Failed to seed initial data" });
        }
    }

    /// <summary>
    /// Initiates graceful shutdown of the betting system
    /// </summary>
    /// <returns>Shutdown confirmation</returns>
    [HttpPost("shutdown")]
    public IActionResult ShutdownSystem()
    {
        try
        {
            _logger.LogInformation("System shutdown requested");

            // Complete the channel to stop accepting new bets
            _channelService.CompleteAdding();

            // Initiate application shutdown
            _applicationLifetime.StopApplication();

            return Ok(new { Message = "System shutdown initiated" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during system shutdown: {Error}", ex.Message);
            return StatusCode(500, new { Error = "Failed to initiate system shutdown" });
        }
    }
}
