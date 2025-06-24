using System;
using Profiling.Api.DataTransferObjects;
using Profiling.Api.Entities;

namespace Profiling.Api.Contracts;

public interface IResourceRepository
{
    Task<IEnumerable<Resource>> GetAllResources();
    Task<Resource?> GetResourceByName(string Name);
    Task<Resource> Create(CreateResourceDto NewResource);
    Task<int> Delete(string Name);
}
