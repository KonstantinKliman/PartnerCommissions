using System.ComponentModel.DataAnnotations;

namespace Accrual.Api.Validation;

public sealed class MaxDecimalPlacesAttribute(int places) : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value is null)
            return true;

        var number = (decimal)value;
        return decimal.Round(number, places) == number;
    }

    public override string FormatErrorMessage(string name)
    {
        return $"{name} must have at most {places} decimal places.";
    }
}