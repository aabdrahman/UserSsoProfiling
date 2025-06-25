using System.ComponentModel.DataAnnotations;
using Profiling.Api.Shared;

namespace Profiling.Api.DataTransferObjects;

public record class AddUserModuleDto
{
    [Required(ErrorMessage = "User ID is required")]
    public string UserId { get; init; }
    [EnumValidator(typeof(Resources)), Required(ErrorMessage = "Resource Name is required")]
    public string ResourceName { get; init; }
    [EnumValidator(typeof(Modules)), Required(ErrorMessage = "Resource Name is required")]
    public string ModuleName { get; init; }
}
