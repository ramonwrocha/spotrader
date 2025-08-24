using System.ComponentModel.DataAnnotations;

namespace Spotrader.Service.Application.DTOs;

public class CreateBetRequest
{
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
    public double Amount { get; set; }

    [Required]
    [Range(1.01, double.MaxValue, ErrorMessage = "Odds must be greater than 1.0")]
    public double Odds { get; set; }

    [Required]
    [StringLength(50, MinimumLength = 1, ErrorMessage = "Client name must be between 1 and 50 characters")]
    public string Client { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Event name must be between 1 and 100 characters")]
    public string Event { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Market name must be between 1 and 100 characters")]
    public string Market { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Selection name must be between 1 and 100 characters")]
    public string Selection { get; set; } = string.Empty;
}
