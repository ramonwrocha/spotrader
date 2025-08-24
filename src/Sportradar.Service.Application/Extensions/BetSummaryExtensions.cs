using Sportradar.Service.Domain.DTOs;
using System.Text;

namespace Sportradar.Service.Application.Extensions;

public static class BetSummaryExtensions
{
    public static string ToSummary(
        this BasicStatsDto basicStats,
        List<ClientProfitDto> topProfits,
        List<ClientLossDto> topLosses)
    {
        var summary = new StringBuilder();

        summary.AppendLine("=== BET PROCESSING SUMMARY ===");
        summary.AppendLine();

        summary.AppendLine($"Total Bets Processed: {basicStats.TotalProcessed:N0}");
        summary.AppendLine($"Total Amount Bet: ${basicStats.TotalAmount:N2}");

        var profitLossText = basicStats.TotalProfitLoss >= 0 ? "Profit" : "Loss";
        summary.AppendLine($"Total {profitLossText}: ${Math.Abs(basicStats.TotalProfitLoss):N2}");

        summary.AppendLine();
        summary.AppendLine("TOP 5 CLIENTS - HIGHEST PROFITS:");
        for (var i = 0; i < topProfits.Count; i++)
            summary.AppendLine($"  {i + 1}. {topProfits[i].Client}: ${topProfits[i].Profit:N2}");

        summary.AppendLine();
        summary.AppendLine("TOP 5 CLIENTS - HIGHEST LOSSES:");
        for (var i = 0; i < topLosses.Count; i++)
            summary.AppendLine($"  {i + 1}. {topLosses[i].Client}: ${Math.Abs(topLosses[i].Loss):N2}");

        summary.AppendLine();
        summary.AppendLine("=== END SUMMARY ===");

        return summary.ToString();
    }
}
