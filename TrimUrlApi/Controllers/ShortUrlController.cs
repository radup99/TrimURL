using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TrimUrlApi.Models;
using TrimUrlApi.Services;
using TrimUrlApi.Extensions;

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

            if (getModel.ExpiresAt < DateTime.Now)
            {
                return StatusCode(StatusCodes.Status410Gone, "URL expired");
            }
            return Ok(getModel);
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetByCreatorId()
        {
            var creatorId = User.GetAuthUserId();
            var shortUrlList = await _shortUrlService.GetByCreatorId(creatorId);
            if (creatorId == null || shortUrlList.Count == 0)
            {
                return NotFound($"No URLs found");
            }

            return Ok(shortUrlList);
        }

        [HttpPost()]
        public async Task<IActionResult> Create(ShortUrlPostModel postModel)
        {
            if (!_shortUrlService.IsValidUrl(postModel.Url))
            {
                return BadRequest($"Invalid URL string: {postModel.Url}");
            }

            int? creatorId = null;
            if (User.Identity?.IsAuthenticated == true)
            {
                creatorId = User.GetAuthUserId();
            }
            var shortUrl = await _shortUrlService.Create(postModel, creatorId);
            return Ok(shortUrl);
        }

        [Authorize]
        [HttpPut()]
        public async Task<IActionResult> UpdateByCode(ShortUrlPutModel putModel)
        {
            if (!_shortUrlService.IsValidUrl(putModel.Url))
            {
                return BadRequest($"Invalid URL string: {putModel.Url}");
            }

            int? creatorId = User.GetAuthUserId();
            var updatedShortUrl = await _shortUrlService.UpdateByCode(putModel, creatorId);
            if (updatedShortUrl == null)
            {
                return Unauthorized();
            }
            return Ok(updatedShortUrl);
        }

        [Authorize]
        [HttpDelete()]
        public async Task<IActionResult> DeleteByCode(string code)
        {
            int? creatorId = User.GetAuthUserId();
            var deletedUrl = await _shortUrlService.DeleteByCode(code, creatorId);
            if (deletedUrl == null)
            {
                return Unauthorized();
            }
            return NoContent();
        }
    }
}
