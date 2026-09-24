using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Users.Domain.Entities;

namespace Users.Infrastructure.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
        
        builder.Property(u => u.ExternalId)
            .HasMaxLength(128)
            .IsRequired();
        
        builder.HasIndex(u => u.ExternalId)
            .IsUnique();

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(u => u.PartnerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}