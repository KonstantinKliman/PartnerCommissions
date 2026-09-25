using Accrual.Domain;
using Accrual.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Accrual.Infrastructure.Configurations;

public class SettingsConfiguration : IEntityTypeConfiguration<AccrualSettings>
{
    public void Configure(EntityTypeBuilder<AccrualSettings> builder)
    {
        builder.Property(s => s.SchemaType)
            .HasConversion<string>();

        builder.Property(s => s.Id)
            .ValueGeneratedNever();
        
        builder.HasData(new AccrualSettings
        {
            Id = 1,
            SchemaType = SchemaType.Linear
        });
    }
}