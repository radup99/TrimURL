using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using TrimUrlApi.Database;
using TrimUrlApi.IntegrationTests.Infrastructure;

namespace TrimUrlApi.IntegrationTests.ShortUrls
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
                ApiRoutes.ShortUrls,
                new
                {
                    Url = "https://www.google.com/"
                });

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Create_ShouldReturnBadRequest_WhenUrlIsInvalid()
        {
            await _factory.ResetDatabaseAsync();

            var response = await _client.PostAsJsonAsync(
                ApiRoutes.ShortUrls,
                new
                {
                    Url = "not-a-url"
                });

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Create_ShouldReturnBadRequest_WhenRequestBodyIsInvalid()
        {
            await _factory.ResetDatabaseAsync();

            var response = await _client.PostAsJsonAsync(
                ApiRoutes.ShortUrls,
                new
                {
                    Website = "https://www.google.com/"
                });

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
