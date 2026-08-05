using Microsoft.AspNetCore.Identity.Data;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using TrimUrlApi.Database;
using TrimUrlApi.IntegrationTests.Infrastructure;

namespace TrimUrlApi.IntegrationTests.Users
{
    public class CreateTests : IClassFixture<TrimUrlWebApplicationFactory>
    {
        private readonly TrimUrlWebApplicationFactory _factory;
        private readonly HttpClient _client;

        public CreateTests(TrimUrlWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();

            using var scope = factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MainDbContext>();
            db.Database.EnsureCreated();
        }

        [Fact]
        public async Task Create_ShouldReturnCreated()
        {
            await _factory.ResetDatabaseAsync();

            var response = await _client.PostAsJsonAsync(
                "/users",
                new
                {
                    Username = "john",
                    EmailAddress = "john@test.com",
                    Password = "Password123!",
                    FullName = "John Smith"
                });

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Create_ShouldReturnBadRequest_WhenUsernameAlreadyExists()
        {
            await _factory.ResetDatabaseAsync();

            // Arrange
            using var scope = _factory.Services.CreateScope();

            var db = scope.ServiceProvider.GetRequiredService<MainDbContext>();

            await DatabaseSeeder.SeedUserAsync(
                db,
                username: "john",
                email: "john@test.com"
             );


            var response = await _client.PostAsJsonAsync(
                "/users",
                new
                {
                    Username = "john",
                    EmailAddress = "different@test.com",
                    Password = "Password123!",
                    FullName = "John Smith"
                });

            // Assert
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }

        [Fact]
        public async Task Create_ShouldReturnBadRequest_WhenEmailAlreadyExists()
        {
            await _factory.ResetDatabaseAsync();

            // Arrange
            using var scope = _factory.Services.CreateScope();

            var db = scope.ServiceProvider.GetRequiredService<MainDbContext>();

            await DatabaseSeeder.SeedUserAsync(
                db,
                username: "john",
                email: "john@test.com"
             );


            var response = await _client.PostAsJsonAsync(
                "/users",
                new
                {
                    Username = "different_john",
                    EmailAddress = "john@test.com",
                    Password = "Password123!",
                    FullName = "John James"
                });

            // Assert
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }

        [Fact]
        public async Task Create_ShouldReturnBadRequest_WhenUsernameIsInvalid()
        {
            await _factory.ResetDatabaseAsync();

            var response = await _client.PostAsJsonAsync(
                "/users",
                new
                {
                    Username = "john:inv@l!d",
                    EmailAddress = "john@test.com",
                    Password = "Password123!",
                    FullName = "John Smith"
                });

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Create_ShouldReturnBadRequest_WhenEmailIsInvalid()
        {
            await _factory.ResetDatabaseAsync();

            var response = await _client.PostAsJsonAsync(
                "/users",
                new
                {
                    Username = "john",
                    EmailAddress = "johntestcom",
                    Password = "Password123!",
                    FullName = "John Smith"
                });

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Create_ShouldReturnBadRequest_WhenPasswordIsWeak()
        {
            await _factory.ResetDatabaseAsync();

            var response = await _client.PostAsJsonAsync(
                "/users",
                new
                {
                    Username = "john",
                    EmailAddress = "john@test.com",
                    Password = "weak-password",
                    FullName = "John Smith"
                });

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Create_ShouldReturnBadRequest_WhenRequestBodyIsInvalid()
        {
            await _factory.ResetDatabaseAsync();

            var response = await _client.PostAsJsonAsync(
                "/users",
                new
                {
                    User = "john",
                    Email = "johntestcom",
                    Pass = "Password123!",

                });

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
