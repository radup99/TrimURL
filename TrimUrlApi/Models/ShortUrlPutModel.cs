using System.ComponentModel.DataAnnotations;

namespace TrimUrlApi.Models
{
    public class ShortUrlPutModel
    {
        [StringLength(2048)]
        public string? Url { get; set; } = null;

        public DateTime? ExpiresAt { get; set; } = DateTime.MaxValue;
    }
}
