using System.ComponentModel.DataAnnotations;
using TrimUrlApi.Enums;

namespace TrimUrlApi.Entities
{
    public class User : BaseEntity
    {
        [MaxLength(50)]
        public string Username { get; set; }

        [MaxLength(255)]
        public string PasswordHash { get; set; }

        [MaxLength(1)]
        public UserRole Role { get; set; }

        [MaxLength(254)]
        public string EmailAddress { get; set; }

        [MaxLength(100)]
        public string FullName { get; set; }

        public User() { }
    }
}
