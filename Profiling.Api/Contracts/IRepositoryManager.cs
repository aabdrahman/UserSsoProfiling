using System;

namespace Profiling.Api.Contracts;

public interface IRepositoryManager
{
    IModuleRepository ModuleRepository { get; }
    IResourceRepository ResourceRepository { get; }
    IUserRepository UserRepository { get; }
}
