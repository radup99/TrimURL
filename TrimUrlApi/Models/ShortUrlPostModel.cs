using TrimUrlApi.Entities;

namespace TrimUrlApi.Models
{
    public class ShortUrlPostModel
    {
        public string Url { get; set; }

        public DateTime? ExpiresAt { get; set; }
    }
}
