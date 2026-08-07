using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using TrimUrlApi.Database;
using TrimUrlApi.Models;
using TrimUrlApi.IntegrationTests.Infrastructure;
using TrimUrlApi.IntegrationTests.Helpers;

namespace TrimUrlApi.IntegrationTests.Users
{
    public class GetTests : IClassFixture<TrimUrlWebApplicationFactory>
    {
        private readonly TrimUrlWebApplicationFactory _factory;
        private readonly HttpClient _client;

        public GetTests(TrimUrlWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();

            using var scope = factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MainDbContext>();
            db.Database.EnsureCreated();
        }

        [Fact]
        public async Task GetCurrentUser_ShouldReturnUserModel_WhenAuthenticated()
        {
            await _factory.ResetDatabaseAsync();

            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MainDbContext>();

            await DatabaseSeeder.SeedUserAsync(
                db,
                username: "john",
                password: "!Password1",
                email: "john@test.com",
                fullName: "John Smith"
             );

            var token = await _client.LoginAndGetToken("john", "!Password1");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var authResponse = await _client.GetAsync(ApiRoutes.AuthenticatedUser);
            Assert.Equal(HttpStatusCode.OK, authResponse.StatusCode);

            var userModel = await authResponse.Content.ReadFromJsonAsync<UserResponseModel>();
            Assert.NotNull(userModel);
            Assert.Equal("john", userModel.Username);
            Assert.Equal("john@test.com", userModel.EmailAddress);
            Assert.Equal("John Smith", userModel.FullName);
        }

        [Fact]
        public async Task GetCurrentUser_ShouldReturnUnauthorized_WhenTokenIsMissng()
        {
            await _factory.ResetDatabaseAsync();

            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MainDbContext>();

            var authResponse = await _client.GetAsync(ApiRoutes.AuthenticatedUser);
            Assert.Equal(HttpStatusCode.Unauthorized, authResponse.StatusCode);
        }

        [Fact]
        public async Task GetCurrentUser_ShouldReturnUnauthorized_WhenTokenIsInvalid()
        {
            await _factory.ResetDatabaseAsync();

            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MainDbContext>();

            var invalidToken = "not-a-jwt";
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", invalidToken);

            var authResponse = await _client.GetAsync(ApiRoutes.AuthenticatedUser);
            Assert.Equal(HttpStatusCode.Unauthorized, authResponse.StatusCode);
        }
    }
}
