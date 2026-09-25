using Accrual.Domain;
using Accrual.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Accrual.Infrastructure.Configurations;

public class SchemaChangeConfiguration : IEntityTypeConfiguration<SchemaChange>
{
    public void Configure(EntityTypeBuilder<SchemaChange> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.SchemaType)
            .HasConversion<string>();

        builder.HasIndex(s => s.ChangedAt);
    }
}