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
        public string EmailAddress { get; set; }

        [Required]
        public string FullName { get; set; }
    }
}
