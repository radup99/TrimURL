using System.ComponentModel.DataAnnotations;
using TrimUrlApi.Enums;

namespace TrimUrlApi.Models
{
    public class UserPostModel
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Username { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 10)]
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
