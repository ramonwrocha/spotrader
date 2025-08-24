using Microsoft.AspNetCore.Mvc;
using Sportradar.Service.Worker.Services;
using Spotrader.Service.Application.DTOs;
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

    [HttpPost("seed-data/bets")]
    public async Task<IActionResult> SeedData([FromQuery] SeedBetsParams param)
    {
        await _dataSeedingService.SeedInitialBetsAsync(totalBets: param.TotalBets);

        return Ok(new { Message = $"Successfully seeded {param.TotalBets} initial bets" });
    }

    [HttpPost("shutdown")]
    public IActionResult ShutdownSystem()
    {
        _channelService.CompleteAdding();

        _applicationLifetime.StopApplication();

        return Ok(new { Message = "System shutdown initiated" });
    }
}
