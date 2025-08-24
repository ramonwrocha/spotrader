using System.ComponentModel.DataAnnotations;

namespace Spotrader.Service.Application.DTOs;

public class SeedBetsParams
{
    [Range(1, 5_000_000)]
    public long TotalBets { get; set; }
}
