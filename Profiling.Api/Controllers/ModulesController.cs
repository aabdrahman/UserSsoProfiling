using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Profiling.Api.Contracts;
using Profiling.Api.DataTransferObjects;

namespace Profiling.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModulesController : ControllerBase
    {
        private readonly IRepositoryManager _repositoryManager;

        public ModulesController(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetModules()
        {
            try
            {
                var modules = await _repositoryManager.ModuleRepository.GetModules();
                return Ok(modules);
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{Name}", Name = "GetByName")]
        public async Task<IActionResult> GetByName(string Name)
        {
            try
            {
                var module = await _repositoryManager.ModuleRepository.GetByName(Name);
                if (module is null)
                    return NotFound($"Module With Name: {Name} does not exist");

                return Ok(module);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("Resource/{ResourceName}")]
        public async Task<IActionResult> GetModuleByResource(string ResourceName)
        {
            try
            {
                var resourceModules = await _repositoryManager.ModuleRepository.GetModuleByResource(ResourceName);

                if (resourceModules.Count() == 0)
                    return NotFound($"No Module for specified resource: {ResourceName}");

                return Ok(resourceModules);
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateModuleDto NewModule)
        {
            try
            {
                var resource = await _repositoryManager.ResourceRepository.GetResourceByName(NewModule.ResourceName.ToUpper());

                if (resource is null)
                    return BadRequest($"Specified Resource Name does not exist: {NewModule.ResourceName}");

                var createdModule = await _repositoryManager.ModuleRepository.Create(NewModule);

                return CreatedAtRoute("GetByName", new { Name = createdModule.NormalizedName }, createdModule);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] DeleteModuleDto deleteModule)
        {
            try
            {
                var result = await _repositoryManager.ModuleRepository.Delete(deleteModule);
                return Ok($"{result} Module Deleted.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
