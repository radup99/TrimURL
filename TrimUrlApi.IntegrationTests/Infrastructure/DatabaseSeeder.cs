using Microsoft.AspNetCore.Identity;
using TrimUrlApi.Database;
using TrimUrlApi.Entities;
using TrimUrlApi.Enums;

namespace TrimUrlApi.IntegrationTests.Infrastructure
{
    public static class DatabaseSeeder
    {
        public static async Task<User> SeedUserAsync(
            MainDbContext db,
            int id = 1,
            string username = "testuser",
            string password = "Password123!",
            UserRole role = UserRole.Default,
            string email = "test@test.com", 
            string fullName = "Test"
            )
        {
            var hasher = new PasswordHasher<User>();

            var user = new User
            {
                Id = id,
                Username = username,
                Role = role,
                EmailAddress = email,
                FullName = fullName
            };
            user.PasswordHash = hasher.HashPassword(user, password);

            db.Users.Add(user);
            await db.SaveChangesAsync();

            return user;
        }

        public static async Task<ShortUrl> SeedShortUrlAsync(
            MainDbContext db,
            int? creatorId = 1,
            string url = "https://google.com",
            string code = "abc123",
            DateTime? expiresAt = null,
            int accessCount = 0
            )
        {
            var shortUrl = new ShortUrl
            {
                CreatorId = creatorId,
                Url = url,
                Code = code,
                ExpiresAt = expiresAt,
                AccessCount = accessCount,
            };

            db.ShortUrls.Add(shortUrl);
            await db.SaveChangesAsync();

            return shortUrl;
        }
    }
}
