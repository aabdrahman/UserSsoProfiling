using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Profiling.Api.Contracts;
using Profiling.Api.DataTransferObjects;

namespace Profiling.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResourcesController : ControllerBase
    {
        private readonly IRepositoryManager _repositoryManager;

        public ResourcesController(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetResources()
        {
            try
            {
                var resources = await _repositoryManager.ResourceRepository.GetAllResources();

                return Ok(resources);
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{name}", Name = "GetByResourceName")]
        public async Task<IActionResult> GetByName(string name)
        {
            try
            {
                var resource = await _repositoryManager.ResourceRepository.GetResourceByName(name);

                if (resource is null)
                    return StatusCode(400, $"No resource with name: {name}");

                return Ok(resource);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateResourceDto NewResource)
        {
            try
            {
                var resource = await _repositoryManager.ResourceRepository.Create(NewResource);
                return CreatedAtRoute("GetByResourceName", new { name = resource.NormalizedName }, resource);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{Name}")]
        public async Task<IActionResult> Delete(string Name)
        {
            try
            {
                var result = await _repositoryManager.ResourceRepository.Delete(Name);
                return Ok($"Number Of Records deleted: {result}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}
