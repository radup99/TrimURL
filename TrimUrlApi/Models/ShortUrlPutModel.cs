using System.ComponentModel.DataAnnotations;

namespace TrimUrlApi.Models
{
    public class ShortUrlPutModel
    {
        [Required]
        [StringLength(2048)]
        public string Url { get; set; }

        public DateTime? ExpiresAt { get; set; } = DateTime.MaxValue;
    }
}
