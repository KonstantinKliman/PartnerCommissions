using Wallets.Application.Dtos;
using Wallets.Domain.Entities;

namespace Wallets.Application.Mappings;

public static class WalletMappings
{
    public static CreditDto ToDto(this WalletCredit c) =>
        new(c.CommissionId, c.EventExternalId, c.Amount, c.CreditedAt);
}