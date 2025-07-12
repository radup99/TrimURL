using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TrimUrlApi.Entities
{
    public abstract class BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonPropertyOrder(-3)]
        public int Id { get; set; }

        [JsonPropertyOrder(-2)]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyOrder(-1)]
        public DateTime UpdatedAt { get; set; }
    }
}
