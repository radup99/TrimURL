using TrimUrlApi.Entities;

namespace TrimUrlApi.Models
{
    public class ShortUrlGetModel
    {
        public int Id { get; set; }

        public string Url { get; set; }

        public string Code { get; set; }

        public int AccessCount { get; set; }

        public ShortUrlGetModel(ShortUrl shortUrl)
        {
            Id = shortUrl.Id;
            Url = shortUrl.Url;
            Code = shortUrl.Code;
            AccessCount = shortUrl.AccessCount;
        }
    }
}
