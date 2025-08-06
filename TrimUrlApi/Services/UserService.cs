using Microsoft.AspNetCore.Identity;
using TrimUrlApi.Entities;
using TrimUrlApi.Enums;
using TrimUrlApi.Models;
using TrimUrlApi.Repositories;

namespace TrimUrlApi.Services
{
    public class UserService(UserRepository userRepository)
    {
        private readonly UserRepository _userRepository = userRepository;

        public async Task<User> RegisterUser(UserPostModel postModel)
        {
            var user = new User
            {
                Username = postModel.Username,
                PasswordHash = GenerateHash(postModel.Password),
                Role = UserRole.Default,
                EmailAddress = postModel.EmailAddress,
                FullName = postModel.FullName,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            };
            await _userRepository.Create(user);
            return user;
        }

        public async Task<bool> IsUsernameAvailable(string username)
        {
            return await _userRepository.ReadByUsername(username) == null;
        }

        public async Task<bool> IsEmailAvailable(string email)
        {
            return await _userRepository.ReadByEmail(email) == null;
        }

        private static string GenerateHash(string password)
        {
            var hasher = new PasswordHasher<string>();
            return hasher.HashPassword("", password);
        }
    }
}
