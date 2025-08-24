using Microsoft.AspNetCore.Mvc;
using Sportradar.Service.Application.DTOs;
using Sportradar.Service.Application.Interfaces;
using Sportradar.Service.Domain.Entities;

namespace Sportradar.Service.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BetsController : ControllerBase
{
    private readonly IBetProcessingService _betProcessingService;

    public BetsController(IBetProcessingService betProcessingService)
    {
        _betProcessingService = betProcessingService;
    }

    [HttpPost]
    public async Task<IActionResult> AddBet([FromBody] CreateBetRequest request)
    {
        var bet = Bet.Create(
                request.Amount,
                request.Odds,
                request.Client,
                request.Event,
                request.Market,
                request.Selection);

        await _betProcessingService.ProcessBetAsync(bet);

        return Ok();
    }

    [HttpGet("Summary")]
    public async Task<IActionResult> GetSummary()
    {
        var summary = await _betProcessingService.GetSummaryAsync();

        return Ok(summary);
    }
}
