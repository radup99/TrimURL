using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using TrimUrlApi.Database;
using TrimUrlApi.IntegrationTests.Infrastructure;

namespace TrimUrlApi.IntegrationTests.ShortUrls
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
        public async Task GetByCode_ShouldReturnShortUrl()
        {
            await _factory.ResetDatabaseAsync();

            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MainDbContext>();

            const string validCode = "abc123";

            await DatabaseSeeder.SeedShortUrlAsync(
                db,
                code: validCode,
                url: "https://google.com"
            );


            var response = await _client.GetAsync(
                ApiRoutes.ShortUrlByCode(validCode)
             );

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }


        [Fact]
        public async Task GetByCode_ShouldReturnNotFound_WhenCodeDoesNotExist()
        {
            await _factory.ResetDatabaseAsync();

            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MainDbContext>();

            const string invalidCode = "def456";

            await DatabaseSeeder.SeedShortUrlAsync(
                db,
                code: "abc123",
                url: "https://google.com"
            );


            var response = await _client.GetAsync(
                ApiRoutes.ShortUrlByCode(invalidCode)
             );

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetByCode_ShouldReturnGone_WhenShortUrlIsExpired()
        {
            await _factory.ResetDatabaseAsync();

            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MainDbContext>();

            const string expiredCode = "abc123";

            await DatabaseSeeder.SeedShortUrlAsync(
                db,
                code: expiredCode,
                url: "https://google.com",
                expiresAt: DateTime.Parse("January 1, 2000")
            );


            var response = await _client.GetAsync(
                ApiRoutes.ShortUrlByCode(expiredCode)
             );

            Assert.Equal(HttpStatusCode.Gone, response.StatusCode);
        }
    }
}
