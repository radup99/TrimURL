﻿using TrimUrlApi.Entities;
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
                ExpiresAt = postModel.ExpiresAt,
                AccessCount = 0,
            };
            await _suRepository.Create(shortUrl);
            return shortUrl;
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
