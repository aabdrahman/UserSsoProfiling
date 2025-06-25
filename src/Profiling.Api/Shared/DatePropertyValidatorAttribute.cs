using System.ComponentModel.DataAnnotations;

namespace Profiling.Api.Shared;

public class DatePropertyValidatorAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null)
            return ValidationResult.Success;

        if (value is not DateOnly dateValue)
            return new ValidationResult($"Provided Value is not a valid date.");

        if (dateValue <= DateOnly.FromDateTime(DateTime.Today))
            return new ValidationResult($"The provided date must be a future date");

        return ValidationResult.Success;
    }
}
