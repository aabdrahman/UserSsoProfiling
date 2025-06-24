using System;
using System.ComponentModel.DataAnnotations;

namespace Profiling.Api.Shared;

public class EnumValidatorAttribute : ValidationAttribute
{
    private readonly Type _enumType;

    public EnumValidatorAttribute(Type enumType)
    {
        if (enumType is null)
            throw new ArgumentException($"Must be a Enum Type.");
        _enumType = enumType;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null)
        {
            return ValidationResult.Success;
        }

        if (value is not string stringValue)
            return new ValidationResult($"Property must be a string Value");

        if (!Enum.TryParse(_enumType, stringValue.ToUpper(), ignoreCase: false, out _))
        {
            return new ValidationResult($"{stringValue} is not a valid value");
        }

        return ValidationResult.Success;
    }
}
