namespace TrimUrlApi.Exceptions
{
    public class InvalidUsernameException : ApiException
    {
        public InvalidUsernameException(string usernameErrorText)
            : base(usernameErrorText, StatusCodes.Status400BadRequest)
        {
        }
    }
}
