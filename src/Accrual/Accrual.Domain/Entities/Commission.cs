using Accrual.Domain.Commissions;

namespace Accrual.Domain.Entities;

public class Commission
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    
    public Guid EventId { get; set; }

    public string BeneficiaryExternalId { get; set; } = null!;
    
    public int Level { get; set; }
    
    public decimal Amount { get; set; }
    
    public SchemaType SchemaType { get; set; }
    
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    
    public DateTimeOffset? PaidAt { get; set; }
    
    public static Commission FromLine(CommissionLine line) => new()
    {
        BeneficiaryExternalId = line.BeneficiaryExternalId,
        Level = line.Level,
        Amount = line.Amount,
        SchemaType = line.Schema
    };
}