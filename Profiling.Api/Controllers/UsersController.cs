using System.Globalization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Profiling.Api.Contracts;
using Profiling.Api.DataTransferObjects;

namespace Profiling.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IRepositoryManager _repositoryManager;

        public UsersController(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
        }

        [HttpPost("AddUserModules")]
        public async Task<IActionResult> AddUserModules([FromBody] List<AddUserModuleDto> userModulesToAdd)
        {
            try
            {
                var result = await _repositoryManager.UserRepository.AddUserModule(userModulesToAdd);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("AddUserResources")]
        public async Task<IActionResult> AddUserResources([FromBody] List<AddUserResourceDto> userResourcesToAdd)
        {
            try
            {
                var result = await _repositoryManager.UserRepository.AddUserResource(userResourcesToAdd);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("GetUserResources/{UserId}")]
        public async Task<IActionResult> GetUserResources(string UserId)
        {
            try
            {
                var UserResources = await _repositoryManager.UserRepository.GetUserResources(UserId.ToUpper());
                return Ok(UserResources);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("GetUserModules/{UserId}")]
        public async Task<IActionResult> GetUserModules(string UserId)
        {
            try
            {
                var UserModules = await _repositoryManager.UserRepository.GetUserModules(UserId.ToUpper());
                return Ok(UserModules);
            }
            catch (Exception ex)
            {
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
                var createdUser = await _repositoryManager.UserRepository.Create(NewUser);
                return Ok($"User Created: {createdUser}");
            }
            catch (Exception ex)
            {
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
                var result = await _repositoryManager.UserRepository.RemoveUserResourceAccess(userResourcesToDelete);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("DeleteUserModules")]
        public async Task<IActionResult> DeleteUserModules([FromBody] List<AddUserModuleDto> userModulesToDelete)
        {
            try
            {
                var result = await _repositoryManager.UserRepository.RemoveUserModuleAccess(userModulesToDelete);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("DeleteUser/{UserId}")]
        public async Task<IActionResult> DeleteUser(string UserId)
        {
            try
            {
                var result = await _repositoryManager.UserRepository.DeleteUser(UserId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("UnDeleteUser/{UserId}")]
        public async Task<IActionResult> UnDeleteUser(string UserId)
        {
            try
            {
                var result = await _repositoryManager.UserRepository.UnDeleteUser(UserId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
