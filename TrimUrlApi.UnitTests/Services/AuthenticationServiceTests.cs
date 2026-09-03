using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TrimUrlApi.Entities;
using TrimUrlApi.Enums;
using TrimUrlApi.Exceptions;
using TrimUrlApi.Models;
using TrimUrlApi.Repositories;
using TrimUrlApi.Services;

namespace TrimUrlApi.UnitTests.Services
{
    public class AuthenticationServiceTests
    {
        private readonly Mock<IUserRepository> _repoMock;
        private readonly Mock<IConfiguration> _configMock;
        private readonly Mock<IPasswordHasher<string>> _hasherMock;

        private readonly AuthenticationService _service;

        public AuthenticationServiceTests()
        {
            _repoMock = new Mock<IUserRepository>();
            _configMock = new Mock<IConfiguration>();
            _hasherMock = new Mock<IPasswordHasher<string>>();

            _service = new AuthenticationService(_repoMock.Object, _configMock.Object, _hasherMock.Object);
        }

        [Fact]
        public async Task GetUserByCredentials_ShouldReturnUser_WhenCredentialsAreValid()
        {
            var loginModel = new LoginPostModel
            {
                Username = "john",
                Password = "!Password123"
            };

            var user = new User
            {
                Id = 1,
                Username = "john",
                PasswordHash = "hashedpassword"
            };

            _repoMock.Setup(r => r.ReadByUsername(loginModel.Username)).ReturnsAsync(user);

            _hasherMock
                .Setup(h => h.VerifyHashedPassword("", user.PasswordHash, loginModel.Password))
                .Returns(PasswordVerificationResult.Success);

            var result = await _service.GetUserByCredentials(loginModel);
            Assert.Equal(user, result);
        }

        [Fact]
        public async Task GetUserByCredentials_ShouldThrowException_WhenUserDoesNotExist()
        {
            var loginModel = new LoginPostModel
            {
                Username = "does-not-exist",
                Password = "password"
            };

            _repoMock.Setup(x => x.ReadByUsername(loginModel.Username)).ReturnsAsync((User?)null);
            await Assert.ThrowsAsync<InvalidCredentialsException>(() => _service.GetUserByCredentials(loginModel));
        }

        [Fact]
        public async Task GetUserByCredentials_ShouldThrowException_WhenPasswordIsIncorrect()
        {
            var loginModel = new LoginPostModel
            {
                Username = "john",
                Password = "incorrect-password"
            };

            var user = new User
            {
                Id = 1,
                Username = "john",
                PasswordHash = "hashedpassword"
            };

            _repoMock.Setup(r => r.ReadByUsername(loginModel.Username)).ReturnsAsync(user);

            _hasherMock
                .Setup(h => h.VerifyHashedPassword("", user.PasswordHash, loginModel.Password))
                .Returns(PasswordVerificationResult.Failed);

            await Assert.ThrowsAsync<InvalidCredentialsException>(() => _service.GetUserByCredentials(loginModel));
        }

        [Fact]
        public void GenerateJwtToken_ShouldReturnValidJwt_WhenUserIsValid()
                {
                    // Arrange
                    var configuration = new ConfigurationBuilder()
                        .AddInMemoryCollection(new Dictionary<string, string?>
                        {
                            ["Jwt:Secret"] = "ThisIsAReallyLongSecretKeyForTests123!",
                            ["JwtSettings:Issuer"] = "TrimUrlApi",
                            ["JwtSettings:Audience"] = "TrimUrl",
                            ["JwtSettings:ExpiresInHours"] = "336"
                        })
                        .Build();

                    var service = new AuthenticationService(_repoMock.Object, configuration, _hasherMock.Object);

                    var user = new User
                    {
                        Id = 1,
                        Username = "john",
                        Role = UserRole.Default
                    };

                    var tokenString = service.GenerateJwtToken(user);
                    Assert.False(string.IsNullOrWhiteSpace(tokenString));

                    var handler = new JwtSecurityTokenHandler();
                    var token = handler.ReadJwtToken(tokenString);

                    Assert.Equal("1", token.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value);
                    Assert.Equal("john", token.Claims.First(c => c.Type == "username").Value);
                    Assert.Equal(
                        user.Role.ToString(),
                        token.Claims.First(c => c.Type == ClaimTypes.Role).Value);
                    Assert.Equal(
                        ((int)user.Role).ToString(),
                        token.Claims.First(c => c.Type == "roleNum").Value);

                    Assert.Equal("TrimUrlApi", token.Issuer);
                    Assert.Contains("TrimUrlClient", token.Audiences);

                    Assert.True(token.ValidTo > DateTime.UtcNow);
                    Assert.True(token.ValidTo <= DateTime.UtcNow.AddHours(2).AddSeconds(1));
                }
    }
}
