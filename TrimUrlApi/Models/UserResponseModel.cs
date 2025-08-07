using TrimUrlApi.Entities;
using TrimUrlApi.Enums;

namespace TrimUrlApi.Models
{
    public class UserResponseModel
    {
        public string Username { get; set; }

        public string EmailAddress { get; set; }

        public string FullName { get; set; }

        public UserResponseModel(User user)
        {
            Username = user.Username;
            EmailAddress = user.EmailAddress;
            FullName = user.FullName;
        }
    }
}
