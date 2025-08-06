using Microsoft.AspNetCore.Mvc;
using TrimUrlApi.Models;
using TrimUrlApi.Services;

namespace TrimUrlApi.Controllers
{
    [ApiController]
    [Route("users")]
    public class UserController(ILogger<UserController> logger, UserService userService) : ControllerBase
    {
        private readonly ILogger<UserController> _logger = logger;
        private readonly UserService _userService = userService;

        [HttpPost()]
        public async Task<IActionResult> Create(UserPostModel postModel)
        {
            if (!await _userService.IsUsernameAvailable(postModel.Username) || !await _userService.IsEmailAvailable(postModel.EmailAddress))
            {
                return BadRequest("Username or email is already in use.");
            }

            await _userService.Create(postModel);
            return Ok();
        }
    }
}
