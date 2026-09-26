using System.ComponentModel.DataAnnotations;

namespace Wallets.Api.Validation;

public sealed class PositiveAmountAttribute : ValidationAttribute
{
    private const decimal MaxAmount = 99_999_999_999_999.9999m;

    public override bool IsValid(object? value)
    {
        if (value is null)
            return true;

        var amount = (decimal)value;
        return amount > 0
               && amount <= MaxAmount
               && decimal.Round(amount, 4) == amount;
    }

    public override string FormatErrorMessage(string name) =>
        $"{name} must be greater than 0, not exceed {MaxAmount} and have at most 4 decimal places.";
}