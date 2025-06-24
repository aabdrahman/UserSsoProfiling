namespace Profiling.Api.DataTransferObjects;

public record class ModuleDto
{
    public string ResourceName { get; init; }
    public string ModuleName { get; init; }
}
