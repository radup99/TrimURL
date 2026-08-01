using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TrimUrlApi.Extensions;
using TrimUrlApi.Models;
using TrimUrlApi.Services;

namespace TrimUrlApi.Controllers
{
    [ApiController]
    [EnableRateLimiting(RateLimitPolicies.GeneralApi)]
    [Route("users")]
    public class UserController(ILogger<UserController> logger, IUserService userService) : ControllerBase
    {
        private readonly ILogger<UserController> _logger = logger;
        private readonly IUserService _userService = userService;

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <response code="200">The user was registered successfully.</response>
        /// <response code="400">The username, password or e-mail address is invalid.</response>
        /// <response code="409">The username or e-mail address is already in use.</response>
        [HttpPost()]
        [EnableRateLimiting(RateLimitPolicies.Authentication)]
        public async Task<IActionResult> Create(UserPostModel postModel)
        {
            var userRespModel = await _userService.Create(postModel);
            return Ok(userRespModel);
        }

        /// <summary>
        /// Retrieves the profile of the authenticated user.
        /// </summary>
        /// <response code="200">The profile was retrieved successfully.</response>
        /// <response code="401">Authentication is required.</response>
        /// <response code="404">The user does not exist.</response>
        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetByAuthUsername()
        {
            var username = User.GetAuthUsername();
            var userRespModel = await _userService.GetByUsername(username);
            return Ok(userRespModel);
        }

        /// <summary>
        /// Updates the e-mail address or password of the authenticated user.
        /// </summary>
        /// <response code="200">The user was updated successfully.</response>
        /// <response code="400">The username, password or e-mail address is invalid.</response>
        /// <response code="401">Authentication is required.</response>
        /// <response code="404">The user does not exist.</response>
        /// <response code="409">The username or e-mail address is already in use.</response>
        [Authorize]
        [HttpPut("me")]
        public async Task<IActionResult> UpdateByAuthUsername(UserPutModel putModel)
        {
            var username = User.GetAuthUsername();
            var userRespModel = await _userService.UpdateByUsername(username, putModel);
            return Ok(userRespModel);
        }

        /// <summary>
        /// Deletes the authenticated user.
        /// </summary>
        /// <response code="204">The user was deleted successfully.</response>
        /// <response code="401">Authentication is required.</response>
        /// <response code="404">The user does not exist.</response>
        [Authorize]
        [HttpDelete("me")]
        public async Task<IActionResult> DeleteByAuthUsername()
        {
            var username = User.GetAuthUsername();
            await _userService.DeleteByUsername(username);
            return NoContent();
        }
    }
}
