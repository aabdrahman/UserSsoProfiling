using System.ComponentModel.DataAnnotations;
using Profiling.Api.Shared;

namespace Profiling.Api.DataTransferObjects;

public record class CreateUserDto : IValidatableObject
{
    [Required(ErrorMessage = "User ID is required")]
    public string UserId { get; init; }
    [Required(ErrorMessage = "User Name is required")]
    public string UserName { get; init; }
    [Required(ErrorMessage = "LoginTimeLow is required")]
    public DateOnly LoginTimeLow { get; init; }
    [Required(ErrorMessage = "LoginTimeHigh is required"), DatePropertyValidator]
    public DateOnly LoginTimeHigh { get; init; }
    [Required(ErrorMessage = "DisabledFromDate is required"), DatePropertyValidator]
    public DateOnly DisabledFromDate { get; init; }
    [Required(ErrorMessage = "DisabledUpToDate is required"), DatePropertyValidator]
    public DateOnly DisabledUpToDate { get; init; }
    [Required(ErrorMessage = "PasswordExpiryDate is required"), DatePropertyValidator]
    public DateOnly PasswordExpiryDate { get; init; }
    [Required(ErrorMessage = "AcctExpiryDate is required"), DatePropertyValidator]
    public DateOnly AcctExpiryDate { get; init; }
    [Required(ErrorMessage = "AcctInactiveDays is required")]
    public int AcctInactiveDays { get; init; }
    [Required(ErrorMessage = "Require2FA is required"), EnumValidator(typeof(YesNo))]
    public string Require2FA { get; init; }
    [Required(ErrorMessage = "IsGlobalAdmin is required"), EnumValidator(typeof(YesNo))]
    public string IsGlobalAdmin { get; init; }
    [Required(ErrorMessage = "User Maximum Inactive Time is required"), Range(1, 30, ErrorMessage = "Value must be between 1 and 30")]
    public int UserMaxInactiveTime { get; init; }

    public List<AddUserResourceDto> Resources { get; init; } = new();
    public List<AddUserModuleDto> Modules { get; init; } = new();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (UserId is not null)
        {
            foreach (var resource in Resources)
            {
                if (resource.UserId != UserId)
                    yield return new ValidationResult($"User Id mismatch for one of the resource records.");
            }
            foreach (var module in Modules)
            {
                if (module.UserId != UserId)
                    yield return new ValidationResult($"User Id mismatch for one of the module records");
            }
        }
    }
}
