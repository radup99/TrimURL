using System.ComponentModel.DataAnnotations;

namespace TrimUrlApi.Models
{
    public class LoginPostModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
