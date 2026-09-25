using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wallets.Domain.Entities;

namespace Wallets.Infrastructure.Configurations;

public class WalletCreditConfiguration : IEntityTypeConfiguration<WalletCredit>
{
    public void Configure(EntityTypeBuilder<WalletCredit> builder)
    {
        builder.HasKey(c => c.Id);

        builder.HasIndex(c => c.CommissionId)
            .IsUnique();

        builder.Property(c => c.EventExternalId)
            .HasMaxLength(128);

        builder.Property(c => c.Amount)
            .HasPrecision(18, 4);

        builder.HasIndex(c => new { c.WalletId, c.CreditedAt, c.Id })
            .IsDescending(false, true, true);
    }
}