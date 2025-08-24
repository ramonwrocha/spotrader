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

    public SystemController(
        IBetChannelService channelService,
        IHostApplicationLifetime applicationLifetime,
        DataSeedingService dataSeedingService)
    {
        _channelService = channelService;
        _applicationLifetime = applicationLifetime;
        _dataSeedingService = dataSeedingService;
    }

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
            return StatusCode(500, new { Error = "Failed to seed initial data" });
        }
    }

    [HttpPost("shutdown")]
    public IActionResult ShutdownSystem()
    {
        try
        {
            _channelService.CompleteAdding();

            _applicationLifetime.StopApplication();

            return Ok(new { Message = "System shutdown initiated" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Error = "Failed to initiate system shutdown" });
        }
    }
}
