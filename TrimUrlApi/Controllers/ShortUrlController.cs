using Microsoft.AspNetCore.Mvc;
using TrimUrlApi.Models;
using TrimUrlApi.Services;

namespace TrimUrlApi.Controllers
{
    [ApiController]
    [Route("short-urls")]
    public class ShortUrlController(ILogger<ShortUrlController> logger, ShortUrlService shortUrlService) : ControllerBase
    {
        private readonly ILogger<ShortUrlController> _logger = logger;
        private readonly ShortUrlService _shortUrlService = shortUrlService;

        [HttpGet("code/{code}")]
        public async Task<IActionResult> GetByCode(string code)
        {
            var getModel = await _shortUrlService.GetByCode(code);
            if (getModel == null) 
            {
                return NotFound($"No URL found with code: {code}");
            }
            return Ok(getModel);
        }

        [HttpPost()]
        public async Task<IActionResult> Create(ShortUrlPostModel postModel)
        {
            var shortUrl = await _shortUrlService.Create(postModel);
            return Ok(shortUrl);
        }
    }
}
