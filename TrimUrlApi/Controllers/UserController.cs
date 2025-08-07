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

            var userRespModel = await _userService.Create(postModel);
            return Ok(userRespModel);
        }

        [HttpGet()]
        public async Task<IActionResult> GetByUsername(string username)
        {
            var userRespModel = await _userService.GetByUsername(username);
            if (userRespModel == null)
            {
                return NotFound($"Username not found: {username}");
            }
            return Ok(userRespModel);
        }

        [HttpPut()]
        public async Task<IActionResult> UpdateByUsername(UserPutModel putModel)
        {
            if (putModel.Password == null && putModel.EmailAddress == null)
            {
                return BadRequest("At least one field must be provided: (password, emailAddress).");
            }

            var userRespModel = await _userService.UpdateByUsername(putModel);
            if (userRespModel == null)
            {
                return NotFound($"Username does not exist: {putModel.Username}");
            }
            return Ok(userRespModel);
        }
    }
}
