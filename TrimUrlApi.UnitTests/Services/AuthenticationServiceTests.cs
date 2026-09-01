using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using TrimUrlApi.Entities;
using TrimUrlApi.Exceptions;
using TrimUrlApi.Models;
using TrimUrlApi.Repositories;
using TrimUrlApi.Services;

namespace TrimUrlApi.Tests.Services
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
        public async Task GetUserByCredentials_ValidCredentials_ReturnsUser()
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
        public async Task GetUserByCredentials_UserDoesNotExist_ThrowsInvalidCredentialsException()
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
        public async Task GetUserByCredentials_InvalidPassword_ThrowsInvalidCredentialsException()
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
    }
}