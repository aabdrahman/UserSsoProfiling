using System;
using Profiling.Api.DataTransferObjects;

namespace Profiling.Api.Contracts;

public interface IUserRepository
{
    Task<string> Create(CreateUserDto NewUser);
    Task<string?> GetUser(string UserId);
    Task<List<ResourceDto>> GetUserResources(string UserId);
    Task<List<ModuleDto>> GetUserModules(string UserId);
    Task<string> AddUserModule(List<AddUserModuleDto> userModules);
    Task<string> AddUserResource(List<AddUserResourceDto> userResources);
    Task<string> RemoveUserModuleAccess(List<AddUserModuleDto> userModulesToDelete);
    Task<string> RemoveUserResourceAccess(List<AddUserResourceDto> userResourcesToDelete);
    Task<string> DeleteUser(string UserId);
    Task<string> UnDeleteUser(string UserId);
}
