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
            if (!_shortUrlService.IsValidUrl(postModel.Url))
            {
                return BadRequest($"Invalid URL string: {postModel.Url}");
            }
            var shortUrl = await _shortUrlService.Create(postModel);
            return Ok(shortUrl);
        }

        [HttpPut()]
        public async Task<IActionResult> UpdateByCode(ShortUrlPutModel putModel)
        {
            if (!_shortUrlService.IsValidUrl(putModel.Url))
            {
                return BadRequest($"Invalid URL string: {putModel.Url}");
            }

            var updatedShortUrl = await _shortUrlService.UpdateByCode(putModel);
            if (updatedShortUrl == null)
            {
                return NotFound($"No URL found with code: {putModel.Code}");
            }
            return Ok(updatedShortUrl);
        }

        [HttpDelete()]
        public async Task<IActionResult> DeleteByCode(string code)
        {
            var deletedUrl = await _shortUrlService.DeleteByCode(code);
            if (deletedUrl == null)
            {
                return NotFound($"No URL found with code: {code}");
            }
            return NoContent();
        }
    }
}
