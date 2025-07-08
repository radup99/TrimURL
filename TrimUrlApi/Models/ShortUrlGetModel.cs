using TrimUrlApi.Entities;

namespace TrimUrlApi.Models
{
    public class ShortUrlGetModel
    {
        public string Url { get; set; }

        public string Code { get; set; }

        public ShortUrlGetModel(ShortUrl shortUrl)
        {
            Url = shortUrl.Url;
            Code = shortUrl.Code;
        }
    }
}
