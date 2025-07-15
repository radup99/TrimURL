using System.ComponentModel.DataAnnotations;

namespace TrimUrlApi.Models
{
    public class ShortUrlPutModel
    {
        [Required]
        public string Code { get; set; }

        [Required]
        public string Url { get; set; }

        [Required]
        public DateTime? ExpiresAt { get; set; }
    }
}
