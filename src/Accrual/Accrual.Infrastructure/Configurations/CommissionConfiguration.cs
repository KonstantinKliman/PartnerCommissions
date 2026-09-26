using Accrual.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Accrual.Infrastructure.Configurations;

public class CommissionConfiguration : IEntityTypeConfiguration<Commission>
{
    public void Configure(EntityTypeBuilder<Commission> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.HasIndex(e => new { e.EventId, e.BeneficiaryExternalId })
            .IsUnique();
        
        builder.Property(e => e.Amount)
            .HasPrecision(18, 4);

        builder.Property(c => c.SchemaType)
            .HasConversion<string>();
    }
}