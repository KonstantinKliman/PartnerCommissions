using Accrual.Application.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Accrual.Infrastructure.Configurations;

public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Type)
            .HasMaxLength(100);

        builder.Property(m => m.Payload)
            .HasColumnType("jsonb");

        builder.HasIndex(m => m.NextAttemptAt)
            .HasFilter("processed_at IS NULL");
    }
}