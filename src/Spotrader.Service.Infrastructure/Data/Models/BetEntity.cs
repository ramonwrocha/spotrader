using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Spotrader.Service.Domain.ValueObjects;

namespace Spotrader.Service.Infrastructure.Data.Models;

[Table("Bets")]
public class BetEntity
{
    [Key]
    public long Id { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public double Amount { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public double Odds { get; set; }

    [Required]
    [MaxLength(100)]
    public string Client { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Event { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Market { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Selection { get; set; } = string.Empty;

    [Required]
    public BetStatus Status { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
