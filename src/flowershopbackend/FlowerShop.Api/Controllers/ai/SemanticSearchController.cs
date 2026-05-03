using FlowerShop.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FlowerShop.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SemanticSearchController : ControllerBase
    {
        readonly IAiSearchService _aiSearchService;
        public SemanticSearchController(IAiSearchService aiSearchService)
        {
            _aiSearchService = aiSearchService;
        }
        [HttpGet("aisearch/{search}")]
        public async Task<IActionResult> GetAllFlowers(string search)
        {
            var flowers = await _aiSearchService.Search(search);
            return Ok(flowers);
        }
    }
}
