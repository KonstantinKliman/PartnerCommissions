using System.ComponentModel.DataAnnotations;

namespace Accrual.Api.Validation;

public sealed class MoneyAmountAttribute : ValidationAttribute
{
    private const decimal MaxAbsoluteAmount = 99_999_999_999_999.9999m;

    public override bool IsValid(object? value)
    {
        if (value is null)
            return true;

        var amount = (decimal)value;
        return Math.Abs(amount) <= MaxAbsoluteAmount
               && decimal.Round(amount, 4) == amount;
    }

    public override string FormatErrorMessage(string name)
    {
        return $"{name} must be between -{MaxAbsoluteAmount} and {MaxAbsoluteAmount} and have at most 4 decimal places.";
    }
        
}