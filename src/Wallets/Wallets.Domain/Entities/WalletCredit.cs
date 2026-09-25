namespace Wallets.Domain.Entities;

public class WalletCredit
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public Guid WalletId { get; set; }
    public Guid CommissionId { get; set; }
    public string EventExternalId { get; set; } = null!;
    public decimal Amount { get; set; }
    public DateTimeOffset CreditedAt { get; set; } = DateTimeOffset.UtcNow;
}