using Accrual.Application.Dtos;
using Accrual.Domain.Entities;

namespace Accrual.Application.Mappings;

public static class EventMappings
{
    public static EventDto ToDto(this Event e)
    {
        return new EventDto(
            e.ExternalId, 
            e.UserExternalId, 
            e.Profit, 
            e.CreatedAt,
            e.Commissions
                .OrderBy(c => c.Level)
                .Select(c => c.ToDto())
                .ToList()
        );
    }


    public static CommissionDto ToDto(this Commission c)
    {
        return new CommissionDto(
            c.BeneficiaryExternalId,
            c.Level,
            c.Amount,
            c.SchemaType,
            c.PaidAt is not null,
            c.PaidAt
        );
    }
}