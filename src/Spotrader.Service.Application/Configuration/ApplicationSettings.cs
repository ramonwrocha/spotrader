namespace Spotrader.Service.Application.Configuration;

public class ApplicationSettings
{
    public const string SectionName = "ApplicationSettings";

    public int MaxDataSeedBets { get; set; }

    public int MaxBetBatchSize { get; set; }

    public double WinnerThreshold { get; set; }

    public double LoserThreshold { get; set; }

    public int ProcessingDelayMs { get; set; }
}
