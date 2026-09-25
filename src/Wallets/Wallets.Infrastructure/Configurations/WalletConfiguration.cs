using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wallets.Domain.Entities;

namespace Wallets.Infrastructure.Configurations;

public class WalletConfiguration : IEntityTypeConfiguration<Wallet>
{
    public void Configure(EntityTypeBuilder<Wallet> builder)
    {
        builder.HasKey(w => w.Id);

        builder.Property(w => w.UserExternalId)
            .HasMaxLength(128);

        builder.HasIndex(w => w.UserExternalId)
            .IsUnique();

        builder.HasMany(w => w.Credits)
            .WithOne()
            .HasForeignKey(c => c.WalletId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}