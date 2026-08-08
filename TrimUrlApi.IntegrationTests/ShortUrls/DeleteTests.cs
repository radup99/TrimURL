using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Headers;
using TrimUrlApi.Database;
using TrimUrlApi.IntegrationTests.Helpers;
using TrimUrlApi.IntegrationTests.Infrastructure;
using TrimUrlApi.Enums;

namespace TrimUrlApi.IntegrationTests.ShortUrls
{
    public class DeleteTests : IClassFixture<TrimUrlWebApplicationFactory>
    {
        private readonly TrimUrlWebApplicationFactory _factory;
        private readonly HttpClient _client;

        public DeleteTests(TrimUrlWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();

            using var scope = factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MainDbContext>();
            db.Database.EnsureCreated();
        }

        [Fact]
        public async Task DeleteByCode_ShouldDeleteShortUrl()
        {
            await _factory.ResetDatabaseAsync();

            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MainDbContext>();

            const string validCode = "abc123";

            await DatabaseSeeder.SeedShortUrlAsync(
                db,
                creatorId: 1,
                code: validCode,
                url: "https://google.com"
            );
            Assert.Contains(db.ShortUrls, su => su.Code == validCode);

            await DatabaseSeeder.SeedUserAsync(
                db,
                id: 1,
                username: "john",
                password: "!Password1",
                email: "john@test.com",
                fullName: "John Smith"
             );

            var token = await _client.LoginAndGetToken("john", "!Password1");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.DeleteAsync(ApiRoutes.ShortUrlByCode(validCode));
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            Assert.DoesNotContain(db.ShortUrls, su => su.Code == validCode);
        }

        [Fact]
        public async Task DeleteByCode_ShouldReturnUnauthorized_WhenTokenIsMissing()
        {
            await _factory.ResetDatabaseAsync();

            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MainDbContext>();

            const string validCode = "abc123";

            await DatabaseSeeder.SeedShortUrlAsync(
                db,
                creatorId: 1,
                code: validCode,
                url: "https://google.com"
            );

            var response = await _client.DeleteAsync(ApiRoutes.ShortUrlByCode(validCode));
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task DeleteByCode_ShouldReturnForbidden_WhenUserDoesNotOwnShortUrl()
        {
            await _factory.ResetDatabaseAsync();

            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MainDbContext>();

            const string validCode = "abc123";

            await DatabaseSeeder.SeedShortUrlAsync(
                db,
                creatorId: 2,
                code: validCode,
                url: "https://google.com"
            );

            await DatabaseSeeder.SeedUserAsync(
                db,
                id: 1,
                username: "john",
                password: "!Password1",
                email: "john@test.com",
                fullName: "John Smith"
             );

            var token = await _client.LoginAndGetToken("john", "!Password1");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.DeleteAsync(ApiRoutes.ShortUrlByCode(validCode));
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task DeleteByCode_ShouldReturnForbidden_WhenShortUrlHasNoCreatorId()
        {
            await _factory.ResetDatabaseAsync();

            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MainDbContext>();

            const string validCode = "abc123";

            await DatabaseSeeder.SeedShortUrlAsync(
                db,
                creatorId: null,
                code: validCode,
                url: "https://google.com"
            );

            await DatabaseSeeder.SeedUserAsync(
                db,
                id: 1,
                username: "john",
                password: "!Password1",
                email: "john@test.com",
                fullName: "John Smith"
             );

            var token = await _client.LoginAndGetToken("john", "!Password1");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.DeleteAsync(ApiRoutes.ShortUrlByCode(validCode));
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task DeleteByCode_ShouldReturnNotFound_WhenShortUrlDoesNotExist()
        {
            await _factory.ResetDatabaseAsync();

            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MainDbContext>();

            const string invalidCode = "abc123";

            await DatabaseSeeder.SeedUserAsync(
                db,
                id: 1,
                username: "john",
                password: "!Password1",
                email: "john@test.com",
                fullName: "John Smith"
             );

            var token = await _client.LoginAndGetToken("john", "!Password1");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.DeleteAsync(ApiRoutes.ShortUrlByCode(invalidCode));
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task DeleteByCodeAsAdmin_ShouldDeleteShortUrl_WhenCreatorIdIsDifferent()
        {
            await _factory.ResetDatabaseAsync();

            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MainDbContext>();

            const string validCode = "abc123";

            await DatabaseSeeder.SeedShortUrlAsync(
                db,
                creatorId: 2,
                code: validCode,
                url: "https://google.com"
            );
            Assert.Contains(db.ShortUrls, su => su.Code == validCode);

            await DatabaseSeeder.SeedUserAsync(
                db,
                id: 1,
                role: UserRole.Admin,
                username: "admin",
                password: "!Password1",
                email: "admin@test.com",
                fullName: "Administrator"
             );

            var token = await _client.LoginAndGetToken("admin", "!Password1");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.DeleteAsync(ApiRoutes.ShortUrlByCodeAsAdmin(validCode));
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            Assert.DoesNotContain(db.ShortUrls, su => su.Code == validCode);
        }

        [Fact]
        public async Task DeleteByCodeAsAdmin_ShoulReturnUnauthorized_WhenTokenIsMissing()
        {
            await _factory.ResetDatabaseAsync();

            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MainDbContext>();

            const string validCode = "abc123";

            await DatabaseSeeder.SeedShortUrlAsync(
                db,
                creatorId: 2,
                code: validCode,
                url: "https://google.com"
            );

            var response = await _client.DeleteAsync(ApiRoutes.ShortUrlByCodeAsAdmin(validCode));
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task DeleteByCodeAsAdmin_ShouldReturnForbidden_WhenUserIsNotAdmin()
        {
            await _factory.ResetDatabaseAsync();

            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MainDbContext>();

            const string validCode = "abc123";

            await DatabaseSeeder.SeedShortUrlAsync(
                db,
                creatorId: 2,
                code: validCode,
                url: "https://google.com"
            );

            await DatabaseSeeder.SeedUserAsync(
                db,
                id: 1,
                role: UserRole.Default,
                username: "john",
                password: "!Password1",
                email: "john@test.com",
                fullName: "John Smith"
             );

            var token = await _client.LoginAndGetToken("john", "!Password1");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.DeleteAsync(ApiRoutes.ShortUrlByCodeAsAdmin(validCode));
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task DeleteByCodeAsAdmin_ShouldReturnNotFound_WhenShortUrlDoesNotExist()
        {
            await _factory.ResetDatabaseAsync();

            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MainDbContext>();

            const string invalidCode = "abc123";

            await DatabaseSeeder.SeedUserAsync(
                db,
                id: 1,
                role: UserRole.Admin,
                username: "admin",
                password: "!Password1",
                email: "admin@test.com",
                fullName: "Administrator"
             );

            var token = await _client.LoginAndGetToken("admin", "!Password1");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.DeleteAsync(ApiRoutes.ShortUrlByCodeAsAdmin(invalidCode));
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
