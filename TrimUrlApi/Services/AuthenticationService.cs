using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TrimUrlApi.Controllers;
using TrimUrlApi.Entities;
using TrimUrlApi.Exceptions;
using TrimUrlApi.Models;
using TrimUrlApi.Repositories;

namespace TrimUrlApi.Services
{
    public class AuthenticationService(IUserRepository userRepository, IConfiguration config, IPasswordHasher<string> hasher) : IAuthenticationService
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IConfiguration _config = config;
        private readonly IPasswordHasher<string> _hasher = hasher;

        public async Task<User?> GetUserByCredentials(LoginPostModel loginModel)
        {
            var user = await _userRepository.ReadByUsername(loginModel.Username);
            if (user == null)
            {
                throw new InvalidCredentialsException();
            }
            
            var result = _hasher.VerifyHashedPassword("", user.PasswordHash, loginModel.Password);
            if (result != PasswordVerificationResult.Success)
            {
                throw new InvalidCredentialsException();
            }
            return user;
        }

        public string GenerateJwtToken(User user)
        {
            var jwtSettings = _config.GetSection("JwtSettings");
            var jwtSecret = _config["Jwt:Secret"]; //value stored as user secret

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim("roleNum", ((int)user.Role).ToString()),
                new Claim("username", user.Username),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(Convert.ToDouble(jwtSettings["ExpiresInHours"])),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
