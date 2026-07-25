using System.ComponentModel.DataAnnotations;
using TrimUrlApi.Enums;

namespace TrimUrlApi.Models
{
    public class UserPostModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(254)]
        public string EmailAddress { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 5)]
        public string FullName { get; set; }
    }
}
