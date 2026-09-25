namespace Wallets.Domain.Entities;

public class Wallet
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public string UserExternalId { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public List<WalletCredit> Credits { get; set; } = new();
}