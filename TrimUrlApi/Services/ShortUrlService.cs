using TrimUrlApi.Entities;
using TrimUrlApi.Models;
using TrimUrlApi.Repositories;
using TrimUrlApi.Exceptions;
using TrimUrlApi.Validators;

namespace TrimUrlApi.Services
{
    public class ShortUrlService(IShortUrlRepository suReporitory, ICacheService cacheService) : IShortUrlService
    {
        private readonly IShortUrlRepository _suRepository = suReporitory;
        private readonly ICacheService _cacheService = cacheService;
        private static readonly Random _random = new();

        public async Task<ShortUrlGetModel?> GetByCode(string code)
        {
            var cacheKey = GetCacheKey(code);
            var shortUrl = await _cacheService.GetAsync<ShortUrl>(cacheKey)
                         ?? await GetByCodeOrThrow(code);

            if (shortUrl.ExpiresAt < DateTime.UtcNow)
            {
                await _cacheService.RemoveAsync(cacheKey);
                throw new ShortUrlExpiredException();
            }

            shortUrl.AccessCount++;
            await _suRepository.Update(shortUrl);
            await _cacheService.SetAsync(cacheKey, shortUrl, TimeSpan.FromHours(24));

            return new ShortUrlGetModel(shortUrl);
        }

        public async Task<List<ShortUrlGetModel>> GetByCreatorId(int? id)
        {
            var shortUrlList = await _suRepository.ReadByCreatorId(id);
            if (id == null || shortUrlList.Count == 0)
            {
                throw new ShortUrlsNotFoundException();
            }

            return shortUrlList.Select(su => new ShortUrlGetModel(su)).ToList();
        }

        public async Task<ShortUrl> Create(ShortUrlPostModel postModel, int? userId)
        {
            ShortUrlValidator.ValidateUrl(postModel.Url);

            var code = GenerateCode();
            while (await _suRepository.ReadByCode(code) != null)
            {
                code = GenerateCode();
            }

            var shortUrl = new ShortUrl
            {
                CreatorId = userId,
                Url = postModel.Url,
                Code = code,
                ExpiresAt = (postModel.ExpiresAt != DateTime.MaxValue) ? postModel.ExpiresAt : null,
                AccessCount = 0,
            };
            await _suRepository.Create(shortUrl);

            return shortUrl;
        }

        public async Task<ShortUrl?> UpdateByCode(string code, ShortUrlPutModel putModel, int? userId)
        {
            if (putModel.Url != null)
            {
                ShortUrlValidator.ValidateUrl(putModel.Url);
            }

            var shortUrl = await GetByCodeOrThrow(code);
            EnsureOwnershipOrThrow(shortUrl, userId);

            if (putModel.Url != null)
            {
                shortUrl.Url = putModel.Url;
            }
            
            if (putModel.ExpiresAt != DateTime.MaxValue)
            {
                shortUrl.ExpiresAt = putModel.ExpiresAt;
            }

            var cacheKey = GetCacheKey(code);
            await _suRepository.Update(shortUrl);
            await _cacheService.SetAsync(cacheKey, shortUrl, TimeSpan.FromHours(24));

            return shortUrl;
        }

        public async Task<ShortUrl?> DeleteByCode(string code, int? userId)
        {
            var shortUrl = await GetByCodeOrThrow(code);
            EnsureOwnershipOrThrow(shortUrl, userId);

            var cacheKey = GetCacheKey(code);
            await _suRepository.DeleteById(shortUrl.Id);
            await _cacheService.RemoveAsync(cacheKey);

            return shortUrl;
        }

        public async Task<ShortUrl?> DeleteByCodeAsAdmin(string code)
        {
            var shortUrl = await GetByCodeOrThrow(code);

            var cacheKey = GetCacheKey(code);
            await _suRepository.DeleteById(shortUrl.Id);
            await _cacheService.RemoveAsync(cacheKey);

            return shortUrl;
        }

        private async Task<ShortUrl> GetByCodeOrThrow(string code)
        {
            var shortUrl = await _suRepository.ReadByCode(code);
            if (shortUrl == null)
            {
                throw new ShortUrlNotFoundByCodeException(code);
            }
            return shortUrl;
        }

        private static void EnsureOwnershipOrThrow(ShortUrl shortUrl, int? userId)
        {
            if (shortUrl.CreatorId == null || shortUrl.CreatorId != userId)
            {
                throw new ForbiddenShortUrlAccessException();
            }
        }

        private static string GenerateCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            return new string(Enumerable.Range(0, 6)
                .Select(_ => chars[_random.Next(chars.Length)])
                .ToArray());
        }

        private static string GetCacheKey(string code)
        {
            return $"shorturl:{code}";
        }
    }
}
