using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using TrimUrlApi.Database;
using TrimUrlApi.Entities;
using TrimUrlApi.IntegrationTests.Infrastructure;
using TrimUrlApi.Models;

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
        public async Task Create_ShouldCreateShortUrl()
        {
            await _factory.ResetDatabaseAsync();

            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MainDbContext>();

            var response = await _client.PostAsJsonAsync(
                ApiRoutes.ShortUrls,
                new
                {
                    Url = "https://www.google.com/"
                });

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var shortUrl = await response.Content.ReadFromJsonAsync<ShortUrl>();
            Assert.NotNull(shortUrl);
            Assert.Equal("https://www.google.com/", shortUrl.Url);

            Assert.Contains(
                db.ShortUrls,
                su => su.Code == shortUrl.Code &&  su.Url == shortUrl.Url
             );
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
