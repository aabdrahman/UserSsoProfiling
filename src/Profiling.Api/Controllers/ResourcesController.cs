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
    public class ResourcesController : ControllerBase
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly ILoggerManager _loggerManager;

        public ResourcesController(IRepositoryManager repositoryManager, ILoggerManager loggerManager)
        {
            _repositoryManager = repositoryManager;
            _loggerManager = loggerManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetResources()
        {
            try
            {
                _loggerManager.LogInfo($"Fetching All Resources..");
                var resources = await _repositoryManager.ResourceRepository.GetAllResources();
                _loggerManager.LogInfo($"Resources Fetched - Data : {JsonSerializer.Serialize(resources)}");

                return Ok(resources);
            }
            catch (Exception ex)
            {
                _loggerManager.LogError($"Unhandled Exception during Fecth Resources: {JsonSerializer.Serialize(ex)}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{name}", Name = "GetByResourceName")]
        public async Task<IActionResult> GetByName(string name)
        {
            try
            {
                _loggerManager.LogInfo($"Fetching Resource - Name: {name}");
                var resource = await _repositoryManager.ResourceRepository.GetResourceByName(name);

                if (resource is null)
                {
                    _loggerManager.LogWarn($"No Resource Found for Name: {name}");
                    return StatusCode(400, $"No resource with name: {name}");
                }
                _loggerManager.LogInfo($"Resource with Name: {name} - {JsonSerializer.Serialize(resource)}");
                return Ok(resource);
            }
            catch (Exception ex)
            {
                _loggerManager.LogError($"Unhandled Exception during Fecth Resource - Name: {name}: {JsonSerializer.Serialize(ex)}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateResourceDto NewResource)
        {
            try
            {
                _loggerManager.LogInfo($"Creating Resource - Resource: {JsonSerializer.Serialize(NewResource)}");
                var resource = await _repositoryManager.ResourceRepository.Create(NewResource);
                return CreatedAtRoute("GetByResourceName", new { name = resource.NormalizedName }, resource);
            }
            catch (Exception ex)
            {
                _loggerManager.LogError($"Unhandled Exception during Create Resource - Date: {JsonSerializer.Serialize(NewResource)} : {JsonSerializer.Serialize(ex)}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{Name}")]
        public async Task<IActionResult> Delete(string Name)
        {
            try
            {
                _loggerManager.LogInfo($"Delete Resource - Name: {Name}");
                var result = await _repositoryManager.ResourceRepository.Delete(Name);
                _loggerManager.LogInfo($"Resource Delete Successfule - Name: {Name}");
                return Ok($"Number Of Records deleted: {result}");
            }
            catch (Exception ex)
            {
                _loggerManager.LogError($"Unhandled Exception during Delete Resource: {JsonSerializer.Serialize(ex)}");
                return StatusCode(500, ex.Message);
            }
        }

    }
}
