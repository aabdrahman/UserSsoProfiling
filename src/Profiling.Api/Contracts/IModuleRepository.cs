using System;
using Profiling.Api.DataTransferObjects;
using Profiling.Api.Entities;

namespace Profiling.Api.Contracts;

public interface IModuleRepository
{
    Task<IEnumerable<Module>> GetModules();
    Task<IEnumerable<Module>> GetModuleByResource(string ResourceName);
    Task<Module?> GetByName(string Name);
    Task<Module> Create(CreateModuleDto NewModule);
    Task<int> Delete(DeleteModuleDto moduleToDelete);
}
