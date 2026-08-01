using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TrimUrlApi.Extensions;
using TrimUrlApi.Models;
using TrimUrlApi.Services;

namespace TrimUrlApi.Controllers
{
    [ApiController]
    [EnableRateLimiting(RateLimitPolicies.GeneralApi)]
    [Route("login")]
    public class AuthenticationController(ILogger<ShortUrlController> logger, IAuthenticationService authService) : ControllerBase
    {
        private readonly ILogger<ShortUrlController> _logger = logger;
        private readonly IAuthenticationService _authService = authService;

        /// <summary>
        /// Authenticates a user and returns a JSON Web Token (JWT).
        /// </summary>
        /// <response code="200">Authentication was successful and a JWT was returned.</response>
        /// <response code="401">The username or password is incorrect.</response>
        [HttpPost]
        [EnableRateLimiting(RateLimitPolicies.Authentication)]
        public async Task<IActionResult> Post(LoginPostModel loginModel)
        {
            var user = await _authService.GetUserByCredentials(loginModel);
            var jwtToken = _authService.GenerateJwtToken(user);
            return Ok(jwtToken);
        }
    }
}
