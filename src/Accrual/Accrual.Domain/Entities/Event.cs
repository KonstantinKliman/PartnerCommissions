namespace Accrual.Domain.Entities;

public class Event
{
    public Guid Id { get; set; } = Guid.CreateVersion7();

    public string ExternalId { get; set; } = null!;

    public string UserExternalId { get; set; } = null!;
    
    public decimal Profit { get; set; }

    public List<Commission> Commissions { get; set; } = new();
    
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}