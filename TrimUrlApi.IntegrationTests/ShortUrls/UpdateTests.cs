using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using TrimUrlApi.Database;
using TrimUrlApi.Entities;
using TrimUrlApi.IntegrationTests.Helpers;
using TrimUrlApi.IntegrationTests.Infrastructure;
using TrimUrlApi.Models;

namespace TrimUrlApi.IntegrationTests.ShortUrls
{
    public class UpdateTests : IClassFixture<TrimUrlWebApplicationFactory>
    {
        private readonly TrimUrlWebApplicationFactory _factory;
        private readonly HttpClient _client;

        public UpdateTests(TrimUrlWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();

            using var scope = factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MainDbContext>();
            db.Database.EnsureCreated();
        }

        [Fact]
        public async Task UpdateByCode_ShouldUpdateUrl()
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

            var response = await _client.PutAsJsonAsync(
                ApiRoutes.ShortUrlByCode(validCode),
                new
                {
                    Url = "https://wikipedia.org"
                }
            );
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var shortUrl = await response.Content.ReadFromJsonAsync<ShortUrl>();
            Assert.NotNull(shortUrl);
            Assert.Equal("https://wikipedia.org", shortUrl.Url);
        }

        [Fact]
        public async Task UpdateByCode_ShouldReturnUnauthorized_WhenTokenIsMissing()
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

            var response = await _client.PutAsJsonAsync(
                ApiRoutes.ShortUrlByCode(validCode),
                new
                {
                    Url = "https://wikipedia.org"
                }
            );
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task UpdateByCode_ShouldReturnForbidden_WhenUserDoesNotOwnShortUrl()
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

            var response = await _client.PutAsJsonAsync(
                ApiRoutes.ShortUrlByCode(validCode),
                new
                {
                    Url = "https://wikipedia.org"
                }
            );
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task UpdateByCode_ShouldReturnForbidden_WhenShortUrlHasNoCreatorId()
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

            var response = await _client.PutAsJsonAsync(
                ApiRoutes.ShortUrlByCode(validCode),
                new
                {
                    Url = "https://wikipedia.org"
                }
            );
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task UpdateByCode_ShouldReturnNotFound_WhenShortUrlDoesNotExist()
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

            var response = await _client.PutAsJsonAsync(
                ApiRoutes.ShortUrlByCode(invalidCode),
                new
                {
                    Url = "https://wikipedia.org"
                }
            );
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task UpdateByCode_ShouldReturnBadRequest_WhenUrlIsInvalid()
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

            var response = await _client.PutAsJsonAsync(
                ApiRoutes.ShortUrlByCode(validCode),
                new
                {
                    Url = "not-a-url"
                }
            );
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        }
    }
}
