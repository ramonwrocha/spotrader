namespace Spotrader.Service.Application.DTOs;

public class BetSummary
{
    public int TotalBets { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal TotalWinnings { get; set; }
    public List<ClientProfitInfo> TopClientsByProfit { get; set; } = new();
    public List<ClientLossInfo> TopClientsByLoss { get; set; } = new();
}

public class ClientProfitInfo
{
    public string Client { get; set; } = string.Empty;
    public decimal TotalProfit { get; set; }
}

public class ClientLossInfo
{
    public string Client { get; set; } = string.Empty;
    public decimal TotalLoss { get; set; }
}
