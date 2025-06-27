using System.Text.Json;
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
    public class ModulesController : ControllerBase
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly ILoggerManager _loggerManager;

        public ModulesController(IRepositoryManager repositoryManager, ILoggerManager loggerManager)
        {
            _repositoryManager = repositoryManager;
            _loggerManager = loggerManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetModules()
        {
            try
            {
                _loggerManager.LogInfo($"Calling The Get All Modules");
                var modules = await _repositoryManager.ModuleRepository.GetModules();
                _loggerManager.LogInfo($"Modules Fetched Successfully: {JsonSerializer.Serialize(modules)}");
                return Ok(modules);
            }
            catch (Exception ex)
            {
                _loggerManager.LogError($"Error occurred Fetching User Module: {JsonSerializer.Serialize(ex)}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{Name}", Name = "GetByName")]
        public async Task<IActionResult> GetByName(string Name)
        {
            try
            {
                _loggerManager.LogInfo($"Fetching Module: {Name}");
                var module = await _repositoryManager.ModuleRepository.GetByName(Name);
                if (module is null)
                {
                    _loggerManager.LogError($"Module With Name: {Name} does not exist");
                    return NotFound($"Module With Name: {Name} does not exist");
                }
                _loggerManager.LogInfo($"Module Fetched Successfully: {JsonSerializer.Serialize(module)}");

                return Ok(module);

            }
            catch (Exception ex)
            {
                _loggerManager.LogError($"Error occurred Fetching User Module with name: {Name}: {JsonSerializer.Serialize(ex)}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("Resource/{ResourceName}")]
        public async Task<IActionResult> GetModuleByResource(string ResourceName)
        {
            try
            {
                _loggerManager.LogInfo($"Fetching Module For Resource: {ResourceName}");
                var resourceModules = await _repositoryManager.ModuleRepository.GetModuleByResource(ResourceName);

                if (resourceModules.Count() == 0)
                {
                    _loggerManager.LogError($"No Module found for resource with Name: {ResourceName}");
                    return NotFound($"No Module for specified resource: {ResourceName}");
                }
                _loggerManager.LogInfo($"Module Fetched Successfully For Resource: {ResourceName}: {JsonSerializer.Serialize(resourceModules)}");
                return Ok(resourceModules);
            }
            catch (Exception ex)
            {
                _loggerManager.LogError($"Error occurred Fetching User Module for resource: {ResourceName}: {JsonSerializer.Serialize(ex)}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateModuleDto NewModule)
        {
            try
            {
                _loggerManager.LogInfo($"Creating Resource: {JsonSerializer.Serialize(NewModule)}");
                var resource = await _repositoryManager.ResourceRepository.GetResourceByName(NewModule.ResourceName.ToUpper());

                if (resource is null)
                {
                    _loggerManager.LogError($"Error creating User Module: {JsonSerializer.Serialize(NewModule)} as specified resource does not exist.");
                    return BadRequest($"Specified Resource Name does not exist: {NewModule.ResourceName}");
                }

                var createdModule = await _repositoryManager.ModuleRepository.Create(NewModule);
                _loggerManager.LogInfo($"Module Created Successfully: {JsonSerializer.Serialize(createdModule)}");
                return CreatedAtRoute("GetByName", new { Name = createdModule.NormalizedName }, createdModule);
            }
            catch (Exception ex)
            {
                _loggerManager.LogError($"Error occurred Creating User Module: {JsonSerializer.Serialize(ex)}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] DeleteModuleDto deleteModule)
        {
            try
            {
                _loggerManager.LogInfo($"Deleting Resource: {JsonSerializer.Serialize(deleteModule)}");
                var result = await _repositoryManager.ModuleRepository.Delete(deleteModule);
                _loggerManager.LogInfo($"Deleting Resource Succesful: {JsonSerializer.Serialize(deleteModule)} with response: {result}");
                return Ok($"{result} Module Deleted.");
            }
            catch (Exception ex)
            {
                _loggerManager.LogError($"Error occurred Deleting Module: {JsonSerializer.Serialize(ex)}");
                return StatusCode(500, ex.Message);
            }
        }
    }
}
