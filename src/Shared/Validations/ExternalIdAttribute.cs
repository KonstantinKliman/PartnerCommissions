using System.ComponentModel.DataAnnotations;

namespace Shared.Validations;

public sealed class ExternalIdAttribute() : RegularExpressionAttribute("^[A-Za-z0-9_-]{1,128}$")
{
    public override string FormatErrorMessage(string name)
    {
        return $"{name} must be 1-128 characters long and contain only Latin letters, digits, '_' or '-'.";
    }
}