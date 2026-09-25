using Accrual.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Accrual.Infrastructure.Configurations;

public class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.HasIndex(e => e.ExternalId)
            .IsUnique();

        builder.HasIndex(e => new { e.UserExternalId, e.CreatedAt, e.Id })
            .IsDescending(false, true, true);
        
        builder.Property(e => e.Profit)
            .HasPrecision(18, 4);
        
        builder.HasMany(e => e.Commissions)
            .WithOne()
            .HasForeignKey(e => e.EventId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}