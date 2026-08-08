using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Mail;
using TrimUrlApi.Database;
using TrimUrlApi.IntegrationTests.Helpers;
using TrimUrlApi.IntegrationTests.Infrastructure;
using TrimUrlApi.Models;
using Xunit.Abstractions;

namespace TrimUrlApi.IntegrationTests.Users
{
    public class DeleteTests : IClassFixture<TrimUrlWebApplicationFactory>
    {
        private readonly TrimUrlWebApplicationFactory _factory;
        private readonly HttpClient _client;
        private readonly ITestOutputHelper _output;

        public DeleteTests(TrimUrlWebApplicationFactory factory, ITestOutputHelper output)
        {
            _factory = factory;
            _client = factory.CreateClient();
            _output = output;

            using var scope = factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MainDbContext>();
            db.Database.EnsureCreated();
        }

        [Fact]
        public async Task Delete_ShouldDeleteUser()
        {
            await _factory.ResetDatabaseAsync();

            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MainDbContext>();

            await DatabaseSeeder.SeedUserAsync(
                db,
                username: "john",
                email: "john@test.com",
                password: "Password123!",
                fullName: "John Smith"
            );
            Assert.Contains(db.Users, u => u.Username == "john");

            var token = await _client.LoginAndGetToken("john", "Password123!");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.DeleteAsync(ApiRoutes.AuthenticatedUser);
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            Assert.DoesNotContain(db.Users, u => u.Username == "john");
        }

        [Fact]
        public async Task Delete_ShouldReturnUnauthorized_WhenTokenIsMissng()
        {
            await _factory.ResetDatabaseAsync();

            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MainDbContext>();

            var response = await _client.DeleteAsync(ApiRoutes.AuthenticatedUser);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
