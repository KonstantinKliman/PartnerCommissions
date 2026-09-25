namespace Accrual.Domain.Entities;

public class SchemaChange
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    
    public SchemaType SchemaType { get; set; }

    public DateTimeOffset ChangedAt { get; set; } = DateTimeOffset.UtcNow;
}