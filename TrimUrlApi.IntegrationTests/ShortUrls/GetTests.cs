using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using TrimUrlApi.Database;
using TrimUrlApi.IntegrationTests.Helpers;
using TrimUrlApi.IntegrationTests.Infrastructure;
using TrimUrlApi.Models;

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


            var response = await _client.GetAsync(ApiRoutes.ShortUrlByCode(validCode));
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var getModel = await response.Content.ReadFromJsonAsync<ShortUrlGetModel>();
            Assert.NotNull(getModel);
            Assert.Equal("https://google.com", getModel.Url);
            Assert.Equal(validCode, getModel.Code);
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

        [Fact]
        public async Task GetByAuthUser_ShouldReturnShortUrlList()
        {
            await _factory.ResetDatabaseAsync();

            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MainDbContext>();

            await DatabaseSeeder.SeedUserAsync(
                db,
                id: 1,
                username: "john",
                password: "!Password123"
            );

            await DatabaseSeeder.SeedShortUrlAsync(
                db,
                creatorId: 1,
                code: "abc123",
                url: "https://google.com"
            );

            // ShortUrl from a different user
            await DatabaseSeeder.SeedShortUrlAsync(
                db,
                creatorId: 2,
                code: "xyz000",
                url: "https://instagram.com"
            );

            await DatabaseSeeder.SeedShortUrlAsync(
                db,
                creatorId: 1,
                code: "def456",
                url: "https://wikipedia.org"
            );

            await DatabaseSeeder.SeedShortUrlAsync(
                db,
                creatorId: 1,
                code: "ghi789",
                url: "https://youtube.com"
            );

            var token = await _client.LoginAndGetToken("john", "!Password123");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync(ApiRoutes.ShortUrlsFromAuthUser);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var shortUrlList = await response.Content.ReadFromJsonAsync<List<ShortUrlGetModel>>();
            Assert.NotNull(shortUrlList);
            Assert.NotEmpty(shortUrlList);
            Assert.Equal(3, shortUrlList.Count);
            Assert.DoesNotContain(shortUrlList, su => su.Code == "xyz000");
        }

        [Fact]
        public async Task GetByAuthUser_ShouldReturnNotFound_IfUserHasNoShortUrls()
        {
            await _factory.ResetDatabaseAsync();

            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MainDbContext>();

            await DatabaseSeeder.SeedUserAsync(
                db,
                id: 1,
                username: "john",
                password: "!Password123"
            );

            // ShortUrl from a different user
            await DatabaseSeeder.SeedShortUrlAsync(
                db,
                creatorId: 2,
                code: "xyz000",
                url: "https://instagram.com"
            );

            var token = await _client.LoginAndGetToken("john", "!Password123");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync(ApiRoutes.ShortUrlsFromAuthUser);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetByAuthUser_ShouldReturnUnauthorized_IfTokenIsMissing()
        {
            await _factory.ResetDatabaseAsync();

            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MainDbContext>();

            var response = await _client.GetAsync(ApiRoutes.ShortUrlsFromAuthUser);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
