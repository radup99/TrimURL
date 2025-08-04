using TrimUrlApi.Enums;

namespace TrimUrlApi.Entities
{
    public class User : BaseEntity
    {
        public string Username { get; set; }

        public string PasswordHash { get; set; }

        public UserRole Role { get; set; }

        public string EmailAddress { get; set; }

        public string FullName { get; set; }

        public User() { }
    }
}
