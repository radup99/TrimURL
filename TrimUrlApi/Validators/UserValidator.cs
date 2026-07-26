using TrimUrlApi.Exceptions;

namespace TrimUrlApi.Validators
{
    public class UserValidator
    {
        public static void ValidateUsername(string username)
        {
            if (username.Length < 3)
                throw new InvalidUsernameException("Username must be at least 3 characters long.");

            if (username.Length > 20)
                throw new InvalidUsernameException("Username cannot exceed 20 characters.");

            if (!char.IsLetter(username[0]))
                throw new InvalidUsernameException("Username must start with a letter.");

            foreach (char c in username)
            {
                if (char.IsLower(c) || char.IsDigit(c) || c == '_' || c == '-')
                    continue;

                throw new InvalidUsernameException(
                    "Username may only contain lowercase letters, numbers, underscores (_) and hyphens (-).");
            }
        }

        public static void ValidatePassword(string password)
        {
            if (password.Length < 10)
            {
                throw new WeakPasswordException("Password must be at least 10 characters.");
            }

            if (password.Length > 50)
            {
                throw new WeakPasswordException("Maximum password length exceeded (50 characters).");
            }

            if (!password.Any(c => char.IsUpper(c)))
            {
                throw new WeakPasswordException("Password must contain an uppercase character.");
            }

            if (!password.Any(c => char.IsLower(c)))
            {
                throw new WeakPasswordException("Password must contain a lowercase character.");
            }

            if (!password.Any(c => char.IsDigit(c)))
            {
                throw new WeakPasswordException("Password must contain a digit.");
            }

            if (!password.Any(c => !char.IsLetterOrDigit(c)))
            {
                throw new WeakPasswordException("Password must contain a special character.");
            }
        }
    }
}
