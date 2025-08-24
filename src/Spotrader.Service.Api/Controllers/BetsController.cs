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
    private readonly ILogger<BetsController> _logger;

    public BetsController(
        BetProcessingService betProcessingService,
        ILogger<BetsController> logger)
    {
        _betProcessingService = betProcessingService;
        _logger = logger;
    }

    /// <summary>
    /// Adds a new bet to the processing queue
    /// </summary>
    /// <param name="request">Bet creation request</param>
    /// <returns>Created bet confirmation</returns>
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

            _logger.LogInformation("Bet queued for processing: {BetId} for client {Client}", 
                bet.Id, bet.Client);

            return Ok(new { BetId = bet.Id, Message = "Bet queued for processing" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding bet: {Error}", ex.Message);
            return StatusCode(500, new { Error = "Failed to queue bet for processing" });
        }
    }

    /// <summary>
    /// Gets the current system summary with betting statistics
    /// </summary>
    /// <returns>System summary with betting statistics</returns>
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
            _logger.LogError(ex, "Error getting summary: {Error}", ex.Message);
            return StatusCode(500, new { Error = "Failed to retrieve system summary" });
        }
    }
}
