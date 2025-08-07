using System.ComponentModel.DataAnnotations;

namespace TrimUrlApi.Models
{
    public class UserPutModel
    {
        [Required]
        public string Username { get; set; }

        public string? Password { get; set; } = null;

        [EmailAddress]
        public string? EmailAddress { get; set; } = null;
    }
}
