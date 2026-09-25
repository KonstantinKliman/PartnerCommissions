using Accrual.Domain.Commissions;

namespace Accrual.Domain.Entities;

public class Event
{
    public Guid Id { get; set; } = Guid.CreateVersion7();

    public string ExternalId { get; set; } = null!;

    public string UserExternalId { get; set; } = null!;
    
    public decimal Profit { get; set; }

    public List<Commission> Commissions { get; set; } = new();
    
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    
    public static Event Create(
        string externalId, string userExternalId, decimal profit, List<string> partnersUpward, SchemaType schema)
    {
        var lines = CommissionCalculator.Calculate(profit, partnersUpward, schema);

        return new Event
        {
            ExternalId = externalId,
            UserExternalId = userExternalId,
            Profit = profit,
            Commissions = lines.Select(Commission.FromLine).ToList()
        };
    }
}