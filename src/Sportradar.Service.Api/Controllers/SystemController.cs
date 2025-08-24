using Microsoft.AspNetCore.Mvc;
using Sportradar.Service.Api.Services;
using Sportradar.Service.Application.DTOs;
using Sportradar.Service.Application.Interfaces;

namespace Sportradar.Service.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SystemController : ControllerBase
{
    private readonly IHostApplicationLifetime _applicationLifetime;
    private readonly DataSeedingService _dataSeedingService;
    private readonly IBetProcessingService _betProcessingService;

    public SystemController(
        IHostApplicationLifetime applicationLifetime,
        DataSeedingService dataSeedingService,
        IBetProcessingService betProcessingService)
    {
        _applicationLifetime = applicationLifetime;
        _dataSeedingService = dataSeedingService;
        _betProcessingService = betProcessingService;
    }

    [HttpPost("SeedDataBets")]
    public async Task<IActionResult> SeedData([FromQuery] SeedBetsParams param)
    {
        await _dataSeedingService.SeedInitialBetsAsync(totalBets: param.TotalBets);

        return Ok(new { Message = $"Successfully seeded {param.TotalBets} initial bets" });
    }

    [HttpPost("Shutdown")]
    public IActionResult ShutdownSystem()
    {
        _betProcessingService.CompleteProcessingAsync();
        _applicationLifetime.StopApplication();

        return Ok(new { Message = "System shutdown initiated" });
    }
}
