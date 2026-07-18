using System.ComponentModel.DataAnnotations;
namespace TrimUrlApi.Entities
{
    public class ShortUrl : BaseEntity
    {
        public int? CreatorId { get; set; }

        [MaxLength(2048)]
        public string Url { get; set; }

        [MaxLength(10)]
        public string Code { get; set; }

        public DateTime? ExpiresAt { get; set; }

        public int AccessCount { get; set; }
    }
}
