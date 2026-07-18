using System.ComponentModel.DataAnnotations;

namespace TrimUrlApi.Models
{
    public class LoginPostModel
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Username { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 10)]
        public string Password { get; set; }
    }
}
