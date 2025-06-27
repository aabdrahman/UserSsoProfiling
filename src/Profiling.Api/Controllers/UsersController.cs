using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using LoggerService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Profiling.Api.Contracts;
using Profiling.Api.DataTransferObjects;

namespace Profiling.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly ILoggerManager _loggerManager;

        public UsersController(IRepositoryManager repositoryManager, ILoggerManager loggerManager)
        {
            _repositoryManager = repositoryManager;
            _loggerManager = loggerManager;
        }

        [HttpPost("AddUserModules")]
        public async Task<IActionResult> AddUserModules([FromBody] List<AddUserModuleDto> userModulesToAdd)
        {
            try
            {
                _loggerManager.LogInfo($"Adding User Module: Data - {JsonSerializer.Serialize(userModulesToAdd)}");
                var result = await _repositoryManager.UserRepository.AddUserModule(userModulesToAdd);
                _loggerManager.LogInfo($"Modules Added Successfully - {JsonSerializer.Serialize(userModulesToAdd)} : {result}");

                return Ok(result);
            }
            catch (Exception ex)
            {
                _loggerManager.LogError($"Error Adding Modules - {JsonSerializer.Serialize(userModulesToAdd)}: {JsonSerializer.Serialize(ex)}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("AddUserResources")]
        public async Task<IActionResult> AddUserResources([FromBody] List<AddUserResourceDto> userResourcesToAdd)
        {
            try
            {
                _loggerManager.LogInfo($"Adding User Resource - {JsonSerializer.Serialize(userResourcesToAdd)}");
                var result = await _repositoryManager.UserRepository.AddUserResource(userResourcesToAdd);
                _loggerManager.LogInfo($"Adding User Resource Successfull - {JsonSerializer.Serialize(userResourcesToAdd)} : {result}");

                return Ok(result);
            }
            catch (Exception ex)
            {
                _loggerManager.LogError($"Error Adding Resources - {JsonSerializer.Serialize(userResourcesToAdd)}: {JsonSerializer.Serialize(ex)}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("GetUserResources/{UserId}")]
        public async Task<IActionResult> GetUserResources(string UserId)
        {
            try
            {
                _loggerManager.LogInfo($"Get User Resource - User ID: {UserId}");
                var UserResources = await _repositoryManager.UserRepository.GetUserResources(UserId.ToUpper());
                _loggerManager.LogInfo($"Get User Resource Successfull - {JsonSerializer.Serialize(UserResources)} : {UserId}");
                return Ok(UserResources);
            }
            catch (Exception ex)
            {
                _loggerManager.LogError($"Error Getting User Resources - {UserId}: {JsonSerializer.Serialize(ex)}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("GetUserModules/{UserId}")]
        public async Task<IActionResult> GetUserModules(string UserId)
        {
            try
            {
                _loggerManager.LogInfo($"Get User Modules - {UserId}");
                var UserModules = await _repositoryManager.UserRepository.GetUserModules(UserId.ToUpper());
                _loggerManager.LogInfo($"Get User Resource Successfull - {JsonSerializer.Serialize(UserModules)} : {UserId}");
                return Ok(UserModules);
            }
            catch (Exception ex)
            {
                _loggerManager.LogError($"Error Getting User Modules - {UserId}: {JsonSerializer.Serialize(ex)}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> RegisterUser([FromBody] CreateUserDto NewUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _loggerManager.LogInfo($"Register User: {JsonSerializer.Serialize(NewUser)}");
                var createdUser = await _repositoryManager.UserRepository.Create(NewUser);
                _loggerManager.LogInfo($"Register User Successfull - {JsonSerializer.Serialize(createdUser)}");
                return Ok($"User Created: {createdUser}");
            }
            catch (Exception ex)
            {
                _loggerManager.LogError($"Error Registering User - {JsonSerializer.Serialize(NewUser)}: {JsonSerializer.Serialize(ex)}");
                if (ex is Microsoft.Data.SqlClient.SqlException sqlEx)
                    return StatusCode(400, sqlEx.Message);

                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("DeleteUserResources")]
        public async Task<IActionResult> DeleteUserResources([FromBody] List<AddUserResourceDto> userResourcesToDelete)
        {
            try
            {
                _loggerManager.LogInfo($"Delete User Resources - {JsonSerializer.Serialize(userResourcesToDelete)}");
                var result = await _repositoryManager.UserRepository.RemoveUserResourceAccess(userResourcesToDelete);
                _loggerManager.LogInfo($"Delete User Resource Successfull - {JsonSerializer.Serialize(userResourcesToDelete)} :{result}");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _loggerManager.LogError($"Error Deleting User Resources - {JsonSerializer.Serialize(userResourcesToDelete)}: {JsonSerializer.Serialize(ex)}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("DeleteUserModules")]
        public async Task<IActionResult> DeleteUserModules([FromBody] List<AddUserModuleDto> userModulesToDelete)
        {
            try
            {
                _loggerManager.LogInfo($"Delete User Modules - {JsonSerializer.Serialize(userModulesToDelete)}");
                var result = await _repositoryManager.UserRepository.RemoveUserModuleAccess(userModulesToDelete);
                _loggerManager.LogInfo($"Delete User Modules Successfull - {JsonSerializer.Serialize(userModulesToDelete)} : {result}");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _loggerManager.LogError($"Error Deleting User Modules - {JsonSerializer.Serialize(userModulesToDelete)}: {JsonSerializer.Serialize(ex)}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("DeleteUser/{UserId}")]
        public async Task<IActionResult> DeleteUser(string UserId)
        {
            try
            {
                _loggerManager.LogInfo($"Delete User: {UserId}");
                var result = await _repositoryManager.UserRepository.DeleteUser(UserId);
                _loggerManager.LogInfo($"Delete User Successful - {UserId} : {result}");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _loggerManager.LogError($"Error Deleting User - {UserId}: {JsonSerializer.Serialize(ex)}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("UnDeleteUser/{UserId}")]
        public async Task<IActionResult> UnDeleteUser(string UserId)
        {
            try
            {
                _loggerManager.LogInfo($"UnDelete User: {UserId}");
                var result = await _repositoryManager.UserRepository.UnDeleteUser(UserId);
                _loggerManager.LogInfo($"UnDelete User Successful - {UserId} : {result}");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _loggerManager.LogError($"Error Undeleting User - {UserId}: {JsonSerializer.Serialize(ex)}");
                return StatusCode(500, ex.Message);
            }
        }
    }
}
