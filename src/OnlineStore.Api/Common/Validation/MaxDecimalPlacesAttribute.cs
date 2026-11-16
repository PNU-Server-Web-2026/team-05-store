using System.ComponentModel.DataAnnotations;

namespace OnlineStore.Api.Common.Validation;

/// <summary>
/// Ensures a <see cref="decimal"/> has at most the specified number of fractional digits.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
public sealed class MaxDecimalPlacesAttribute : ValidationAttribute
{
    public MaxDecimalPlacesAttribute(int places)
    {
        Places = places;
    }

    public int Places { get; }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null)
        {
            return ValidationResult.Success;
        }

        if (value is not decimal decimalValue)
        {
            return new ValidationResult("Value must be a decimal.");
        }

        var scale = (decimal.GetBits(decimalValue)[3] >> 16) & 31;
        if (scale > Places)
        {
            return new ValidationResult($"A maximum of {Places} decimal places is allowed.");
        }

        return ValidationResult.Success;
    }
}
