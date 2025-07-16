using TrimUrlApi.Entities;
using TrimUrlApi.Models;
using TrimUrlApi.Repositories;

namespace TrimUrlApi.Services
{
    public class ShortUrlService(ShortUrlRepository suReporitory)
    {
        private readonly ShortUrlRepository _suRepository = suReporitory;
        private static readonly Random _random = new();

        public async Task<ShortUrlGetModel?> GetByCode(string code)
        {
            var shortUrl = await _suRepository.ReadByCode(code);
            if (shortUrl == null)
            {
                return null;
            }

            shortUrl.AccessCount++;
            await _suRepository.Update(shortUrl);
            return new ShortUrlGetModel(shortUrl);
        }

        public async Task<ShortUrl> Create(ShortUrlPostModel postModel)
        {
            var code = GenerateCode();
            while (await _suRepository.ReadByCode(code) != null)
            {
                code = GenerateCode();
            }
            var shortUrl = new ShortUrl
            {
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Url = postModel.Url,
                Code = code,
                ExpiresAt = (postModel.ExpiresAt != DateTime.MaxValue) ? postModel.ExpiresAt : null,
                AccessCount = 0,
            };
            await _suRepository.Create(shortUrl);
            return shortUrl;
        }

        public async Task<ShortUrl?> UpdateByCode(ShortUrlPutModel putModel)
        {
            var shortUrl = await _suRepository.ReadByCode(putModel.Code);
            if (shortUrl == null)
            {
                return null;
            }

            shortUrl.Url = putModel.Url;
            if (putModel.ExpiresAt != DateTime.MaxValue)
            {
                shortUrl.ExpiresAt = putModel.ExpiresAt;
            }
            shortUrl.UpdatedAt = DateTime.Now;
            await _suRepository.Update(shortUrl);
            return shortUrl;
        }

        public async Task<ShortUrl?> DeleteByCode(string code)
        {
            var shortUrl = await _suRepository.ReadByCode(code);
            if (shortUrl == null)
            {
                return null;
            }

            await _suRepository.DeleteById(shortUrl.Id);
            return shortUrl;
        }

        public bool IsValidUrl(string url)
        {
            _ = Uri.TryCreate(url, UriKind.Absolute, out var uriResult);
            return uriResult != null && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }

        private static string GenerateCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            return new string(Enumerable.Range(0, 6)
                .Select(_ => chars[_random.Next(chars.Length)])
                .ToArray());
        }
    }
}
