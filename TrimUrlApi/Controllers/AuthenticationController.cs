﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TrimUrlApi.Entities;
using TrimUrlApi.Models;
using TrimUrlApi.Services;

namespace TrimUrlApi.Controllers
{
    [ApiController]
    [Route("login")]
    public class AuthenticationController(ILogger<ShortUrlController> logger, AuthenticationService authService, IConfiguration config) : ControllerBase
    {
        private readonly ILogger<ShortUrlController> _logger = logger;
        private readonly AuthenticationService _authService = authService;
        private readonly IConfiguration _config = config;

        [HttpPost]
        public async Task<IActionResult> Post(LoginPostModel loginModel)
        {
            var user = await _authService.GetUserByCredentials(loginModel);
            if (user == null)
            {
                return Unauthorized("Invalid username or password.");
            }
            var jwtToken = _authService.GenerateJwtToken(user);
            return Ok(jwtToken);
        }
    }
}
