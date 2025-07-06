namespace TrimUrlApi.Entities
{
    public class ShortUrl : BaseEntity
    {
        public int CreatorId { get; set; }

        public string Url { get; set; }

        public string Code { get; set; }

        public DateTime ExpiresAt { get; set; }
    }
}
