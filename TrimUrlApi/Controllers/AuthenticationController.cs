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
    public class AuthenticationController(ILogger<ShortUrlController> logger, IAuthenticationService authService, IConfiguration config) : ControllerBase
    {
        private readonly ILogger<ShortUrlController> _logger = logger;
        private readonly IAuthenticationService _authService = authService;
        private readonly IConfiguration _config = config;

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
