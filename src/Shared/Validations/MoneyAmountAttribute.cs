using System.ComponentModel.DataAnnotations;

namespace Shared.Validations;

public sealed class MoneyAmountAttribute : ValidationAttribute
{
    private const decimal MaxAbsoluteAmount = 99_999_999_999_999.9999m;
    
    public bool MustBePositive { get; set; }

    public override bool IsValid(object? value)
    {
        if (value is null)
            return true;

        var amount = (decimal)value;
        return Math.Abs(amount) <= MaxAbsoluteAmount
               && (!MustBePositive || amount > 0)
               && decimal.Round(amount, 4) == amount;
    }

    public override string FormatErrorMessage(string name)
    {
        return MustBePositive
            ? $"{name} must be greater than 0, not exceed {MaxAbsoluteAmount} and have at most 4 decimal places."
            : $"{name} must be between -{MaxAbsoluteAmount} and {MaxAbsoluteAmount} and have at most 4 decimal places.";
    } 
}