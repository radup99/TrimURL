using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using TrimUrlApi.Models;
using TrimUrlApi.Services;
using TrimUrlApi.Extensions;

namespace TrimUrlApi.Controllers
{
    [ApiController]
    [EnableRateLimiting(RateLimitPolicies.GeneralApi)]
    [Route("short-urls")]
    public class ShortUrlController(ILogger<ShortUrlController> logger, IShortUrlService shortUrlService) : ControllerBase
    {
        private readonly ILogger<ShortUrlController> _logger = logger;
        private readonly IShortUrlService _shortUrlService = shortUrlService;

        /// <summary>
        /// Retrieves information about a shortened URL.
        /// </summary>
        /// <param name="code">The short URL code.</param>
        /// <response code="200">The shortened URL was found.</response>
        /// <response code="404">No shortened URL exists with the specified code.</response>
        /// <response code="410">The shortened URL is expired.</response>
        [HttpGet("code/{code}")]
        public async Task<IActionResult> GetByCode(string code)
        {
            var getModel = await _shortUrlService.GetByCode(code);
            return Ok(getModel);
        }

        /// <summary>
        /// Retrieves all shortened URLs created by the authenticated user.
        /// </summary>
        /// <response code="200">The URLs were retrieved successfully.</response>
        /// <response code="401">Authentication is required.</response>
        /// <response code="404">The user has not created any shortened URLs.</response>
        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetByCreatorId()
        {
            var userId = User.GetAuthUserId();
            var shortUrlList = await _shortUrlService.GetByCreatorId(userId);
            return Ok(shortUrlList);
        }

        /// <summary>
        /// Creates a new shortened URL.
        /// </summary>
        /// <response code="200">The shortened URL was created successfully.</response>
        /// <response code="400">The provided URL is invalid.</response>
        [HttpPost()]
        [EnableRateLimiting(RateLimitPolicies.UrlCreation)]
        public async Task<IActionResult> Create(ShortUrlPostModel postModel)
        {
            int? userId = null;
            if (User.Identity?.IsAuthenticated == true)
            {
                userId = User.GetAuthUserId();
            }
            var shortUrl = await _shortUrlService.Create(postModel, userId);
            return Ok(shortUrl);
        }

        /// <summary>
        /// Updates a shortened URL created by the authenticated user.
        /// </summary>
        /// <param name="code">The short URL code.</param>
        /// <response code="200">The shortened URL was updated successfully.</response>
        /// <response code="400">The provided URL is invalid.</response>
        /// <response code="401">Authentication is required.</response>
        /// <response code="403">The authenticated user does not have permission to modify the shortened URL.</response>
        /// <response code="404">No shortened URL exists with the specified code.</response>
        [Authorize]
        [HttpPut("code/{code}")]
        public async Task<IActionResult> UpdateByCode(string code, ShortUrlPutModel putModel)
        {
            int? userId = User.GetAuthUserId();
            var updatedShortUrl = await _shortUrlService.UpdateByCode(code, putModel, userId);
            return Ok(updatedShortUrl);
        }

        /// <summary>
        /// Deletes a shortened URL created by the authenticated user.
        /// </summary>
        /// <param name="code">The short URL code.</param>
        /// <response code="204">The shortened URL was deleted successfully.</response>
        /// <response code="401">Authentication is required.</response>
        /// <response code="403">The authenticated user does not have permission to delete the shortened URL.</response>
        /// <response code="404">No shortened URL exists with the specified code.</response>
        [Authorize]
        [HttpDelete("code/{code}")]
        public async Task<IActionResult> DeleteByCode(string code)
        {
            int? userId = User.GetAuthUserId();
            await _shortUrlService.DeleteByCode(code, userId);
            return NoContent();
        }

        /// <summary>
        /// Deletes a shortened URL if the authenticated user has administrator privileges.
        /// </summary>
        /// <param name="code">The short URL code.</param>
        /// <response code="204">The shortened URL was deleted successfully.</response>
        /// <response code="401">Authentication is required.</response>
        /// <response code="403">The authenticated user does not have administrator privileges.</response>
        /// <response code="404">No shortened URL exists with the specified code.</response>
        [Authorize]
        [HttpDelete("admin/code/{code}")]
        public async Task<IActionResult> DeleteByCodeAsAdmin(string code)
        {
            if (!User.HasAdminPrivileges())
            {
                return Forbid();
            }

            await _shortUrlService.DeleteByCodeAsAdmin(code);
            return NoContent();
        }
    }
}
