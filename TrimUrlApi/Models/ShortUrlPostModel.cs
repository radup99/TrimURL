using System.ComponentModel.DataAnnotations;

namespace TrimUrlApi.Models
{
    public class ShortUrlPostModel
    {
        [Required]
        public string Url { get; set; }

        public DateTime? ExpiresAt { get; set; }
    }
}
