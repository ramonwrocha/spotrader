using Microsoft.AspNetCore.Mvc;
using Spotrader.Service.Application.DTOs;
using Spotrader.Service.Application.Interfaces;
using Spotrader.Service.Application.Services;
using Spotrader.Service.Domain.Entities;

namespace Spotrader.Service.Api.Controllers;

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

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary()
    {
        var summary = await _betProcessingService.GetSummaryAsync();

        return Ok(summary);
    }

    [HttpPost("shutdown")]
    public IActionResult Shutdown()
    {
        _betProcessingService.CompleteProcessing();

        return Ok("System shutdown initiated");
    }
}
