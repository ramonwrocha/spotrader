using Microsoft.EntityFrameworkCore;
using Spotrader.Service.Infrastructure.Data.Models;

namespace Spotrader.Service.Infrastructure.Data;

public class SpotraderDbContext : DbContext
{
    public SpotraderDbContext(DbContextOptions<SpotraderDbContext> options) : base(options) { }

    public DbSet<BetEntity> Bets { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<BetEntity>(entity =>
        {
            entity.ToTable("bets");
            
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();
                
            entity.Property(e => e.Amount)
                .HasColumnName("amount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();
                
            entity.Property(e => e.Odds)
                .HasColumnName("odds")
                .HasColumnType("decimal(18,2)")
                .IsRequired();
                
            entity.Property(e => e.Client)
                .HasColumnName("client")
                .HasMaxLength(100)
                .IsRequired();
                
            entity.Property(e => e.Event)
                .HasColumnName("event")
                .HasMaxLength(200)
                .IsRequired();
                
            entity.Property(e => e.Market)
                .HasColumnName("market")
                .HasMaxLength(100)
                .IsRequired();
                
            entity.Property(e => e.Selection)
                .HasColumnName("selection")
                .HasMaxLength(100)
                .IsRequired();
                
            entity.Property(e => e.Status)
                .HasColumnName("status")
                .HasMaxLength(20)
                .HasConversion<string>()
                .IsRequired();
        });
    }
}
