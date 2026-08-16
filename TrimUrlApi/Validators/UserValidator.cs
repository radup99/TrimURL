using System.Text.RegularExpressions;
using TrimUrlApi.Exceptions;

namespace TrimUrlApi.Validators
{
    public class UserValidator
    {
        public static void ValidateUsername(string username)
        {
            if (username.Length < 3)
                throw new InvalidFieldException("Username must be at least 3 characters long.");

            if (username.Length > 20)
                throw new InvalidFieldException("Username cannot exceed 20 characters.");

            if (!char.IsLetter(username[0]))
                throw new InvalidFieldException("Username must start with a letter.");

            foreach (char c in username)
            {
                if (char.IsLower(c) || char.IsDigit(c) || c == '_' || c == '-')
                    continue;

                throw new InvalidFieldException(
                    "Username may only contain lowercase letters, numbers, underscores (_) and hyphens (-).");
            }
        }

        public static void ValidatePassword(string password)
        {
            if (password.Length < 10)
            {
                throw new InvalidFieldException("Password must be at least 10 characters.");
            }

            if (password.Length > 50)
            {
                throw new InvalidFieldException("Maximum password length exceeded (50 characters).");
            }

            if (!password.Any(c => char.IsUpper(c)))
            {
                throw new InvalidFieldException("Password must contain an uppercase character.");
            }

            if (!password.Any(c => char.IsLower(c)))
            {
                throw new InvalidFieldException("Password must contain a lowercase character.");
            }

            if (!password.Any(c => char.IsDigit(c)))
            {
                throw new InvalidFieldException("Password must contain a digit.");
            }

            if (!password.Any(c => !char.IsLetterOrDigit(c)))
            {
                throw new InvalidFieldException("Password must contain a special character.");
            }
        }

        public static void ValidateFullName(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
            {
                throw new InvalidFieldException("Full name cannot be empty.");
            }

            if (fullName.Length > 100)
            {
                throw new InvalidFieldException("Full name cannot exceed 100 characters.");
            }

            if (!Regex.IsMatch(fullName, @"^[\p{L}]+(?:[ '-][\p{L}]+)*$"))
            {
                throw new InvalidFieldException("Full name contains invalid characters.");
            }
        }
    }
}
