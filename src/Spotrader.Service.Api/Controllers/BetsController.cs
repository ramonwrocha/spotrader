using Microsoft.AspNetCore.Mvc;
using Spotrader.Service.Application.DTOs;
using Spotrader.Service.Application.Services;
using Spotrader.Service.Domain.Entities;

namespace Spotrader.Service.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BetsController : ControllerBase
{
    private readonly BetProcessingService _betProcessingService;

    public BetsController(BetProcessingService betProcessingService)
    {
        _betProcessingService = betProcessingService;
    }

    [HttpPost]
    public async Task<IActionResult> AddBet([FromBody] CreateBetRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var bet = Bet.Create(
                request.Amount,
                request.Odds,
                request.Client,
                request.Event,
                request.Market,
                request.Selection);

            await _betProcessingService.AddBetAsync(bet);

            return Ok(new { BetId = bet.Id, Message = "Bet queued for processing" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Error = "Failed to queue bet for processing" });
        }
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary()
    {
        try
        {
            var summary = await _betProcessingService.GetSummaryAsync();
            return Ok(summary);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Error = "Failed to retrieve system summary" });
        }
    }
}
